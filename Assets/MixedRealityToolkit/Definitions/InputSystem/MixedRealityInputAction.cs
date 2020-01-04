// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// An Input Action for mapping an action to an Input Sources Button, Joystick, Sensor, etc.
    /// </summary>
    [Serializable]
    public struct MixedRealityInputAction : IEqualityComparer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public MixedRealityInputAction(string action, AxisType axisConstraint = AxisType.None)
        {
            this.action = action;
            this.axisConstraint = axisConstraint;

#pragma warning disable 0618
            // Fill obsolete variables to satifsy compiler
            id = UInt32.MaxValue;
            description = action;
#pragma warning restore 0618
        }

        public static MixedRealityInputAction None { get; } = new MixedRealityInputAction("None", AxisType.None);

        [SerializeField]
        [FormerlySerializedAs("description")]
        private string action;

        /// <summary>
        /// The name and key for this input action. Acts as unique identifier
        /// </summary>
        public string Action => action;

        /// <summary>
        /// The Axis constraint for the Input Action
        /// </summary>
        public AxisType AxisConstraint => axisConstraint;

        [SerializeField]
        private AxisType axisConstraint;

        public static bool operator ==(MixedRealityInputAction left, MixedRealityInputAction right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(MixedRealityInputAction left, MixedRealityInputAction right)
        {
            return !left.Equals(right);
        }

        #region IEqualityComparer Implementation

        bool IEqualityComparer.Equals(object left, object right)
        {
            if (ReferenceEquals(null, left) || ReferenceEquals(null, right)
                || !(left is MixedRealityInputAction) || !(right is MixedRealityInputAction))
            {
                return false;
            }

            return ((MixedRealityInputAction)left).Equals((MixedRealityInputAction)right);
        }

        public bool Equals(MixedRealityInputAction other)
        {
            return string.Equals(Action, other.action, StringComparison.CurrentCultureIgnoreCase) &&
                   AxisConstraint == other.AxisConstraint;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is MixedRealityInputAction && Equals((MixedRealityInputAction)obj);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj is MixedRealityInputAction ? ((MixedRealityInputAction)obj).GetHashCode() : 0;
        }

        public override int GetHashCode()
        {
            return $"{Action}.{AxisConstraint}".GetHashCode();
        }

        #endregion IEqualityComparer Implementation

        #region Obsolete

        /// <summary>
        /// Constructor.
        /// </summary>
        [Obsolete("Use alternate constructor without id")]
        public MixedRealityInputAction(uint id, string description, AxisType axisConstraint = AxisType.None)
        {
            this.id = id;
            action = this.description = description;
            this.axisConstraint = axisConstraint;
        }

        /// <summary>
        /// The Unique Id of this Input Action.
        /// </summary>
        [Obsolete("Use Action property instead as unique identifer")]
        public uint Id => id;

        [SerializeField]
        [Obsolete("Use Action property instead as unique identifer")]
        private uint id;

        [SerializeField]
        [Obsolete("Use Action property instead")]
        private string description;

        /// <summary>
        /// A short description of the Input Action.
        /// </summary>
        [Obsolete("Use Action property instead")]
        public string Description => description;

        #endregion
    }
}
