using System.Collections;
using System.Collections.Generic;
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

    // These remain populated when the scene reloads.
    private static readonly List<int> remainingAnomalyIndices =
        new List<int>();

    private static int lastAnomalyIndex = -1;
    private static int rememberedVariantCount = -1;

    [Header("Round")]
    [Range(0f, 1f)]
    [SerializeField] private float anomalyChance = 0.5f;

    [SerializeField] private GameObject[] anomalyVariants;

    [Header("Win")]
    [SerializeField, Min(1)] private int winningStreak = 10;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private float winDelay = 0.75f;

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
    private bool gameWon;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticValues()
    {
        Instance = null;
        Streak = 0;
        firstRoundHasStarted = false;

        remainingAnomalyIndices.Clear();
        lastAnomalyIndex = -1;
        rememberedVariantCount = -1;
    }

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        Time.timeScale = 1f;

        if (winPanel != null)
            winPanel.SetActive(false);

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

        DisableAllAnomalies();

        // Only the first round of the entire game is guaranteed normal.
        if (!firstRoundHasStarted)
        {
            roundHasAnomaly = false;
            firstRoundHasStarted = true;
        }
        else
        {
            roundHasAnomaly =
                HasValidAnomalies() &&
                Random.value < anomalyChance;
        }

        if (roundHasAnomaly)
        {
            // If activation fails, treat this as a normal round.
            roundHasAnomaly = ActivateNextAnomaly();
        }

        UpdateSign();
    }

    private bool ActivateNextAnomaly()
    {
        // Rebuild the bag if the Inspector array changed.
        if (rememberedVariantCount != anomalyVariants.Length)
        {
            remainingAnomalyIndices.Clear();
            rememberedVariantCount = anomalyVariants.Length;
            lastAnomalyIndex = -1;
        }

        if (remainingAnomalyIndices.Count == 0)
            RefillAnomalyBag();

        if (remainingAnomalyIndices.Count == 0)
            return false;

        int bagPosition =
            Random.Range(0, remainingAnomalyIndices.Count);

        // Prevent the final anomaly from one cycle from becoming
        // the first anomaly in the next cycle.
        if (remainingAnomalyIndices.Count > 1 &&
            remainingAnomalyIndices[bagPosition] ==
            lastAnomalyIndex)
        {
            bagPosition =
                (bagPosition + 1) %
                remainingAnomalyIndices.Count;
        }

        int anomalyIndex =
            remainingAnomalyIndices[bagPosition];

        remainingAnomalyIndices.RemoveAt(bagPosition);

        GameObject selectedAnomaly =
            anomalyVariants[anomalyIndex];

        selectedAnomaly.SetActive(true);

        MultiObjectMoveAnomaly[] movers =
            selectedAnomaly.GetComponentsInChildren
            <MultiObjectMoveAnomaly>(true);

        foreach (MultiObjectMoveAnomaly mover in movers)
            mover.ApplyAnomaly();

        lastAnomalyIndex = anomalyIndex;

        Debug.Log(
            "Activated anomaly: " +
            selectedAnomaly.name +
            ". Remaining before repeats: " +
            remainingAnomalyIndices.Count
        );

        return true;
    }

    private void RefillAnomalyBag()
    {
        remainingAnomalyIndices.Clear();

        HashSet<GameObject> addedObjects =
            new HashSet<GameObject>();

        for (int i = 0; i < anomalyVariants.Length; i++)
        {
            GameObject anomaly = anomalyVariants[i];

            // Also prevents duplicate Inspector entries.
            if (anomaly != null && addedObjects.Add(anomaly))
                remainingAnomalyIndices.Add(i);
        }

        Debug.Log(
            "Anomaly bag refilled with " +
            remainingAnomalyIndices.Count +
            " anomalies."
        );
    }

    private bool HasValidAnomalies()
    {
        foreach (GameObject anomaly in anomalyVariants)
        {
            if (anomaly != null)
                return true;
        }

        return false;
    }

    private void DisableAllAnomalies()
    {
        foreach (GameObject anomaly in anomalyVariants)
        {
            if (anomaly == null)
                continue;

            MultiObjectMoveAnomaly[] movers =
                anomaly.GetComponentsInChildren
                <MultiObjectMoveAnomaly>(true);

            foreach (MultiObjectMoveAnomaly mover in movers)
                mover.RestoreNormal();

            anomaly.SetActive(false);
        }
    }

    public void BeginInspection()
    {
        inspectionStarted = true;
    }

    public bool SubmitChoice(ExitChoice choice)
    {
        if (resolving ||
            gameWon ||
            !readyForChoice ||
            !inspectionStarted)
        {
            return false;
        }

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

        if (Streak >= winningStreak)
        {
            StartCoroutine(ShowWinScreen());
        }
        else
        {
            StartCoroutine(FinishRound());
        }

        return true;
    }

    private IEnumerator ShowWinScreen()
    {
        gameWon = true;

        // Allow the final door animation/audio to begin.
        if (winDelay > 0f)
            yield return new WaitForSecondsRealtime(winDelay);

        DisableAllAnomalies();

        if (winPanel != null)
            winPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("YOU WIN! Final streak: " + Streak);

        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        Streak = 0;
        firstRoundHasStarted = false;
        remainingAnomalyIndices.Clear();
        lastAnomalyIndex = -1;
        rememberedVariantCount = -1;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
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