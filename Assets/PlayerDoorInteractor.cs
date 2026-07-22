using UnityEngine;

public class PlayerDoorInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 2.5f;
    [SerializeField] private LayerMask interactionMask = ~0;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E))
            return;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactionDistance,
            interactionMask,
            QueryTriggerInteraction.Ignore))
        {
            hit.collider
                .GetComponentInParent<ExitDoor>()
                ?.Interact();
        }
    }
}