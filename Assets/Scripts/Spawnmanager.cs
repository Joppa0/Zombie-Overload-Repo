using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefab;
    public GameObject[] powerupPrefab;

    private int enemyCount;
    private int waveNumber = 2;

    [SerializeField] private float enemySpawnRangeX = 20.0f;
    [SerializeField] private float enemySpawnRangeZ = 7.5f;
    [SerializeField] private float prefabSpawnRangeX = 12f;
    [SerializeField] private float prefabSpawnRangeZ = 4f;

    public LayerMask m_LayerMask;
    public Camera cam;
 
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        SpawnEnemyWave(waveNumber);
    }

    // Update is called once per frame
    void Update()
    {
        //Spawns a new wave and increments the wave number if there are no enemies left
        enemyCount = FindObjectsOfType<EnemyController>().Length;
        if (enemyCount == 1)
        {
            waveNumber++;
            SpawnEnemyWave(waveNumber);
        }
    }

    void SpawnEnemyWave(int enemiesToSpawn)
    {
        //Spawns random new enemies with random positions
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 placeToSpawn = GenerateEnemySpawnPosition();

            int randNumber = Random.Range(0, 3);

            //Creates a box to check for collisions when trying to spawn
            Collider[] collisionWithEnemy = Physics.OverlapBox(placeToSpawn, enemyPrefab[randNumber].transform.localScale, Quaternion.identity, m_LayerMask);
            
            if (collisionWithEnemy.Length > 1)
            {
                //If there are collisions other than the ground, spawn a new enemy with a new spawnpoint
                enemiesToSpawn++;
            }
            
            else
            {
                //If there are no other collisions, spawn at the chosen spawnpoint
                Instantiate(enemyPrefab[randNumber], placeToSpawn, enemyPrefab[randNumber].transform.rotation);
            }
        }

        //Instantiates powerup if none remain at the start of a new wave
        if (GameObject.FindObjectsOfType<PowerUp>().Length == 0)
        {
            Instantiate(powerupPrefab[Random.Range(0, 3)], GeneratePrefabSpawnPosition(), Quaternion.identity);
        }
    }

    private Vector3 GeneratePrefabSpawnPosition()
    {
        //Generates a spawnpoint for the powerup
        Vector3 playerPos = GameObject.Find("Player").transform.position;
        Vector3 spawnPos = new Vector3(Random.Range(-prefabSpawnRangeX, prefabSpawnRangeX) + playerPos.x, -4.3f, Random.Range(-prefabSpawnRangeZ, prefabSpawnRangeZ) + playerPos.z);
        return spawnPos;
    }

    private Vector3 GenerateEnemySpawnPosition()
    {
        //Generates a random spawn position outside the camera's view
        bool goodSpawn = false;

        Vector3 finalSpawn = new Vector3();

        while (goodSpawn == false)
        {
            Vector3 playerPos = GameObject.Find("Player").transform.position;

            float spawnPosX = Random.Range(-enemySpawnRangeX, enemySpawnRangeX) + playerPos.x;
            float spawnPosZ = Random.Range(-enemySpawnRangeZ, enemySpawnRangeZ) + playerPos.z;

            Vector3 randomPos = new Vector3(spawnPosX, 0.5f, spawnPosZ);

            Vector3 pos = cam.WorldToViewportPoint(new Vector3(randomPos.x, cam.nearClipPlane, randomPos.z));

            if (!(pos.x < 1 && pos.x > 0) || !(pos.y < 1 && pos.y > 0))
            {
                goodSpawn = true;
                finalSpawn = randomPos;
            }
        }
        return finalSpawn;
    }
}
