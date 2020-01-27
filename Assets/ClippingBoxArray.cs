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
    private int lastFrameID = 0;

    /// <inheritdoc />
    protected override string Keyword
    {
        get => "_CLIPPING_BOX_ARRAY";
    }

    /// <inheritdoc />
    protected override void Initialize()
    {
        base.Initialize();

        clipBoxSizeArrayID = Shader.PropertyToID("_ClipBoxSizeArray");
        clipBoxInverseTransformArrayID = Shader.PropertyToID("_ClipBoxInverseTransformArray");
    }

    private void UpdateData()
    {
        if (lastFrameID == Time.frameCount)
            return;

        lastFrameID = Time.frameCount;

        if (colliders == null) return;

        if (colliders.Length > ClippingBoxSize)
        {
            Debug.LogError($"Colliders property on ClippingBoxArray is larger than supported size of {ClippingBoxSize}");
        }

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
        UpdateData();
        materialPropertyBlock.SetVectorArray(clipBoxSizeArrayID, clipBoxSizeArray);
        materialPropertyBlock.SetMatrixArray(clipBoxInverseTransformArrayID, clipBoxInverseTransformArray);
    }
}
