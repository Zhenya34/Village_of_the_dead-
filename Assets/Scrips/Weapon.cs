using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[System.Serializable]
public class WeaponStarts
{
    public int maxAmmo = 30;
    public int numPellets = 0;
    public float timeBetweenShots = 0.2f; 
    public float reloadTime = 4.2f;

    public bool isAutomatic = false;
    public bool isShotgun = false;
    public bool isSniperR = false;

    public string pistolReturn;
    public string ShotGunReturn;
    public string machineReturn;
    public string SniperReturn;

    public string pistolAiming;
    public string ShotGunAiming;
    public string machineAiming;
    public string SniperAiming;
}

[System.Serializable]
public class WeaponSounds
{
    public AudioClip shootSound;
    public AudioClip casingSound;
    public AudioClip reloadSound;
}

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    public Transform casingPoint;
    public GameObject bulletPrefab;
    public GameObject casingPrefab;

    public WeaponStarts weaponStats;
    public WeaponSounds weaponSounds;

    public int currentAmmo;
    public bool isReloading = false;

    public TextMeshProUGUI ammoText;

    static public bool canShoot = true;

    private AudioSource _audioSource;

    private bool _previousAimingState = false;

    private Animation _weaponReturnAnimation;
    public GameObject weaponModelForReturn;

    private Animation _weaponAmingAnimation;
    public GameObject weaponModelForAming;
    private string _animationNameAming;
    
    private bool _isAiming = false;
    private bool _aimingAnimationStarted = false;
    public GameObject sniperScope;

    static private bool _enableAllAming = true;
    static private bool _enableAllShot = true;

    public float maxSpreadAngle = 10f;

    private Camera _currentFOV;
    public Camera mainCamera;

    private bool _isZooming = false;

    private void Start()
    {
        currentAmmo = weaponStats.maxAmmo;

        if (!TryGetComponent<AudioSource>(out _audioSource))
            _audioSource = gameObject.AddComponent<AudioSource>();

        ammoText.text = currentAmmo.ToString();

        _weaponReturnAnimation = weaponModelForReturn.GetComponent<Animation>();
        _weaponAmingAnimation = weaponModelForAming.GetComponent<Animation>();

        _currentFOV = mainCamera.GetComponent<Camera>();
    }

    private void StartWeaponAnim()
    {
        string animationNameReturn = weaponStats.isShotgun ? weaponStats.ShotGunReturn :
                      (weaponStats.isAutomatic ? weaponStats.machineReturn :
                      (weaponStats.isSniperR ? weaponStats.SniperReturn : weaponStats.pistolReturn));
        _weaponReturnAnimation.Play(animationNameReturn);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isReloading)
                Reload();
        }

        if (_enableAllShot && Input.GetMouseButton(0) && canShoot && !IsPointerOverUI())
        {
            Shoot();
        }

        static bool IsPointerOverUI()
        {
            PointerEventData eventData = new(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);

            return results.Exists(result => result.gameObject.layer == LayerMask.NameToLayer("UI"));
        }


        bool currentAimingState = Input.GetMouseButton(1);


        if (currentAimingState != _previousAimingState)
        {
            _isAiming = currentAimingState;

            if (_enableAllAming)            
                PlayAimingAnimation(_isAiming);
            
                
            _aimingAnimationStarted = false;
            _isZooming = false;
        }

        _previousAimingState = currentAimingState;

        if (_isAiming && weaponStats.isSniperR)
        {
            if (!_aimingAnimationStarted && !_isZooming)
            {
                _aimingAnimationStarted = true;

                if (_enableAllAming)                
                    StartCoroutine(StartZoomAfterDelay());
            }

            if (_isZooming)
            {
                sniperScope.SetActive(true);
                weaponModelForReturn.SetActive(false);
                float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

                _currentFOV.fieldOfView -= scrollDelta * 10f;
                _currentFOV.fieldOfView = Mathf.Clamp(_currentFOV.fieldOfView, 5f, 30f);
            }
        }
        else
        {
            _aimingAnimationStarted = false;
            _currentFOV.fieldOfView = 60;
            sniperScope.SetActive(false);
            weaponModelForReturn.SetActive(true);
        }
    }

    private IEnumerator StartZoomAfterDelay()
    {
        yield return new WaitForSeconds(0.8f);
        _isZooming = true;
    }

    private void PlayAimingAnimation(bool aimingState)
    {
        if (aimingState)
        {
            if (weaponStats.isShotgun)
                _animationNameAming = weaponStats.ShotGunAiming;
            else if (weaponStats.isAutomatic)
                _animationNameAming = weaponStats.machineAiming;
            else if (weaponStats.isSniperR)
                _animationNameAming = weaponStats.SniperAiming;
            else
                _animationNameAming = weaponStats.pistolAiming;


            _weaponAmingAnimation[_animationNameAming].normalizedTime = 0;
            _weaponAmingAnimation[_animationNameAming].speed = 1;
            _weaponAmingAnimation.Play(_animationNameAming);
        }
        else
        {
            _weaponAmingAnimation[_animationNameAming].normalizedTime = 1;
            _weaponAmingAnimation[_animationNameAming].speed = -1;
            _weaponAmingAnimation.Play(_animationNameAming);
        }
    }

    public void ToggleSound(bool isSoundEnabled)
    {
        _audioSource.mute = !isSoundEnabled;
    }

    public void EnableShooting()
    {
        canShoot = true;
        _enableAllShot = true;
        _enableAllAming = true;
    }

    public void EnableCanShoot()
    {
        canShoot = true;
    }

    public void DisableShooting()
    {
        canShoot = false;
        _enableAllShot = false;
        _enableAllAming = false;
    }

    private IEnumerator ShootWithDelay()
    {
        if (_enableAllShot)
        {
            canShoot = false;
            float shootDelay = weaponStats.isAutomatic ? 0.05f : weaponStats.timeBetweenShots;
            yield return new WaitForSeconds(shootDelay);
            canShoot = true;
        }
    }

    private void Shoot()
    {
        if (currentAmmo > 0 && isReloading == false)
        {
            if (weaponStats.isShotgun)
            {
                for (int i = 0; i < weaponStats.numPellets; i++)
                {
                    GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                    float randomPitch = Random.Range(-maxSpreadAngle, maxSpreadAngle);
                    float randomYaw = Random.Range(-maxSpreadAngle, maxSpreadAngle);

                    bullet.transform.Rotate(randomPitch, randomYaw, 0);

                    Destroy(bullet, 1f);
                }
            }
            else
            {
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Destroy(bullet, 1.8f);
            }

            _audioSource.PlayOneShot(weaponSounds.shootSound);
            StartWeaponAnim();
            currentAmmo--;
            CreateCasing();
            StartCoroutine(ShootWithDelay());
        }

        ammoText.text = currentAmmo.ToString();

        if (currentAmmo <= 0)
            Reload();
    }

    private void CreateCasing()
    {
        GameObject casing = Instantiate(casingPrefab, casingPoint.position, casingPoint.rotation);
        _audioSource.PlayOneShot(weaponSounds.casingSound);
        Destroy(casing, 1.4f);
    }


    private void Reload()
    {
        if (!isReloading && currentAmmo < weaponStats.maxAmmo)
        {
            isReloading = true;
            _audioSource.PlayOneShot(weaponSounds.reloadSound);
            StartCoroutine(CompleteReload());
        }
    }

    private IEnumerator CompleteReload()
    {
        yield return new WaitForSeconds(weaponStats.reloadTime);
        currentAmmo = weaponStats.maxAmmo;
        isReloading = false;
        ammoText.text = currentAmmo.ToString();
    }
}