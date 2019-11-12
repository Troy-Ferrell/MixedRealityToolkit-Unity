// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Rendering
{
    /// <summary>
    /// TODO: Troy
    /// </summary>
    [Serializable]
    public class ShaderProperty
    {
        public enum PropertyType
        {
            Bool,
            Float,
            Color,
            Texture
        }

        #region Class Members

        [SerializeField]
        private PropertyType type;
        public PropertyType Type
        {
            get => type;
            private set => type = value;
        }

        [NonSerialized]
        private int propertyId = -1;
        private int PropertyId
        {
            get => propertyId == -1 ? Shader.PropertyToID(PropertyName) : propertyId;
            set => propertyId = value;
        }

        [SerializeField]
        private string propertyName;
        public string PropertyName
        {
            get => propertyName;
            private set
            {
                propertyName = value;
                PropertyId = Shader.PropertyToID(value);
            }
        }

        [SerializeField]
        private bool boolValue;
        public bool BoolValue
        {
            get => boolValue;
            private set => boolValue = value;
        }

        [SerializeField]
        private float floatValue;
        public float FloatValue
        {
            get => floatValue;
            private set => floatValue = value;
        }

        [SerializeField]
        private Color colorValue;
        public Color ColorValue
        {
            get => colorValue;
            private set => colorValue = value;
        }

        [SerializeField]
        private Texture textureValue;
        public Texture TextureValue
        {
            get => textureValue;
            private set => textureValue = value;
        }

        #endregion  

        private static Dictionary<System.Type, PropertyType> mappings = new Dictionary<Type, PropertyType>()
        {
            { typeof(bool), PropertyType.Bool },
            { typeof(float), PropertyType.Float },
            { typeof(Color), PropertyType.Color },
            { typeof(Texture), PropertyType.Texture },
            { typeof(Texture2D), PropertyType.Texture },
            { typeof(Texture3D), PropertyType.Texture },
        };

        private static Dictionary<PropertyType, MaterialPropertyRuntime> runtimes = new Dictionary<PropertyType, MaterialPropertyRuntime>()
        {
            { PropertyType.Bool, new BoolMaterialRuntime() },
            { PropertyType.Float, new FloatMaterialRuntime() },
            { PropertyType.Color, new ColorMaterialRuntime() },
            { PropertyType.Texture, new TextureMaterialRuntime() },
        };

        public void SetProperty<T>(string propertyName, T propertyValue)
        {
            var systemType = typeof(T);
            if (mappings.ContainsKey(systemType))
            {
                Type = mappings[systemType];
                PropertyName = propertyName;
                runtimes[Type].SetProperty(this, propertyValue);
            }
            else
            {
                // TODO: log warning
            }
        }

        public void ApplyProperty(MaterialPropertyBlock block)
        {
            runtimes[Type].ApplyProperty(this, block);
        }

        #region Property Factories

        private abstract class MaterialPropertyRuntime
        {
            public abstract void SetProperty(ShaderProperty property, object data);
            public abstract void ApplyProperty(ShaderProperty property, MaterialPropertyBlock block);
        }

        private class BoolMaterialRuntime : MaterialPropertyRuntime
        {
            public override void SetProperty(ShaderProperty property, object data) 
                => property.boolValue = (bool)data;
            public override void ApplyProperty(ShaderProperty property, MaterialPropertyBlock block)
                => block?.SetFloat(property.PropertyId, Convert.ToSingle(property.boolValue));
        }

        private class FloatMaterialRuntime : MaterialPropertyRuntime
        {
            public override void SetProperty(ShaderProperty property, object data)
                => property.boolValue = (bool)data;
            public override void ApplyProperty(ShaderProperty property, MaterialPropertyBlock block)
                => block?.SetFloat(property.PropertyId, property.floatValue);
        }

        private class ColorMaterialRuntime : MaterialPropertyRuntime
        {
            public override void SetProperty(ShaderProperty property, object data)
                => property.colorValue = (Color)data;
            public override void ApplyProperty(ShaderProperty property, MaterialPropertyBlock block)
                => block?.SetColor(property.PropertyId, property.colorValue);
        }

        private class TextureMaterialRuntime : MaterialPropertyRuntime
        {
            public override void SetProperty(ShaderProperty property, object data)
                => property.textureValue = (Texture)data;
            public override void ApplyProperty(ShaderProperty property, MaterialPropertyBlock block)
                => block?.SetTexture(property.PropertyId, property.textureValue);
        }
        #endregion
    }
}