using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class TerrainFace
{
    public volatile Mesh mesh;
    public Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;
    float radius;
    public Chunk parentChunk;
    public Planet planetScript;
    public List<Chunk> visibleChildren = new List<Chunk>();

    // These will be filled with the generated data
    public List<Vector3> vertices = new List<Vector3>();
    public List<Vector3> borderVertices = new List<Vector3>();
    public List<Vector3> normals = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<int> borderTriangles = new List<int>();
    public Dictionary<int, bool> edgefanIndex = new Dictionary<int, bool>();

    // Constructor
    public TerrainFace(Mesh mesh, Vector3 localUp, float radius, Planet planetScript)
    {
        this.mesh = mesh;
        this.localUp = localUp;
        this.radius = radius;
        this.planetScript = planetScript;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisA.RoundToWorldAxis();
        axisB = Vector3.Cross(localUp, axisA);
        axisB.RoundToWorldAxis();
    }

    // Construct a quadtree of chunks (even though the chunks end up 3D, they start out 2D in the quadtree and are later projected onto a sphere)
    public void GenerateTree(bool multithread = false)
    {
        // Resets the mesh
        vertices.Clear();
        triangles.Clear();
        normals.Clear();
        borderVertices.Clear();
        borderTriangles.Clear();
        visibleChildren.Clear();
        edgefanIndex.Clear();

         // Extend the resolution capabilities of the mesh

        // Generate chunks
        if(parentChunk == null) {
            parentChunk = new Chunk(1, this, (DVector3) localUp.normalized * planetScript.Size, radius, 0, (DVector3) localUp, (DVector3) axisA, (DVector3) axisB, 0);
            parentChunk.GenerateChildren();
        } else {
            parentChunk.UpdateChunk();
        }

        int triangleOffset = 0;
        int borderTriangleOffset = 0;
        parentChunk.GetVisibleChildren();
        foreach (Chunk child in visibleChildren)
        {
            Vector3[] newVertices;
            int[] newTriangles;
            int[] newBorderTriangles;
            Vector3[] newBorderVertices;
            Vector3[] newNormals;
            Vector2[] newUVs;
            Color[] newColors;
            child.GetNeighbourLOD();

            if (child.vertices == null)
            {
                child.CalculateMeshProperties(triangleOffset, borderTriangleOffset, 
                out newVertices, out newTriangles, out newBorderTriangles, out newBorderVertices, out newNormals, out newUVs, out newColors);
            }
            else if (child.vertices.Length == 0 || child.triangles != Presets.quadTemplateTriangles[child.neighbours.AsBinarySequence(4)])
            {
                child.CalculateMeshProperties(triangleOffset, borderTriangleOffset, 
                out newVertices, out newTriangles, out newBorderTriangles, out newBorderVertices, out newNormals, out newUVs, out newColors);
            }
            else
            {
                newVertices = child.vertices;
                newTriangles = child.GetTrianglesWithOffset(triangleOffset);
                newBorderTriangles = child.GetBorderTrianglesWithOffset(borderTriangleOffset, triangleOffset);
                newBorderVertices = child.borderVertices;
                newNormals = child.normals;
            }

            vertices.AddRange(newVertices);
            triangles.AddRange(newTriangles);
            borderTriangles.AddRange(newBorderTriangles);
            borderVertices.AddRange(newBorderVertices);
            normals.AddRange(newNormals);

            // Increase offset to accurately point to the next slot in the lists
            triangleOffset += (Presets.quadRes + 1) * (Presets.quadRes + 1);
            borderTriangleOffset += newBorderVertices.Length;
        }

        if(multithread) {
            lock(planetScript._asyncLock) {
                planetScript.ActionQueue.Add(() => {
                    UpdateMesh(mesh, vertices.ToArray(), triangles.ToArray(), normals.ToArray());
                });
            }
        } else {
            lock(planetScript._asyncLock) {
                UpdateMesh(mesh, vertices.ToArray(), triangles.ToArray(), normals.ToArray());
            }
        }
    }

    void UpdateMesh(Mesh mesh, Vector3[] vertices, int[] triangles, Vector3[] normals) {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        //mesh.uv = uvs;
        //mesh.colors = colors;
        mesh.RecalculateBounds();
    }

    public static float GetElevation(EarthShape config, Vector3 pointOnUnitSphere, int seed = 0) {
        return config.CalculateHeight(pointOnUnitSphere, seed);
    }

    public static double GetElevationD(TerrainConfig config, Vector3 pointOnUnitSphere) {
        double elevation = 0;

        for(int i = 0; i < config.NoiseFilters.Length; i++) {
            elevation += config.NoiseFilters[i].EvaluateD(pointOnUnitSphere);
        }

        return elevation;
    }

    /// <summary>
    /// Get the maximum elevation value.
    /// </summary>
    public static float GetMaxHeight(TerrainConfig config) {
        float maxHeight = 0;

        for(int i = 0; i < config.NoiseFilters.Length; i++) {
            NoiseFilter noiseFilter = config.NoiseFilters[i];
            if(noiseFilter.clampingEnabled) {
                maxHeight += noiseFilter.strength * Mathf.Clamp(noiseFilter.multiplier, noiseFilter.minValue, noiseFilter.maxValue);
            } else {
                maxHeight += noiseFilter.strength;
            }
        }

        return maxHeight;
    }
}