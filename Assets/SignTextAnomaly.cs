using TMPro;
using UnityEngine;

public class SignTextAnomaly : MonoBehaviour
{
    [Header("Sign")]
    [SerializeField] private TMP_Text signText;

    [Header("Anomaly Text")]
    [SerializeField] private string anomalyText = "HELL";

    private string normalText;
    private bool initialized;

    private void Initialize()
    {
        if (initialized || signText == null)
            return;

        normalText = signText.text;
        initialized = true;
    }

    private void OnEnable()
    {
        Initialize();

        if (signText != null)
            signText.text = anomalyText;
    }

    private void OnDisable()
    {
        RestoreNormal();
    }

    public void RestoreNormal()
    {
        if (!initialized || signText == null)
            return;

        signText.text = normalText;
    }
}