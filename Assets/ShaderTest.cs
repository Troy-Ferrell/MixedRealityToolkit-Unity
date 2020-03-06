using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTest : MonoBehaviour
{
    public GameObject prefab;
    public Material shader;
    public int count = 100;
    public float size = 10f;

    private List<GameObject> gameObjects = new List<GameObject>();
    private Vector3[] rotations;
    private float angleRotationRate = Mathf.PI / 5.0f;

    private void Start()
    {
        rotations = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            var go = GameObject.Instantiate(prefab, this.transform);
            go.transform.localPosition = new Vector3(
                                            Random.Range(-size, size),
                                            Random.Range(-size, size),
                                            Random.Range(-size, size));

            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var r in renderers)
                r.material = new Material(shader);

            gameObjects.Add(go);

            rotations[i] = Random.insideUnitSphere;
        }
    }

    public void Update()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].transform.Rotate(rotations[i], angleRotationRate);
        }
    }

}
