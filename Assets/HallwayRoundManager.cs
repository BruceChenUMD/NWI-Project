using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ExitChoice
{
    SawAnomaly,
    NoAnomaly
}

public class HallwayRoundManager : MonoBehaviour
{
    public static HallwayRoundManager Instance { get; private set; }
    public static int Streak { get; private set; }

    private static bool firstRoundHasStarted;

    [Header("Round")]
    [Range(0f, 1f)]
    [SerializeField] private float anomalyChance = 0.5f;
    [SerializeField] private GameObject[] anomalyVariants;

    [Header("References")]
    [SerializeField] private TMP_Text streakSign;

    [Header("Optional Anti-Cheese")]
    [SerializeField] private bool requireInspectionTrigger;

    [Header("Fade Transition")]
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float blackScreenDuration = 0.15f;

    private bool roundHasAnomaly;
    private bool inspectionStarted;
    private bool readyForChoice;
    private bool resolving;

    // Resets static values when starting the game.
    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticValues()
    {
        Instance = null;
        Streak = 0;
        firstRoundHasStarted = false;
    }

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        PrepareRound();

        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 1f;
            fadeCanvas.blocksRaycasts = true;

            yield return FadeTo(0f);

            fadeCanvas.blocksRaycasts = false;
        }

        readyForChoice = true;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void PrepareRound()
    {
        inspectionStarted = !requireInspectionTrigger;

        // Turn off every anomaly before selecting one.
        foreach (GameObject anomaly in anomalyVariants)
        {
            if (anomaly != null)
                anomaly.SetActive(false);
        }

        // The first round is always normal.
        if (!firstRoundHasStarted)
        {
            roundHasAnomaly = false;
            firstRoundHasStarted = true;
        }
        else
        {
            roundHasAnomaly =
                anomalyVariants.Length > 0 &&
                Random.value < anomalyChance;
        }

        // If this round has an anomaly, activate only one.
        if (roundHasAnomaly)
        {
            int selectedAnomaly =
                Random.Range(0, anomalyVariants.Length);

            if (anomalyVariants[selectedAnomaly] != null)
                anomalyVariants[selectedAnomaly].SetActive(true);
        }

        UpdateSign();
    }

    public void BeginInspection()
    {
        inspectionStarted = true;
    }

    public bool SubmitChoice(ExitChoice choice)
    {
        if (resolving || !readyForChoice || !inspectionStarted)
            return false;

        resolving = true;
        readyForChoice = false;

        bool playerReportedAnomaly =
            choice == ExitChoice.SawAnomaly;

        bool correct =
            playerReportedAnomaly == roundHasAnomaly;

        if (correct)
        {
            Streak++;
            Debug.Log("Correct! Streak: " + Streak);
        }
        else
        {
            Streak = 0;
            Debug.Log("Wrong! Streak reset.");
        }

        UpdateSign();
        StartCoroutine(FinishRound());

        return true;
    }

    private void UpdateSign()
    {
        if (streakSign != null)
            streakSign.text = Streak.ToString();
    }

    private IEnumerator FinishRound()
    {
        if (fadeCanvas != null)
        {
            fadeCanvas.blocksRaycasts = true;
            yield return FadeTo(1f);
        }

        if (blackScreenDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(
                blackScreenDuration
            );
        }

        int sceneIndex =
            SceneManager.GetActiveScene().buildIndex;

        yield return SceneManager.LoadSceneAsync(sceneIndex);
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        if (fadeCanvas == null)
            yield break;

        float startingAlpha = fadeCanvas.alpha;
        float elapsed = 0f;

        if (fadeDuration <= 0f)
        {
            fadeCanvas.alpha = targetAlpha;
            yield break;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            fadeCanvas.alpha = Mathf.Lerp(
                startingAlpha,
                targetAlpha,
                elapsed / fadeDuration
            );

            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }
}