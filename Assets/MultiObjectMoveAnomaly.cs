using UnityEngine;

public class MultiObjectMoveAnomaly : MonoBehaviour
{
    [System.Serializable]
    public class MovingObject
    {
        public Transform objectToMove;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;

        [HideInInspector] public Vector3 originalPosition;
        [HideInInspector] public Quaternion originalRotation;
    }

    [SerializeField] private MovingObject[] objectsToMove;

    private bool positionsSaved;

    public void Initialize()
    {
        if (positionsSaved || objectsToMove == null)
            return;

        foreach (MovingObject item in objectsToMove)
        {
            if (item.objectToMove == null)
                continue;

            item.originalPosition =
                item.objectToMove.localPosition;

            item.originalRotation =
                item.objectToMove.localRotation;
        }

        positionsSaved = true;
    }

    public void ApplyAnomaly()
    {
        Initialize();

        foreach (MovingObject item in objectsToMove)
        {
            if (item.objectToMove == null)
                continue;

            item.objectToMove.localPosition =
                item.originalPosition + item.positionOffset;

            item.objectToMove.localRotation =
                item.originalRotation *
                Quaternion.Euler(item.rotationOffset);
        }
    }

    public void RestoreNormal()
    {
        if (!positionsSaved || objectsToMove == null)
            return;

        foreach (MovingObject item in objectsToMove)
        {
            if (item.objectToMove == null)
                continue;

            item.objectToMove.localPosition =
                item.originalPosition;

            item.objectToMove.localRotation =
                item.originalRotation;
        }
    }

    private void OnDisable()
    {
        RestoreNormal();
    }
}