using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public Transform[] spawnPoints;
    void Awake()
    {
        // Find all GameObjects tagged as "SpawnPoint" and assign them to the spawnPoints array
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("SpawnPoint");
        spawnPoints = new Transform[spawnPointObjects.Length];
        for (int i = 0; i < spawnPointObjects.Length; i++)
        {
            spawnPoints[i] = spawnPointObjects[i].transform;
        }
    }
    void Start()
    {
        SpawnNPC();
    }

    public void SpawnNPC()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    // Example to spawn an NPC at a random spawn point
    public void SpawnNPCAtRandomPoint()
    {
        int index = Random.Range(0, spawnPoints.Length);
        Instantiate(npcPrefab, spawnPoints[index].position, spawnPoints[index].rotation);
    }
}

