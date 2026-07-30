using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeInOnStart : MonoBehaviour
{
    [SerializeField] private float startDelay = 0.25f;
    [SerializeField] private float fadeDuration = 2f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // Begin completely black.
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator Start()
    {
        if (startDelay > 0f)
            yield return new WaitForSecondsRealtime(startDelay);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float progress = elapsedTime / fadeDuration;
            canvasGroup.alpha = 1f - Mathf.Clamp01(progress);

            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}
