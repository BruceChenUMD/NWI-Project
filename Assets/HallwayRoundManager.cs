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
    private static bool initialFadePlayed;

    // Persists while the Gallery scene reloads.
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
    [SerializeField] private string endGameSceneName = "EndGame";
    [SerializeField] private float winDelay = 0.75f;

    [Header("References")]
    [SerializeField] private TMP_Text streakSign;

    [Header("Optional Anti-Cheese")]
    [SerializeField] private bool requireInspectionTrigger;

    [Header("Fade Transition")]
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float initialBlackDelay = 1.5f;
    [SerializeField] private float fadeDuration = 3f;
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
        ResetGameProgress();
    }

    public static void ResetGameProgress()
    {
        Streak = 0;
        firstRoundHasStarted = false;
        initialFadePlayed = false;

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

        PrepareRound();

        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 1f;
            fadeCanvas.blocksRaycasts = true;

            // Only wait on the first Gallery round.
            if (!initialFadePlayed)
            {
                initialFadePlayed = true;

                if (initialBlackDelay > 0f)
                {
                    yield return new WaitForSecondsRealtime(
                        initialBlackDelay
                    );
                }
            }

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
        resolving = false;
        readyForChoice = false;
        inspectionStarted = !requireInspectionTrigger;

        DisableAllAnomalies();

        // The first round of a new game is always normal.
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
            // If no anomaly can be activated, make this a normal round.
            roundHasAnomaly = ActivateNextAnomaly();
        }

        UpdateSign();

        Debug.Log(
            roundHasAnomaly
                ? "ROUND HAS AN ANOMALY"
                : "ROUND IS NORMAL"
        );
    }

    private bool ActivateNextAnomaly()
    {
        if (anomalyVariants == null ||
            anomalyVariants.Length == 0)
        {
            return false;
        }

        // Reset the bag if the Inspector array size changed.
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

        // Avoid repeating the last anomaly when a new bag begins.
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

        if (selectedAnomaly == null)
            return false;

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

        HashSet<GameObject> addedAnomalies =
            new HashSet<GameObject>();

        for (int i = 0; i < anomalyVariants.Length; i++)
        {
            GameObject anomaly = anomalyVariants[i];

            // Nulls and duplicate Inspector entries are ignored.
            if (anomaly != null &&
                addedAnomalies.Add(anomaly))
            {
                remainingAnomalyIndices.Add(i);
            }
        }

        // Shuffle the bag.
        for (int i = remainingAnomalyIndices.Count - 1;
             i > 0;
             i--)
        {
            int randomPosition = Random.Range(0, i + 1);

            int temporary =
                remainingAnomalyIndices[i];

            remainingAnomalyIndices[i] =
                remainingAnomalyIndices[randomPosition];

            remainingAnomalyIndices[randomPosition] =
                temporary;
        }

        // Make sure a new cycle does not begin with the
        // anomaly that ended the previous cycle.
        if (remainingAnomalyIndices.Count > 1 &&
            remainingAnomalyIndices[0] == lastAnomalyIndex)
        {
            int temporary = remainingAnomalyIndices[0];

            remainingAnomalyIndices[0] =
                remainingAnomalyIndices[1];

            remainingAnomalyIndices[1] = temporary;
        }

        Debug.Log(
            "Anomaly bag refilled with " +
            remainingAnomalyIndices.Count +
            " unique anomalies."
        );
    }

    private bool HasValidAnomalies()
    {
        if (anomalyVariants == null)
            return false;

        foreach (GameObject anomaly in anomalyVariants)
        {
            if (anomaly != null)
                return true;
        }

        return false;
    }

    private void DisableAllAnomalies()
    {
        if (anomalyVariants == null)
            return;

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
            StartCoroutine(LoadEndGameScene());
        else
            StartCoroutine(FinishRound());

        return true;
    }

    private IEnumerator LoadEndGameScene()
    {
        gameWon = true;

        // Gives the final door animation/audio time to begin.
        if (winDelay > 0f)
            yield return new WaitForSecondsRealtime(winDelay);

        DisableAllAnomalies();

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

        Time.timeScale = 1f;

        yield return SceneManager.LoadSceneAsync(
            endGameSceneName
        );
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

        int currentSceneIndex =
            SceneManager.GetActiveScene().buildIndex;

        yield return SceneManager.LoadSceneAsync(
            currentSceneIndex
        );
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        ResetGameProgress();

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    private void UpdateSign()
    {
        if (streakSign != null)
            streakSign.text = Streak.ToString();
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