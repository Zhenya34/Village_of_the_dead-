using System.Collections.Generic;
using UnityEngine;

public class RegionTrigger : MonoBehaviour
{
    public Color regionColor = Color.green;
    public float regionRadius = 5f;
    public Transform player;
    public GameObject spawnerInRegion;
    public List<GameObject> zombiesInRegion = new();
    public EnemySpawner enemySpawner;

    private void OnDrawGizmos()
    {
        Gizmos.color = regionColor;
        Gizmos.DrawWireSphere(transform.position, regionRadius);
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        if (distanceToPlayer <= regionRadius)
            ActivateRegion();
        else
            DeactivateRegion();
        
    }

    private void ActivateRegion()
    {
        spawnerInRegion.SetActive(true);
        enemySpawner.spawnerInActiveRegion = true;

        foreach (var zombie in zombiesInRegion)
        {
            if (zombie != null)
            {
                zombie.SetActive(true);
            }
        }
    }

    private void DeactivateRegion()
    {
        spawnerInRegion.SetActive(false);
        enemySpawner.spawnerInActiveRegion = false;

        foreach (var zombie in zombiesInRegion)
        {
            if (zombie != null)
            {
                zombie.SetActive(false);
            }
        }
    }
}