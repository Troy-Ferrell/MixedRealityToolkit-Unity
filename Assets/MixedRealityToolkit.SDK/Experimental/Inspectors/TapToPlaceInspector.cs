// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Editor.Solvers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Experimental.Inspectors
{
    [CustomEditor(typeof(TapToPlace))]
    public class TapToPlaceInspector : SolverInspector
    {
        private TapToPlace tapToPlace;
        protected override void OnEnable()
        {
            base.OnEnable();

            // TODO: insert properties

            tapToPlace = target as TapToPlace;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            // TODO: insert properties

            serializedObject.ApplyModifiedProperties();
        }
    }
}
