using UnityEngine;

public class FlashlightButton : MonoBehaviour
{
    public Light lightStatus;
    public bool isFlashLightEnabled = true;

    private void Start()
    {
        lightStatus = GetComponent<Light>();
    }

    private void Update()
    {
        if (isFlashLightEnabled)
        {
            if (Input.GetKeyUp(KeyCode.L))
                lightStatus.enabled = !lightStatus.enabled;
        }

    }
}
