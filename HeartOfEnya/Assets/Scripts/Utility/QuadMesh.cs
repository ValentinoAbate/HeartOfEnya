using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(SetRendererLayer))]
public class QuadMesh : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MaterialPropertyBlock propertyBlock;
    public MaterialPropertyBlock PropertyBlock 
    { 
        get => propertyBlock;
        set
        {
            propertyBlock = value;
            meshRenderer.SetPropertyBlock(value);
        } 
    }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        // Initialize property block cache
        propertyBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(propertyBlock);
        meshFilter = GetComponent<MeshFilter>();  
    }

    public void SetMesh(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        var mesh = new Mesh();

        Vector2[] uv = new Vector2[]
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(0, 0),
        };

        int[] triangles = new int[]
        {
            0, 1, 2,
            2, 1, 3,
        };

        mesh.vertices = new Vector3[] { v1, v2, v3, v4 };
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    public void SetMaterial(Material mat)
    {
        meshRenderer.material = mat;
    }
}
