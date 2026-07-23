using UnityEngine;

public class PlayerDoorInteractor : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 5f;
    [SerializeField] private LayerMask interactionMask = ~0;

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E))
            return;

        Debug.Log("E pressed");

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactionDistance,
            interactionMask,
            QueryTriggerInteraction.Collide))
        {
            Debug.LogWarning("The interaction ray hit nothing.");
            return;
        }

        Debug.Log("Interaction ray hit: " + hit.collider.name);

        ExitDoor door =
            hit.collider.GetComponentInParent<ExitDoor>();

        if (door == null)
        {
            Debug.LogWarning(
                hit.collider.name + " does not have ExitDoor on it or its parent."
            );
            return;
        }

        door.Interact();
    }
}