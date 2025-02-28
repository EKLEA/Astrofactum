using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [HideInInspector] public CachedPlanet CachedPlanet;
    [HideInInspector] public MeshFilter[] meshFilters;
    [HideInInspector] public TerrainFace[] terrainFaces;
    [HideInInspector] public Vector3 position;
    [HideInInspector] public Transform player;
    [HideInInspector] public Vector3 PlayerPos;
    [HideInInspector] public float distanceToPlayer;
    [HideInInspector] public float distanceToPlayerPow2;
    public bool autoUpdate = false;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;

    public EarthShape shapeConfig;
    public float Size = 10000;
    public bool CullingEnabled = true;
    public float CullingMinAngle = 1.45f;
    public Material surfaceMat;

    public float[] detailLevelDistances = new float[] {
        Mathf.Infinity, 3000f, 1100f, 500f, 210f, 100f, 40f,
    };

    private int printed;
    public List<Action> ActionQueue = new List<Action>(); // Use Queue datatype instead of List?
    public object _asyncLock = new object();

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
    }

    private void Start()
    {
        if(terrainFaces == null || terrainFaces.Length == 0)
        {
            Initialize();
            LoadCachedPlanet(CachedPlanet);
        }
        GenerateMesh(false); // Is this needed?
        StartCoroutine(PlanetGenerationLoop());
    }

    public void Update()
    {
        ExecuteActionQueue();
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
        distanceToPlayerPow2 = distanceToPlayer * distanceToPlayer;
        position = transform.position;
        PlayerPos = player.transform.position;
    }

    private void ExecuteActionQueue()
    {
        if (ActionQueue != null)
        {
            lock (_asyncLock)
            {
                List<Action> actionsToDelete = new List<Action>();
                List<Action> actionQueue = new List<Action>(ActionQueue);
                foreach (Action action in actionQueue)
                {
                    if (action != null)
                    {
                        action.Invoke();
                        actionsToDelete.Add(action);
                    }
                }
                foreach (Action action in actionsToDelete)
                {
                    if (action != null)
                    {
                        actionQueue.Remove(action);
                    }
                }
                ActionQueue = actionQueue;
            }
        }
    }

    private IEnumerator PlanetGenerationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(5.5f);
            GenerateMesh(true);
        }
    }

    public void Initialize()
    {

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        // Connect existing faces to the mesh filters (This is a code smell)
        Transform[] faces = transform.GetChildrenWithTag("PlanetFace");
        for(int i = 0; i < faces.Length; i++) {
            meshFilters[i] = faces[i].GetComponent<MeshFilter>();
        }

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                // Create new game objects
                GameObject meshObject = new GameObject("mesh");
                meshObject.transform.parent = transform;
                meshObject.transform.position = transform.position;
                meshObject.tag = "PlanetFace";
                meshObject.layer = LayerMask.NameToLayer("Planet");

                meshObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(surfaceMat);
                meshFilters[i] = meshObject.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
                meshFilters[i].sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Allow more vertices
            } else {
                // Update old game objects
                if(meshFilters[i].GetComponent<MeshRenderer>() == null) {
                    meshFilters[i].gameObject.AddComponent<MeshRenderer>();
                }
                meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = new Material(surfaceMat);
                meshFilters[i].gameObject.layer = LayerMask.NameToLayer("Planet");
            }

            terrainFaces[i] = new TerrainFace(meshFilters[i].sharedMesh, directions[i], Size, this);
        }
    }

    /*public void GeneratePlanet(){
        Debug.Log("Generating planet...");
        Initialize();
        GenerateMesh();
        UpdateMesh();
    }*/

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
                //planet.GenerateTexture();
                //planet.UpdateShaders();
            CachedPlanet = CachePlanet();
        }
    }

    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            //Initialize();
            //GenerateColors();
        }
    }

    public void GenerateMesh(bool multithread = false)
    {
        foreach (TerrainFace face in terrainFaces)
        {
            if(multithread) {
                Thread t = new Thread(() => { 
                    face.GenerateTree(true);
                });
                t.Start();
            } else {
                face.GenerateTree();
            }
        }
    }

    public CachedPlanet CachePlanet () {
        CachedFace[] cachedFaces = new CachedFace[6];
        for(int i = 0; i < 6; i++) {
            cachedFaces[i] = new CachedFace(terrainFaces[i].mesh);
        }
        return new CachedPlanet(cachedFaces);
    }

    /// <summary>
    /// Load the mesh and texture.
    /// </summary>
    private void LoadCachedPlanet(CachedPlanet cachedPlanet)
    {
        for (int i = 0; i < 6; i++)
        {
            if (cachedPlanet != null)
            {
                terrainFaces[i].mesh = cachedPlanet.cachedFaces[i].mesh;
                //terrainFaces[i].texture = cachedPlanet.cachedFaces[i].texture;
            }
        }
    }


    /*void UpdateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.UpdateTree();
        }
    }*/
}