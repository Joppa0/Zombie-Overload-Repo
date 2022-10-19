using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefab;
    public GameObject[] powerupPrefab;

    public LayerMask m_LayerMask;
    public Camera cam;

    private RoundCount roundCountScript;

    private int enemyCount;
    private int waveNumber = 2;

    private float enemySpawnRangeX = 20.0f;
    private float enemySpawnRangeZ = 7.5f;
    private float prefabSpawnRangeX = 12f;
    private float prefabSpawnRangeZ = 4f;


    // Start is called before the first frame update
    void Start()
    {
        roundCountScript = GameObject.Find("RoundCounter").GetComponent<RoundCount>();
        cam = Camera.main;
        SpawnEnemyWave(waveNumber, false);
    }

    // Update is called once per frame
    void Update()
    {
        //Spawns a new wave and increments the wave number if there are no enemies left
        enemyCount = FindObjectsOfType<EnemyController>().Length;
        if (enemyCount == 1)
        {
            waveNumber++;
            if (waveNumber % 10 == 0)
            {
                SpawnEnemyWave(waveNumber, true);
            }
            else
            {
                SpawnEnemyWave(waveNumber, false);
            }
        }
    }

    void SpawnEnemyWave(int enemiesToSpawn, bool bossWave)
    {
        roundCountScript.UpdateRoundCounter(enemiesToSpawn - 1);

        //Spawns random new enemies with random positions
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 placeToSpawn = GenerateEnemySpawnPosition();

            int randNumber = Random.Range(0, 3);

            //Guarantees one boss spawning if it's a bosswave
            if (bossWave)
            {
                randNumber = 3;
                bossWave = false;
            }

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

        //Instantiates powerup at a clear location if none remain at the start of a new wave
        if (GameObject.FindObjectsOfType<PowerUp>().Length == 0)
        {
            int prefabsToSpawn = 1;

            for (int i = 0; i < prefabsToSpawn; i++)
            {
                Vector3 placeToSpawn = GeneratePrefabSpawnPosition();

                int randNumber = Random.Range(0, 5);

                float radius = powerupPrefab[randNumber].GetComponent<SphereCollider>().radius * 1.5f;

                Collider[] collisions = Physics.OverlapSphere(placeToSpawn, radius);

                if (collisions.Length > 0 && prefabsToSpawn < 10)
                {
                    prefabsToSpawn++;
                }
                else
                {
                    Instantiate(powerupPrefab[randNumber], placeToSpawn, Quaternion.identity);
                }
            }
        }
    }

    //Spawns the permanent health increase
    public void SpawnHealthIncrease(Vector3 spawnPos)
    {
        Instantiate(powerupPrefab[5], spawnPos, Quaternion.identity);
    }


    //Generates a spawnpoint for the powerup
    private Vector3 GeneratePrefabSpawnPosition()
    {
        Vector3 playerPos = GameObject.Find("Player").transform.position;
        Vector3 spawnPos = new Vector3(Random.Range(-prefabSpawnRangeX, prefabSpawnRangeX) + playerPos.x, 1.2f, Random.Range(-prefabSpawnRangeZ, prefabSpawnRangeZ) + playerPos.z);

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
