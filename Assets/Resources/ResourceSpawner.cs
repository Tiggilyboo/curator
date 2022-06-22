using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class ResourceSpawnItem
{
    [SerializeField]
    private GameObject m_Prefab;
    [SerializeField]
    private Collider m_Collider;
    [SerializeField]
    private int m_TreeProtoIndex;
    [SerializeField]
    private bool m_IsTree;

    public GameObject Prefab => m_Prefab;
    public Collider Collider => m_Collider;
    public int? TreeProtoIndex => m_IsTree 
      ? (int?)m_TreeProtoIndex 
      : (int?)null;
}

public class ResourceSpawner : MonoBehaviour
{
    private List<GameObject> m_SpawnedObjects;
    private float m_LastSpawnTime;
    private float m_LastCleanedTime;

    [SerializeField]
    private List<ResourceSpawnItem> m_ResourcePrefabs;

    [SerializeField]
    private Terrain m_Terrain;

    [SerializeField]
    private float m_SecondsToSpawnAPrefab = 3f;
    [SerializeField]
    private float m_SecondsToCleanSpawnedObjects = 10f;

    private void CheckSpawnedObjects()
    {
        for(int i = 0; i < m_SpawnedObjects.Count; i++)
        {
            if(m_SpawnedObjects[i] != null)
              continue;

            m_SpawnedObjects.RemoveAt(i);
        }

        m_LastCleanedTime = Time.realtimeSinceStartup;
    }

    private bool GetSpawnableLocationFor(GameObject prefab, Collider prefabCollider, out Vector3 spawnLocation)
    {
        TerrainData terrainData = m_Terrain.terrainData;
        Bounds itemBounds = prefabCollider.bounds;
        Vector3 colliderExtents = itemBounds.extents;

        // TODO: Attempt count? 
        //  Or be more calculated when we get a collision, calculate a place around that collided bounds?
        for(int i = 0; i < 10; i++)
        {
            float halfSize = terrainData.heightmapResolution * 0.5f;
            Vector2 randomPoint = Random.insideUnitCircle * terrainData.heightmapResolution;
            randomPoint.x += halfSize;
            randomPoint.y += halfSize;
            spawnLocation = new Vector3(randomPoint.x, 100f, randomPoint.y);
            spawnLocation += transform.position;

            // Boxcast the prefab colliders extents downwards at this point to see if we have an overlap
            RaycastHit[] hits = Physics.BoxCastAll(spawnLocation, itemBounds.extents, -Vector3.up);
            RaycastHit? terrainHit = null;
            bool othersAboveTerrain = false;
            float terrainDistance = 200f;

            // TODO: efficiency...

            // Find terrain
            foreach(RaycastHit hit in hits)
            {
                if(hit.collider is TerrainCollider)
                {
                    terrainHit = (RaycastHit?)hit;
                    terrainDistance = hit.distance;
                    break;
                }
            }
            if(terrainHit == null)
                continue;

            foreach(RaycastHit hit in hits)
            {
                if(hit.distance < terrainDistance)
                {
                    othersAboveTerrain = true;
                    break;
                }
            }

            if(othersAboveTerrain == false)
                return true;
        }

        // Unable to find a spot to spawn the object.
        spawnLocation = Vector3.zero;
        return false;
    }

    public void Spawn()
    {
        foreach(ResourceSpawnItem item in m_ResourcePrefabs)
            Spawn(item.Prefab, item.Collider, item.TreeProtoIndex, out _);
          
        m_LastSpawnTime = Time.realtimeSinceStartup;
    }

    public bool Spawn(GameObject prefab, Collider prefabCollider, int? treePrototypeIdx, out GameObject spawnedObject)
    {
        if(GetSpawnableLocationFor(prefab, prefabCollider, out Vector3 spawnLocation))
        {
            if(treePrototypeIdx.HasValue)
            {
                TerrainData terrainData = m_Terrain.terrainData;
                TreeInstance tree = new TreeInstance();
                tree.position = new Vector3(
                    spawnLocation.x / terrainData.heightmapResolution, 
                    0f, 
                    spawnLocation.z / terrainData.heightmapResolution);
                tree.prototypeIndex = treePrototypeIdx.Value;
                tree.widthScale = 1f;
                tree.heightScale = 1f;
                tree.color = Color.white;
                tree.lightmapColor = Color.white;

                m_Terrain.AddTreeInstance(tree);
                spawnedObject = terrainData.treePrototypes[treePrototypeIdx.Value].prefab;
            }
            else
            {
                spawnedObject = Instantiate(prefab, m_Terrain.transform);
                spawnedObject.transform.position = spawnLocation;
            }
            
            m_SpawnedObjects.Add(spawnedObject);

            return true;
        }

        spawnedObject = null;
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_SpawnedObjects = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {    
        if(Time.realtimeSinceStartup > m_LastSpawnTime + m_SecondsToSpawnAPrefab)
        {
            Spawn();
            m_Terrain.Flush();
            return;
        }
        if(Time.realtimeSinceStartup > m_LastCleanedTime + m_SecondsToCleanSpawnedObjects)
        {
            CheckSpawnedObjects();
            return;
        }

    }
}
