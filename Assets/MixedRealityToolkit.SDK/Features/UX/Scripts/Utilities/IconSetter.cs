// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Rendering;
using UnityEngine;

/// <summary>
/// TODO: Troy
/// </summary>
public class IconSetter : MonoBehaviour
{
    public Texture Texture;

    private void Awake()
    {
        if (Texture != null)
        {
            var shaderProperty = new ShaderProperty();
            shaderProperty.SetProperty<Texture>("_MainTex", Texture);
            gameObject.SetMaterialProperties<MeshRenderer>(new []{ shaderProperty });
        }
    }
}
