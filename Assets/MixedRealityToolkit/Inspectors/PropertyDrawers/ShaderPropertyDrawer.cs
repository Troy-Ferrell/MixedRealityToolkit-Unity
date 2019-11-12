// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Rendering;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Draws the scene info struct and populates its hidden fields.
    /// </summary>
    [CustomPropertyDrawer(typeof(ShaderProperty))]
    internal class ShaderPropertyDrawer : PropertyDrawer
    {
        private static readonly GUIContent ValueLabel = new GUIContent("Value", "Shader Property value for given type");
        private static readonly GUIContent PropertyNameLabel = new GUIContent("Property Name", "Key Name of the Shader Property in shader code");

        private const int NUM_OF_FIELDS_RENDERED = 3;

        private static readonly Dictionary<ShaderProperty.PropertyType, string> typeToSerializedFieldName = new Dictionary<ShaderProperty.PropertyType, string>()
        {
            { ShaderProperty.PropertyType.Bool, "boolValue" },
            { ShaderProperty.PropertyType.Float, "floatValue" },
            { ShaderProperty.PropertyType.Color, "colorValue" },
            { ShaderProperty.PropertyType.Texture, "textureValue" },
        };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight) * NUM_OF_FIELDS_RENDERED;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var propertyNameRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            var typeRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            var unitRect = new Rect(position.x, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(propertyNameRect, property.FindPropertyRelative("propertyName"), PropertyNameLabel);

            var typeProperty = property.FindPropertyRelative("type");
            EditorGUI.PropertyField(typeRect, typeProperty);
            
            var type = (ShaderProperty.PropertyType)typeProperty.enumValueIndex;
            var valueProperty = property.FindPropertyRelative(typeToSerializedFieldName[type]);
            EditorGUI.PropertyField(unitRect, valueProperty, ValueLabel);

            EditorGUI.EndProperty();
        }

    }
}
