using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private ZombieStats _zombieStats;
    private ZombieController _zombieController;
    public Transform target;

    public GameObject zombieGuardianObject;
    public GameObject smallZombiePrefab;
    public Transform smallZombieSpawnPoint;

    public int maxSmallZombiesToSpawn;
    private int _numZombies = 3;

    public List<GameObject> spawnedSmallZombies = new();
    private int _nonNullCount;
    public float spawnCooldown = 5.0f;
    private float _lastSpawnTime;

    private void Start()
    {
        _zombieStats = GetComponent<ZombieStats>();
        _zombieController = GetComponent<ZombieController>();
    }

    private void Update()
    {
        float distanceToTarget = Vector3.Distance(target.position, transform.position);

        if (distanceToTarget <= _zombieController.visionRadius)
        {
            SpawnRegularZombies();
        }
    }

    public void SpawnGuardian()
    {
        if (_zombieStats.health <= _zombieStats.maxHealth / 2)
        {
            zombieGuardianObject.SetActive(true);
        }
    }

    private void SpawnRegularZombies()
    {
        _nonNullCount = spawnedSmallZombies.FindAll(zombie => zombie != null).Count;
        spawnedSmallZombies.RemoveAll(zombie => zombie == null);

        if (Time.time >= _lastSpawnTime + spawnCooldown && _nonNullCount < maxSmallZombiesToSpawn)
        {
            int numSmallZombiesToSpawn = CalculateSmallZombiesToSpawn();
            SpawnZombies(numSmallZombiesToSpawn);
            _lastSpawnTime = Time.time;
        }
    }

    private int CalculateSmallZombiesToSpawn()
    {
        if (_zombieStats.health == _zombieStats.maxHealth)        
            _numZombies = 3;        

        if (_zombieStats.health <= _zombieStats.maxHealth * 0.7)        
            _numZombies = 5;      

        if (_zombieStats.health <= _zombieStats.maxHealth * 0.5)        
            _numZombies = 8;        

        if (_zombieStats.health <= _zombieStats.maxHealth * 0.3)        
            _numZombies = 12;        

        return Mathf.Min(maxSmallZombiesToSpawn - _nonNullCount, _numZombies);
    }

    private void SpawnZombies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = smallZombieSpawnPoint.position;
            GameObject newZombie = Instantiate(smallZombiePrefab, spawnPosition, Quaternion.identity);
            spawnedSmallZombies.Add(newZombie);
        }
    }
}