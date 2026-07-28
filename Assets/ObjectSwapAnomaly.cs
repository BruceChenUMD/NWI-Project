using UnityEngine;

public class ObjectSwapAnomaly : MonoBehaviour
{
    [System.Serializable]
    public class ObjectSwap
    {
        public GameObject normalObject;
        public GameObject anomalyObject;
    }

    [SerializeField] private ObjectSwap[] objectSwaps;

    private void OnEnable()
    {
        foreach (ObjectSwap swap in objectSwaps)
        {
            if (swap.normalObject != null)
                swap.normalObject.SetActive(false);

            if (swap.anomalyObject != null)
                swap.anomalyObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        foreach (ObjectSwap swap in objectSwaps)
        {
            if (swap.normalObject != null)
                swap.normalObject.SetActive(true);

            if (swap.anomalyObject != null)
                swap.anomalyObject.SetActive(false);
        }
    }
}