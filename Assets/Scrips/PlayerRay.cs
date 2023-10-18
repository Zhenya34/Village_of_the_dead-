using UnityEngine;
using TMPro;

public class PlayerRay : MonoBehaviour
{
    public CanvasButton canvasButton;
    public GameObject CaveGate;    
    public GameObject panelNote1;
    public GameObject panelNote2;

    public int gemsScore = 0;
    public readonly int totalGems = 3;
    public TextMeshProUGUI gemsScoreText;

    private Selectable _currentSelectable;
    private Selectable _selectable;

    private RaycastHit _hit;

    public TextMeshProUGUI mushroomScoreText;
    private int _mushroomScore = 0;

    public TextMeshProUGUI keyScoreText;
    private int _keyScore = 0;
    private readonly int _totalKeys = 3;

    private bool _isDoorOpen = false;
    public GameObject door1;
    private Rigidbody _doorRigidbody1;

    public GameObject door2;
    private Rigidbody _doorRigidbody2;

    public GameObject chest;
    private Animation _chestAnimation;
    public Weapon_Changer _weaponChanger;

    public GameObject zombieMassive;
    public ZombieStats massiveZombieStats;
    public bool massiveZombieKilled = false;

    public bool zombieGuardianIsKilled;

    public Weapon _weapon;

    private readonly float _maxRaycastDistance = 4f;

    private void Start()
    {
        _doorRigidbody1 = door1.GetComponentInParent<Rigidbody>();
        _doorRigidbody2 = door2.GetComponentInParent<Rigidbody>();        
        _chestAnimation = chest.GetComponent<Animation>();
    }
    
    private void StartChestAnim()
    {
        _chestAnimation.Play("Chest");
    }

    private void Update()
    {
        if(_currentSelectable != null)
        {
            if (Input.GetKeyUp(KeyCode.E))
            {
                ItemCollecting();
            }
            else if (Input.GetKeyUp(KeyCode.F))
            {
                ItemUsing();
            }
        }
    }

    private void ItemCollecting()
    {
        if (_currentSelectable != null)
        {
            if (_hit.collider.CompareTag("Mushroom"))
            {
                _currentSelectable.transform.parent.gameObject.SetActive(false);
                _mushroomScore++;
                mushroomScoreText.text = _mushroomScore.ToString();
            }
            else if (_hit.collider.CompareTag("Key"))
            {
                _currentSelectable.transform.parent.gameObject.SetActive(false);
                _keyScore++;
                keyScoreText.text = string.Format("{0}/{1}", _keyScore, _totalKeys);
            }
            else if (_hit.collider.CompareTag("redGems"))
            {               
                if (massiveZombieKilled)
                {
                    _currentSelectable.transform.gameObject.SetActive(false);
                    gemsScore++;
                    gemsScoreText.text = string.Format("{0}/{1}", gemsScore, totalGems);
                }

                if(zombieMassive != null)
                    zombieMassive.SetActive(true);
            }
            else if (_hit.collider.CompareTag("greenGems"))
            {
                _currentSelectable.transform.gameObject.SetActive(false);
                gemsScore++;
                gemsScoreText.text = string.Format("{0}/{1}", gemsScore, totalGems);
            }
            else if (_hit.collider.CompareTag("note1"))
            {
                _currentSelectable.transform.gameObject.SetActive(false);                
                panelNote1.SetActive(true);
                canvasButton.isPauseEnabled = false;
                _weapon.DisableShooting();
            }
            else if (_hit.collider.CompareTag("note2"))
            {
                _currentSelectable.transform.gameObject.SetActive(false);
                panelNote2.SetActive(true);
                canvasButton.isPauseEnabled = false;
                _weapon.DisableShooting();
            }
            _currentSelectable = null;
        }
    }

    private void ItemUsing()
    {
        if (_currentSelectable != null)
        {
            if (_hit.collider.CompareTag("Door1") && _mushroomScore >= 15 && !_isDoorOpen)
            {
                _doorRigidbody1.isKinematic = false;
                _weaponChanger.shotGunIsEnable = true;
                _weaponChanger.SwitchWeapon(_weaponChanger.shotGun);
                _weaponChanger.audioSource.PlayOneShot(_weaponChanger.gunChangeSound);
                _isDoorOpen = true;
            }
            else if (_hit.collider.CompareTag("Chest") && _keyScore >= 3)
            {
                StartChestAnim();
                _weaponChanger.sniperRifleIsEnable = true;
                _keyScore = 0;
                keyScoreText.text = string.Format("{0}/{1}", _keyScore, _totalKeys);
                _weaponChanger.SwitchWeapon(_weaponChanger.sniperRifle);
                _weaponChanger.audioSource.PlayOneShot(_weaponChanger.gunChangeSound);
            }
            else if (_hit.collider.CompareTag("Door2") && zombieGuardianIsKilled)
            {
                _doorRigidbody2.isKinematic = false;
                _weaponChanger.machineGunIsEnable = true;
                _weaponChanger.SwitchWeapon(_weaponChanger.machineGun);
                _weaponChanger.audioSource.PlayOneShot(_weaponChanger.gunChangeSound);
            }
            else if (_hit.collider.CompareTag("CaveGate") && gemsScore >= 3)
            {
                CaveGate.SetActive(false);
                gemsScore = 0;
                gemsScoreText.text = string.Format("{0}/{1}", gemsScore, totalGems);
                _weaponChanger.pistol.EnableShooting();
            }

            _currentSelectable = null;
        }
    }

    void LateUpdate()
    {
        Ray ray = new(transform.position, transform.forward);
        Debug.DrawRay(transform.position, transform.forward * _maxRaycastDistance, Color.red);

        if (Physics.Raycast(ray, out _hit, _maxRaycastDistance))
        {
            _selectable = _hit.collider.gameObject.GetComponent<Selectable>();

            if (_selectable)
            {
                if (_currentSelectable && _currentSelectable != _selectable)
                {
                    _currentSelectable.Deselect();
                }

                if (_currentSelectable != _selectable)
                {
                    _currentSelectable = _selectable;
                    _currentSelectable.Select();
                }
            }
            else
            {
                if (_currentSelectable)
                {
                    _currentSelectable.Deselect();
                    _currentSelectable = null;
                }
            }
        }
        else
        {
            if (_currentSelectable)
            {
                _currentSelectable.Deselect();
                _currentSelectable = null;
            }
        }
    }
}