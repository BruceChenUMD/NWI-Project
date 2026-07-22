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

    [Header("Round")]
    [Range(0f, 1f)]
    [SerializeField] private float anomalyChance = 0.5f;
    [SerializeField] private GameObject[] anomalyVariants;

    [Header("References")]
    [SerializeField] private TMP_Text streakSign;

    [Header("Optional anti-cheese")]
    [SerializeField] private bool requireInspectionTrigger = true;

    [Header("Transition")]
    [SerializeField] private float reloadDelay = 0.75f;

    private bool roundHasAnomaly;
    private bool inspectionStarted;
    private bool resolving;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PrepareRound();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void PrepareRound()
    {
        inspectionStarted = !requireInspectionTrigger;

        foreach (GameObject anomaly in anomalyVariants)
        {
            if (anomaly != null)
                anomaly.SetActive(false);
        }

        roundHasAnomaly =
            anomalyVariants.Length > 0 &&
            Random.value < anomalyChance;

        if (roundHasAnomaly)
        {
            int index = Random.Range(0, anomalyVariants.Length);
            anomalyVariants[index].SetActive(true);
        }

        UpdateSign();
    }

    public void BeginInspection()
    {
        inspectionStarted = true;
    }

    public bool SubmitChoice(ExitChoice choice)
    {
        if (resolving || !inspectionStarted)
            return false;

        resolving = true;

        bool playerReportedAnomaly = choice == ExitChoice.SawAnomaly;
        bool correct = playerReportedAnomaly == roundHasAnomaly;

        Streak = correct ? Streak + 1 : 0;
        UpdateSign();

        StartCoroutine(ReloadRound());
        return true;
    }

    private void UpdateSign()
    {
        if (streakSign != null)
            streakSign.text = Streak.ToString();
    }

    private IEnumerator ReloadRound()
    {
        yield return new WaitForSeconds(reloadDelay);

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        yield return SceneManager.LoadSceneAsync(sceneIndex);
    }
}