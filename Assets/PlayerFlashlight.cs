using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    [SerializeField] private Light flashlightLight;
    [SerializeField] private KeyCode toggleKey = KeyCode.F;
    [SerializeField] private bool startOn = true;

    private void Awake()
    {
        if (flashlightLight == null)
            flashlightLight = GetComponentInChildren<Light>(true);

        if (flashlightLight == null)
        {
            Debug.LogError("No Light component found in the flashlight.");
            enabled = false;
            return;
        }

        flashlightLight.enabled = startOn;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            flashlightLight.enabled = !flashlightLight.enabled;
    }
}