using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public class CanvasButton : MonoBehaviour
{
    public Terrain terrain;

    public GameObject panelNote_1;
    public GameObject panelNote_2;

    public GameObject pauseButton;
    public GameObject menuCanvas;
    public GameObject settingsMenu;

    public Player_Movement playerMovement;
    public FlashlightButton flashLight;
    public Weapon_Changer weaponChanger;
    public ZombieController zombieController;
    public PlayerRay playerRay;
    public Weapon weapon;

    public Volume volume;
    private ColorAdjustments _colorAdjustments;

    private bool _isSoundEnabled = true;

    public GameObject NightSoundVolume;
    public GameObject AllFlashLightLight;
    public GameObject AllPistolLight;
    public GameObject AllShotGunLight;
    public GameObject AllMachineGunLight;
    public GameObject AllSniperRifleLight;

    [SerializeField] private GameObject _menuOfDead;

    public bool isPauseEnabled = true;

    private void Start()
    {
        if (volume != null && volume.profile.TryGet<ColorAdjustments>(out _colorAdjustments))
            _colorAdjustments.active = true;
    }

    public void DetailDensityChanged(float newDensity)
    {
        if (terrain != null)
            terrain.detailObjectDensity = newDensity;
        
    }

    public void ToggleSound()
    {
        _isSoundEnabled = !_isSoundEnabled;
        NightSoundVolume.SetActive(!NightSoundVolume.activeSelf);

        weapon.ToggleSound(_isSoundEnabled);
        weaponChanger.ToggleSound(_isSoundEnabled);
        playerMovement.ToggleSound(_isSoundEnabled);

        PlayerPrefs.SetInt("SoundEnabled", _isSoundEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }


    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void CloseGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenGameMenu()
    {

        if (isPauseEnabled)
        {
            weaponChanger.isWeaponEnabled = false;
            weapon.DisableShooting();
            pauseButton.SetActive(false);
            menuCanvas.SetActive(true);
            settingsMenu.SetActive(false);

            playerMovement.isMovementEnabled = false;
            flashLight.isFlashLightEnabled = false;
            zombieController.DisableMoving();
        }
    }

    public void CloseGameMenu()
    {
        pauseButton.SetActive(true);
        menuCanvas.SetActive(false);

        playerMovement.isMovementEnabled = true;
        flashLight.isFlashLightEnabled = true;
        weaponChanger.isWeaponEnabled = true;
        zombieController.EnableMoving();
        weapon.EnableShooting();
    }

    public void DeactivateLogic()
    {
        _menuOfDead.SetActive(true);
    }

    public void CloseNoteAll()
    {
        panelNote_1.SetActive(false);
        panelNote_2.SetActive(false);
        playerRay.canvasButton.isPauseEnabled = true;
        weapon.EnableShooting();
    }

    public void OpenGameSettings()
    {
        weapon.DisableShooting();

        pauseButton.SetActive(false);
        menuCanvas.SetActive(false);
        settingsMenu.SetActive(true);

        playerMovement.isMovementEnabled = false;
        flashLight.isFlashLightEnabled = false;
        weaponChanger.isWeaponEnabled = false;
    }

    public void SetDarkNight()
    {
        _colorAdjustments.active = !_colorAdjustments.active;
        AllFlashLightLight.SetActive(!AllFlashLightLight.activeSelf);
        AllPistolLight.SetActive(!AllPistolLight.activeSelf);
        AllShotGunLight.SetActive(!AllShotGunLight.activeSelf);
        AllMachineGunLight.SetActive(!AllMachineGunLight.activeSelf);
        AllSniperRifleLight.SetActive(!AllSniperRifleLight.activeSelf);
        flashLight.lightStatus.enabled = false;
    }
}
