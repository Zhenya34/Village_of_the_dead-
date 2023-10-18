using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ZombieController : MonoBehaviour
{
    public int attackType = 0;
    public int bossAttackType = 0;
    private NavMeshAgent _agent;
    private Animator _anim;
    private ZombieStats _stats;
    private float _timeOfLastAttack = 0;
    private bool _hasStopped = false;
    private PlayerStats _playerStats;
    static private bool _pauseState = false;

    [SerializeField] private Transform _target;

    public Color visionGizmoColor = Color.green;
    public float visionRadius;
    public Color attackRadiusGizmoColor = Color.red;
    public float attackRadius;

    public Vector3 gizmoOffset;

    public AudioClip[] zombieSounds;
    public int minSoundDelay = 10;
    public int maxSoundDelay = 20;


    private void Start()
    {
        GetReferences();
        StartCoroutine(PlayZombieSounds());
    }

    private void Update()
    {
        MoveToTarget();
    }

    public void DisableMoving()
    {
        _pauseState = true;
    }

    public void EnableMoving()
    {
        _pauseState = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = attackRadiusGizmoColor;
        Gizmos.DrawWireSphere(transform.position + gizmoOffset, attackRadius);

        Gizmos.color = visionGizmoColor;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }

    private void MoveToTarget()
    {
        float distanceToTarget = Vector3.Distance(_target.position, transform.position);
        
        if (distanceToTarget <= visionRadius && !_pauseState)
        {
            _agent.isStopped = false;
            _agent.SetDestination(_target.position);
            _anim.SetFloat("Speed", 1f, 0.3f, Time.deltaTime);
            RotateToTarget();

            if (distanceToTarget <= attackRadius)
            {
                _anim.SetFloat("Speed", 0f);

                if (!_hasStopped)
                {
                    _hasStopped = true;
                    _timeOfLastAttack = Time.time;
                }

                if (Time.time >= _timeOfLastAttack + _stats.attackCooldown)
                {
                    _timeOfLastAttack = Time.time;
                    AttackTarget();
                    ManageAttackGuardians();
                    ManageAttackBoss();
                    _playerStats.StartHealing();
                }
            }
            else
            {
                attackType = 0;
                bossAttackType = 0;
                _anim.SetInteger("AttackType", attackType);
                _anim.SetInteger("BossAttackType", bossAttackType);
                if (_hasStopped)
                    _hasStopped = false;
            }
        }
        else
        {
            _anim.SetFloat("Speed", 0f);
            _agent.isStopped = true;
        }
    }

    private void RotateToTarget()
    {
        Vector3 direction = _target.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up).normalized;
            transform.rotation = rotation;
        }
    }

    private void GetReferences()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponentInChildren<Animator>();
        _stats = GetComponent<ZombieStats>();
        _playerStats = FindObjectOfType<PlayerStats>();
    }

    private void AttackTarget()
    {
        _anim.SetTrigger("attack");
        _playerStats.TakeDamage(_stats.damageZombie);
    }

    private void ManageAttackGuardians()
    {
        attackType = Random.Range(1, 3);
        _anim.SetInteger("AttackType", attackType);
        _playerStats.TakeDamage(_stats.damageZombie);
    }

    private void ManageAttackBoss()
    {
        bossAttackType = Random.Range(1, 4);
        _anim.SetInteger("BossAttackType", bossAttackType);
        _playerStats.TakeDamage(_stats.damageZombie);
    }

    private IEnumerator PlayZombieSounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSoundDelay, maxSoundDelay));

            if (zombieSounds.Length > 0)
            {
                int randomSoundIndex = Random.Range(0, zombieSounds.Length);
                AudioSource.PlayClipAtPoint(zombieSounds[randomSoundIndex], transform.position);
            }
        }

    }
}
