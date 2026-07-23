using UnityEngine;

public class SinkObjectsAnomaly : MonoBehaviour
{
    [SerializeField] private Transform[] objectsToMove;

    [Header("Position Change")]
    [SerializeField] private Vector3 moveOffset;

    [Header("Rotation Change")]
    [SerializeField] private Vector3 rotationOffset;

    private Vector3[] normalPositions;
    private Quaternion[] normalRotations;
    private bool initialized;

    private void Awake()
    {
        CacheNormalTransforms();
    }

    private void OnEnable()
    {
        CacheNormalTransforms();

        for (int i = 0; i < objectsToMove.Length; i++)
        {
            if (objectsToMove[i] == null)
                continue;

            objectsToMove[i].position =
                normalPositions[i] + moveOffset;

            objectsToMove[i].rotation =
                normalRotations[i] *
                Quaternion.Euler(rotationOffset);
        }
    }

    private void OnDisable()
    {
        if (!initialized)
            return;

        for (int i = 0; i < objectsToMove.Length; i++)
        {
            if (objectsToMove[i] == null)
                continue;

            objectsToMove[i].position = normalPositions[i];
            objectsToMove[i].rotation = normalRotations[i];
        }
    }

    private void CacheNormalTransforms()
    {
        if (initialized)
            return;

        normalPositions = new Vector3[objectsToMove.Length];
        normalRotations = new Quaternion[objectsToMove.Length];

        for (int i = 0; i < objectsToMove.Length; i++)
        {
            if (objectsToMove[i] == null)
                continue;

            normalPositions[i] = objectsToMove[i].position;
            normalRotations[i] = objectsToMove[i].rotation;
        }

        initialized = true;
    }
}