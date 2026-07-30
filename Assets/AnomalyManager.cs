using System.Collections.Generic;
using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    [SerializeField] private GameObject[] anomalyObjects;

    private readonly List<GameObject> remainingAnomalies =
        new List<GameObject>();

    private GameObject activeAnomaly;
    private GameObject lastAnomaly;

    private void Awake()
    {
        InitializeAndDisableAll();
        RefillAnomalyBag();
    }

    public void GenerateRound(bool shouldHaveAnomaly)
    {
        DisableAllAnomalies();

        if (!shouldHaveAnomaly)
        {
            Debug.Log("No anomaly activated.");
            return;
        }

        if (remainingAnomalies.Count == 0)
            RefillAnomalyBag();

        if (remainingAnomalies.Count == 0)
        {
            Debug.LogWarning("No anomalies are assigned.");
            return;
        }

        int randomIndex =
            Random.Range(0, remainingAnomalies.Count);

        // Avoid immediately repeating the final anomaly
        // when a new cycle begins.
        if (remainingAnomalies.Count > 1 &&
            remainingAnomalies[randomIndex] == lastAnomaly)
        {
            randomIndex =
                (randomIndex + 1) %
                remainingAnomalies.Count;
        }

        activeAnomaly = remainingAnomalies[randomIndex];
        remainingAnomalies.RemoveAt(randomIndex);

        activeAnomaly.SetActive(true);

        MultiObjectMoveAnomaly[] movers =
            activeAnomaly.GetComponentsInChildren
            <MultiObjectMoveAnomaly>(true);

        foreach (MultiObjectMoveAnomaly mover in movers)
            mover.ApplyAnomaly();

        lastAnomaly = activeAnomaly;

        Debug.Log(
            "Activated anomaly: " +
            activeAnomaly.name
        );
    }

    private void RefillAnomalyBag()
    {
        remainingAnomalies.Clear();

        foreach (GameObject anomaly in anomalyObjects)
        {
            if (anomaly != null &&
                !remainingAnomalies.Contains(anomaly))
            {
                remainingAnomalies.Add(anomaly);
            }
        }

        Debug.Log(
            "Anomaly list refilled. Count: " +
            remainingAnomalies.Count
        );
    }

    private void InitializeAndDisableAll()
    {
        foreach (GameObject anomaly in anomalyObjects)
        {
            if (anomaly == null)
                continue;

            MultiObjectMoveAnomaly[] movers =
                anomaly.GetComponentsInChildren
                <MultiObjectMoveAnomaly>(true);

            foreach (MultiObjectMoveAnomaly mover in movers)
            {
                mover.Initialize();
                mover.RestoreNormal();
            }

            anomaly.SetActive(false);
        }
    }

    private void DisableAllAnomalies()
    {
        foreach (GameObject anomaly in anomalyObjects)
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

        activeAnomaly = null;
    }
}