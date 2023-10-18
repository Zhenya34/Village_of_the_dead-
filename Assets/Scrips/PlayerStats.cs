using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _health = 100;
    private PlayerHUD _hud;
    public CanvasButton canvas;
    public float healDelay;
    public int healRate;    
    private WaitForSeconds _healDelayObject;

    private void Start()
    {
        GetReferences();
        _healDelayObject = new WaitForSeconds(healDelay);
    }

    private void GetReferences()
    {
        _hud = GetComponent<PlayerHUD>();
    }

    private void Update()
    {
        CheckHealth();
        _hud.UpdateHealth(_health);
    }

    public void StartHealing()
    {
        if (_health > 0)
        {
            StartCoroutine(HealOverTime(healRate));
        }
    }

    private void CheckHealth()
    {
        if (_health <= 0)
        {
            _health = 0;
            Die();
        }
        if (_health >= _maxHealth)
        {
            _health = _maxHealth;
        }
    }

    private void Die()
    {
        StopCoroutine(HealOverTime(healRate));
        canvas.DeactivateLogic();
    }

    private void SetHealthTo(int healthToSetTo)
    {
        _health = healthToSetTo;
        CheckHealth();
    }

    public void TakeDamage(int damage)
    {
        int healthAfterDamage = _health - damage;
        SetHealthTo(healthAfterDamage);
        StartHealing();
    }

    private IEnumerator HealOverTime(int healAmount)
    {
        while (_health < _maxHealth && _health > 0)
        {
            yield return _healDelayObject;
            int healthAfterHeal = _health + healAmount;
            SetHealthTo(healthAfterHeal);
        }
    }
}