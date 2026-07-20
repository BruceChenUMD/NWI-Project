using UnityEngine;
using EasyDoorSystem;

public class DoorInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionDistance = 3f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteractWithDoor();
        }
    }

    private void TryInteractWithDoor()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance))
        {
            EasyDoor door = hit.collider.GetComponentInParent<EasyDoor>();

            if (door != null)
            {
                door.ToggleDoor();
            }
        }
    }
}