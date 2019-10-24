// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Experimental.Utilities
{
    public class TapToPlace : Solver, IMixedRealityPointerHandler, IMixedRealitySpeechHandler
    {
        // TODO: Tooltip and stuff
        public GameObject GameObjectToPlace;

        [SerializeField]
        [Tooltip("TODO")]
        private bool AutoStart = true;

        [SerializeField]
        [Tooltip("TODO - in meters")]
        private float defaultPlacementDistance = 2.0f;

        /// <summary>
        /// TODO
        /// </summary>
        public float DefaultPlacementDistance
        {
            get { return defaultPlacementDistance; }
            set { defaultPlacementDistance = value; }
        }

        [SerializeField]
        [Tooltip("Max distance for raycast to check for surfaces")]
        private float maxRaycastDistance = 50.0f;

        /// <summary>
        /// Max distance for raycast to check for surfaces
        /// </summary>
        public float MaxRaycastDistance
        {
            get { return maxRaycastDistance; }
            set { maxRaycastDistance = value; }
        }

        [SerializeField]
        [Tooltip("Array of LayerMask to execute from highest to lowest priority. First layermask to provide a raycast hit will be used by component")]
        private LayerMask[] magneticSurfaces = { UnityEngine.Physics.DefaultRaycastLayers };

        /// <summary>
        /// Array of LayerMask to execute from highest to lowest priority. First layermask to provide a raycast hit will be used by component
        /// </summary>
        public LayerMask[] MagneticSurfaces
        {
            get { return magneticSurfaces; }
            set { magneticSurfaces = value; }
        }

        [SerializeField]
        [Tooltip("TODO")]
        private List<string> keywords;

        public List<string> Keywords
        {
            get { return keywords; }
            set { keywords = value; }
        }

        public bool IsBeingPlaced { get; protected set; }

        // auto-start, if false disables SolverHandler?

        protected RaycastHit currentHit;
        protected bool didHit;
        protected RayStep currentRay;

        protected override void Start()
        {
            base.Start();

            SolverHandler.UpdateSolvers = AutoStart;
            if (AutoStart)
            {
                StartPlacement();
            }
        }

        // TODO: OnEnable/OnDisable()?

        public void StartPlacement()
        {
            SolverHandler.UpdateSolvers = true;

            CoreServices.InputSystem?.RegisterHandler<IMixedRealitySpeechHandler>(this);
            CoreServices.InputSystem?.RegisterHandler<IMixedRealityPointerHandler>(this);
            
            // turn off colliders under GameObject?

            IsBeingPlaced = true;
        }

        public void StopPlacement()
        {
            SolverHandler.UpdateSolvers = false;

            CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySpeechHandler>(this);
            CoreServices.InputSystem?.UnregisterHandler<IMixedRealityPointerHandler>(this);

            // turn on colliders under GameObject? Need to save set
            IsBeingPlaced = false;
        }

        /// <inheritdoc/>
        public override void SolverUpdate()
        {
            PerformRaycast();
            SetPosition();
            SetRotation();
        }

        protected virtual void PerformRaycast()
        {
            if (SolverHandler.TransformTarget != null)
            {
                var transform = SolverHandler.TransformTarget;

                Vector3 origin = transform.position;
                Vector3 endpoint = transform.position + transform.forward;
                currentRay.UpdateRayStep(ref origin, ref endpoint);

                didHit = MixedRealityRaycaster.RaycastSimplePhysicsStep(currentRay, MaxRaycastDistance, MagneticSurfaces, false, out currentHit);
            }
        }

        protected virtual void SetPosition()
        {
            if (didHit)
            {
                GoalPosition = currentHit.point;
            }
            else
            {
                if (SolverHandler.TransformTarget != null)
                {
                    GoalPosition = SolverHandler.TransformTarget.position + SolverHandler.TransformTarget.forward * DefaultPlacementDistance;
                }
            }
        }

        protected virtual void SetRotation()
        {
            /*
            if (KeepOrientationVertical)
            {
                direction.y = 0;
                currentHit.normal.y = 0;
            }*/

            GoalRotation = Quaternion.LookRotation(-currentRay.Direction, Vector3.up);
        }

        #region IMixedRealityPointerHandler

        /// <inheritdoc/>
        public void OnPointerDown(MixedRealityPointerEventData eventData) { }
        /// <inheritdoc/>
        public void OnPointerDragged(MixedRealityPointerEventData eventData) { }
        /// <inheritdoc/>
        public void OnPointerUp(MixedRealityPointerEventData eventData) { }
        
        /// <inheritdoc/>
        public void OnPointerClicked(MixedRealityPointerEventData eventData)
        {
            StopPlacement();
        }

        #endregion

        #region IMixedRealitySpeechHandler

        /// <inheritdoc/>
        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (enabled && IsBeingPlaced && Keywords.Contains(eventData.Command.Keyword.ToLower()))
            {
                StopPlacement();
            }
        }

        #endregion
    }
}
