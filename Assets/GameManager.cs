using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Round")]
    [SerializeField] private bool hasAnomaly;
    [SerializeField, Range(0f, 1f)]
    private float anomalyChance = 0.5f;

    [Header("Score")]
    [SerializeField] private int streak = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text streakText;

    [Header("Player Reset")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform spawnPoint;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartNewRound();
    }

    public void ChooseDoor(DoorType doorType)
    {
        bool playerSaysAnomaly =
            doorType == DoorType.StartDoor;

        bool correct =
            playerSaysAnomaly == hasAnomaly;

        if (correct)
        {
            streak++;
            Debug.Log("Correct!");
        }
        else
        {
            streak = 0;
            Debug.Log("Wrong!");
        }

        UpdateStreakDisplay();

        StartNewRound();
    }

    private void StartNewRound()
    {
        // Decide whether this round has an anomaly.
        hasAnomaly = Random.value < anomalyChance;

        Debug.Log(
            hasAnomaly
                ? "ROUND HAS AN ANOMALY"
                : "ROUND IS NORMAL"
        );

        // Later:
        // anomalyManager.GenerateRound(hasAnomaly);

        ResetPlayer();
    }

    private void ResetPlayer()
    {
        CharacterController controller =
            player.GetComponent<CharacterController>();

        if (controller != null)
            controller.enabled = false;

        player.position = spawnPoint.position;
        player.rotation = spawnPoint.rotation;

        if (controller != null)
            controller.enabled = true;
    }

    private void UpdateStreakDisplay()
    {
        if (streakText != null)
            streakText.text = streak.ToString();
    }
}

public enum DoorType
{
    StartDoor,
    EndDoor
}