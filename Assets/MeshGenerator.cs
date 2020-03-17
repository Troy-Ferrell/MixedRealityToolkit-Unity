using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

public class MeshGenerator : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            MakeMesh();
        }
    }

    private void MakeMesh()
    {
        var hand = new GameObject("HandMesh");
        var mesh = hand.AddComponent<MeshFilter>().mesh;
        hand.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

        //var gesturePose = SimulatedArticulatedHandPoses.GetGesturePose(ArticulatedHandPose.GestureId.Flat);
        //var jointPose = gesturePose.GetLocalJointPose(TrackedHandJoint.IndexDistalJoint, Handedness.Right);

        List<Vector3> vertices = new List<Vector3>();
        List<int> indices = new List<int>();

        // parts going around hand, then each finger/thumb

        List<List<TrackedHandJoint>> fingers = new List<List<TrackedHandJoint>>()
        {
            new List<TrackedHandJoint> { TrackedHandJoint.ThumbMetacarpalJoint, TrackedHandJoint.ThumbProximalJoint, TrackedHandJoint.ThumbDistalJoint, TrackedHandJoint.ThumbTip },
            new List<TrackedHandJoint> { TrackedHandJoint.IndexMetacarpal, TrackedHandJoint.IndexKnuckle, TrackedHandJoint.IndexMiddleJoint, TrackedHandJoint.IndexDistalJoint, TrackedHandJoint.IndexTip },
            new List<TrackedHandJoint> { TrackedHandJoint.MiddleMetacarpal, TrackedHandJoint.MiddleKnuckle, TrackedHandJoint.MiddleMiddleJoint, TrackedHandJoint.MiddleDistalJoint, TrackedHandJoint.MiddleTip },
            new List<TrackedHandJoint> { TrackedHandJoint.RingMetacarpal, TrackedHandJoint.RingKnuckle, TrackedHandJoint.RingMiddleJoint, TrackedHandJoint.RingDistalJoint, TrackedHandJoint.RingTip },
            new List<TrackedHandJoint> { TrackedHandJoint.PinkyMetacarpal, TrackedHandJoint.PinkyKnuckle, TrackedHandJoint.PinkyMiddleJoint, TrackedHandJoint.PinkyDistalJoint, TrackedHandJoint.PinkyTip },
        };
        

        const int JOINT_VERT_SIZE = 10;
        int vertexCountOffset = 0;

        foreach (var joints in fingers)
        {
            for (int i = 0; i < joints.Count; i++)
            {
                HandJointUtils.TryGetJointPose(joints[i], Handedness.Right, out MixedRealityPose jointPose);

                float radius = Mathf.Lerp(0.007f, 0.002f, (float)i / joints.Count);

                for (int j = 0; j < JOINT_VERT_SIZE; j++)
                {
                    // sin circle of vectors
                    float angle = -((float)j / JOINT_VERT_SIZE) * 2.0f * Mathf.PI;

                    var offset = radius * (Mathf.Sin(angle) * jointPose.Up + Mathf.Cos(angle) * jointPose.Right);

                    vertices.Add(jointPose.Position + offset);
                }

                if (i != 0)
                {
                    for (int c = 0; c < JOINT_VERT_SIZE; c++)
                    {
                        int currStart = i * JOINT_VERT_SIZE + vertexCountOffset;
                        int prevStart = (i - 1) * JOINT_VERT_SIZE + vertexCountOffset;

                        int wrap = (c + 1) % JOINT_VERT_SIZE;

                        indices.AddRange(MakeSquare(
                            prevStart + c,
                            prevStart + wrap,
                            currStart + c,
                            currStart + wrap
                            ));

                        //indices.Add(prevStart + c); indices.Add(currStart + c); indices.Add(prevStart + wrap);
                        //indices.Add(prevStart + wrap); indices.Add(currStart + c); indices.Add(currStart + wrap);
                    }
                }
            }

            vertexCountOffset = vertices.Count;
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        mesh.RecalculateNormals();
    }

    private int[] MakeSquare(int p0, int p1, int p2, int p3)
    {
        return new int[]
        {
            p0, p2, p1, // triangle 1 
            p1, p2, p3  // triangle 2
        };
        //indices.Add(prevStart + c); indices.Add(currStart + c); indices.Add(prevStart + wrap);
        //indices.Add(prevStart + wrap); indices.Add(currStart + c); indices.Add(currStart + wrap);
    }
}
