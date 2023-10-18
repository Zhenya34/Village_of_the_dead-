using UnityEngine;

public class Weapon_Changer : MonoBehaviour
{
    public bool isWeaponEnabled = true;

    public Weapon pistol;
    public Weapon shotGun;
    public Weapon machineGun;
    public Weapon sniperRifle;

    public bool pistolIsEnable = true;
    public bool shotGunIsEnable = false;
    public bool machineGunIsEnable = false;
    public bool sniperRifleIsEnable = false;

    public AudioSource audioSource;

    public AudioClip gunChangeSound;
    public AudioClip bellSound;

    public Weapon weapon;

    private void Start()
    {
        if (!TryGetComponent<AudioSource>(out audioSource))
            audioSource = gameObject.AddComponent<AudioSource>();

        SwitchWeapon(pistol);
    }

    private void Update()
    {
        if (isWeaponEnabled)
        {
            if (Input.GetKeyUp(KeyCode.Alpha1))
                SwitchWeapon(pistol);
            else if (Input.GetKeyUp(KeyCode.Alpha2) && shotGunIsEnable)
                SwitchWeapon(shotGun);
            else if (Input.GetKeyUp(KeyCode.Alpha3) && machineGunIsEnable)
                SwitchWeapon(machineGun);
            else if (Input.GetKeyUp(KeyCode.Alpha4) && sniperRifleIsEnable)
                SwitchWeapon(sniperRifle);
        }
    }

    public void SwitchWeapon(Weapon weaponToEnable)
    {
        pistol.isReloading = false;
        shotGun.isReloading = false;
        machineGun.isReloading = false;
        sniperRifle.isReloading = false;

        pistol.gameObject.SetActive(false);
        shotGun.gameObject.SetActive(false);
        machineGun.gameObject.SetActive(false);
        sniperRifle.gameObject.SetActive(false);

        weaponToEnable.gameObject.SetActive(true);
        audioSource.PlayOneShot(gunChangeSound);
        weaponToEnable.ammoText.text = weaponToEnable.currentAmmo.ToString();
        
        weapon.EnableCanShoot();
    }

    public void ToggleSound(bool isSoundEnabled)
    {
        audioSource.mute = !isSoundEnabled;
    }

    public void BellSound()
    {
        audioSource.PlayOneShot(bellSound);
    }
}
