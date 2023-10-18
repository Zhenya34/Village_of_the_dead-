using UnityEngine;
using System.Collections;

public class ZombieStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int health = 100;
    [SerializeField] private bool _canAttack;
    public EnemySpawner enemySpawner;
    public PlayerRay playerRay;
    public BossController bossController;
    public int damageZombie;
    public float attackCooldown;
    
    private WaitForSeconds _zombieHealDelayObject;
    private bool _healingCoroutineActive = false;
    public bool IsBoss = false;
    public float zombieHealDelay;
    public int zombieHealingRate;


    private void Start()
    {
        _zombieHealDelayObject = new WaitForSeconds(zombieHealDelay);
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            health = 0;
            Die();
        }
        if (health >= maxHealth)
        {
            health = maxHealth;
        }
    }

    private void Die()
    {
        if (gameObject != null)
        {
            if (gameObject.name == "ZombiemassivePlus")
                playerRay.massiveZombieKilled = true;
            else if (gameObject.name == "ZombieGuardianForest")
                playerRay.zombieGuardianIsKilled = true;

            Destroy(gameObject);

            if (enemySpawner.currentZombieCount > 0)
                enemySpawner.currentZombieCount--;
        }
    }

    private void SetHealthTo(int healthToSetTo)
    {
        health = healthToSetTo;
        CheckHealth();
    }

    public void TakeDamage(int damage)
    {
        int healthAfterDamage = health - damage;
        SetHealthTo(healthAfterDamage);

        if (IsBoss && health < maxHealth && health > 0)
        {
            StartHealing();
            bossController.SpawnGuardian();
        }
        else
        {
            StopHealing();
        }
    }

    private void StartHealing()
    {
        if (!_healingCoroutineActive)
        {
            _healingCoroutineActive = true;
            StartCoroutine(HealOverTime(zombieHealingRate));
        }
    }

    private void StopHealing()
    {
        _healingCoroutineActive = false;
    }

    private IEnumerator HealOverTime(int zombieHealingRate)
    {
        while (health > 0 && health < maxHealth)
        {
            int healthAfterHeal = health + zombieHealingRate;
            SetHealthTo(healthAfterHeal);
            yield return _zombieHealDelayObject;
        }
        StopHealing();
    }
}