using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] zombieTypes;
    public Transform[] spawnPoints;
    public float minSpawnDelay = 5.0f;
    public float maxSpawnDelay = 10.0f;
    public int minZombiesToSpawn = 2;
    public int maxZombiesToSpawn = 5;
    public int maxZombies = 10;

    public int currentZombieCount = 0;
    public float nextSpawnTime = 0f;

    public bool spawnerInActiveRegion = false;
    public RegionTrigger region;

    private void Update()
    {
        if (Time.time >= nextSpawnTime && currentZombieCount < maxZombies)
        {
            int numZombiesToSpawn = Random.Range(minZombiesToSpawn, maxZombiesToSpawn + 1);

            for (int i = 0; i < numZombiesToSpawn; i++)
            {
                SpawnZombie();
            }

            nextSpawnTime = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay);
        }
    }

    private void SpawnZombie()
    {
        if (!spawnerInActiveRegion || currentZombieCount >= maxZombies)
            return;

        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);
        int randomZombieTypeIndex = Random.Range(0, zombieTypes.Length);

        GameObject newZombie = Instantiate(zombieTypes[randomZombieTypeIndex],
                                          spawnPoints[randomSpawnPointIndex].position,
                                          Quaternion.identity);
        currentZombieCount++;
        region.zombiesInRegion.Add(newZombie);
    }
}