using UnityEngine;

public class PlayerDoorInteractor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 5f;
    [SerializeField] private LayerMask interactionMask = ~0;

    [Header("Tooltip")]
    [SerializeField] private GameObject doorTooltip;

    private ExitDoor lookedAtDoor;

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (doorTooltip != null)
            doorTooltip.SetActive(false);
    }

    private void Update()
    {
        lookedAtDoor = FindDoor();

        bool lookingAtDoor = lookedAtDoor != null;

        if (doorTooltip != null)
            doorTooltip.SetActive(lookingAtDoor);

        if (lookingAtDoor && Input.GetKeyDown(KeyCode.E))
            lookedAtDoor.Interact();
    }

    private ExitDoor FindDoor()
    {
        if (playerCamera == null)
            return null;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactionDistance,
            interactionMask,
            QueryTriggerInteraction.Collide))
        {
            return hit.collider.GetComponentInParent<ExitDoor>();
        }

        return null;
    }

    private void OnDisable()
    {
        if (doorTooltip != null)
            doorTooltip.SetActive(false);
    }
}