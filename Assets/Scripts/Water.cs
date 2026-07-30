//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class NoiseCube : MonoBehaviour
{
    [Header("Wobble")]
    public float wobbleAmount =0.1f;
    public float wobbleRate=1f;
    public float noiseScale =1f;

    Mesh mesh;
    Vector3[] originalVertices;
    Vector3[] vertices;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices =mesh.vertices;
        vertices =new Vector3[originalVertices.Length];
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time*wobbleRate;
        for(int i=0; i<originalVertices.Length; i++)
        {
            Vector3 v = originalVertices[i];

            float x =(Mathf.PerlinNoise(v.y*noiseScale +time, v.z*noiseScale)-0.5f)*2f;
            float y =(Mathf.PerlinNoise(v.z*noiseScale +time, v.x*noiseScale)-0.5f)*2f;
            float z =(Mathf.PerlinNoise(v.x*noiseScale +time, v.y*noiseScale)-0.5f)*2f;

            vertices[i] = v+ new Vector3(x,y,z)*wobbleAmount;
        }
        mesh.vertices =vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}
