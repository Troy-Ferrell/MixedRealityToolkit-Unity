using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using UnityEngine;

public class ClippingBoxArray : ClippingBox
{
    [SerializeField]
    private BoxCollider[] colliders = null;

    private const int ClippingBoxSize = 4;

    private static Vector4[] clipBoxSizeArray = new Vector4[ClippingBoxSize];

    private static Matrix4x4[] clipBoxInverseTransformArray = new Matrix4x4[ClippingBoxSize];

    private int clipBoxSizeArrayID;
    private int clipBoxInverseTransformArrayID;

    protected override void Initialize()
    {
        base.Initialize();

        clipBoxSizeArrayID = Shader.PropertyToID("_ClipBoxSize");
        clipBoxInverseTransformArrayID = Shader.PropertyToID("_ClipBoxInverseTransform");
    }

    private new void Update()
    {
        if (colliders == null) return;

        Debug.Assert(colliders.Length <= ClippingBoxSize);

        for (int i = 0; i < ClippingBoxSize; i++)
        {
            int idx = Math.Min(i, colliders.Length - 1);
            var current = colliders[idx];

            if (current != null)
            {
                Vector3 lossyScale = current.transform.lossyScale * 0.5f;
                clipBoxSizeArray[i] = new Vector4(lossyScale.x, lossyScale.y, lossyScale.z, 0.0f);
                clipBoxInverseTransformArray[i] = Matrix4x4.TRS(current.transform.position, current.transform.rotation, Vector3.one).inverse;
            }
        }
    }

    protected override void UpdateShaderProperties(MaterialPropertyBlock materialPropertyBlock)
    {
        materialPropertyBlock.SetVectorArray(clipBoxSizeArrayID, clipBoxSizeArray);
        materialPropertyBlock.SetMatrixArray(clipBoxInverseTransformArrayID, clipBoxInverseTransformArray);
    }
}
