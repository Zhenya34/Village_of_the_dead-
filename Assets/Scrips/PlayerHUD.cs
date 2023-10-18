using UnityEngine;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentHealthText;

    public void UpdateHealth(int currentHealth)
    {
        _currentHealthText.text = currentHealth.ToString() + "%";
    }
}
