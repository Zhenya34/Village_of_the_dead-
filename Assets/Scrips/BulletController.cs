using UnityEngine;

public class BulletController : MonoBehaviour
{
    public int damage;
    public float speed;

    private Rigidbody _rb;
    public PlayerRay _playerRay;
    private ZombieStats _zombieStats;
    public Weapon_Changer _weaponChanger;

    private static bool _gemCollected = false;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.TryGetComponent<ZombieStats>(out _zombieStats))
            {
                _zombieStats.TakeDamage(damage);
            }
        }
        else if (collision.gameObject.CompareTag("bell") && _gemCollected == false)
        {
            _playerRay.gemsScore++;
            _playerRay.gemsScoreText.text = string.Format("{0}/{1}", _playerRay.gemsScore, _playerRay.totalGems);
            _gemCollected = true;
            _weaponChanger.BellSound();
            Destroy(gameObject);
        }
    }
}
