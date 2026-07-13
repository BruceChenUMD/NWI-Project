using System.Collections;
using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    [Header("Teleport Destination")]
    [SerializeField] private Transform TeleportZoneObject;

    [Header("Player Setup")]
    [Tooltip("Drag First Person Controller here")]
    [SerializeField] private Transform FirstPersonController;

    [Header("Rotation")]
    [Tooltip("-90 turns left, 90 turns right")]
    [SerializeField] private float TurnAmount = -90f;

    [Header("Position Adjustment")]
    [Tooltip("Use a small negative number if the player spawns too high")]
    [SerializeField] private float VerticalOffset = -2f;

    [Header("Cooldown")]
    [SerializeField] private float TeleportCooldown = 0.25f;

    private bool teleporting;

    private void OnTriggerEnter(Collider other)
    {
        if (teleporting)
            return;

        Transform playerRoot = other.transform.root;

        if (!other.CompareTag("Player") &&
            !playerRoot.CompareTag("Player"))
        {
            return;
        }

        if (TeleportZoneObject == null)
        {
            Debug.LogError("Teleport Zone Object is not assigned.", this);
            return;
        }

        if (FirstPersonController == null)
        {
            Debug.LogError("First Person Controller is not assigned.", this);
            return;
        }

        StartCoroutine(TeleportPlayer(playerRoot));
    }

    private IEnumerator TeleportPlayer(Transform playerRoot)
    {
        teleporting = true;

        // Rotate the whole player around the actual controller position.
        playerRoot.RotateAround(
            FirstPersonController.position,
            Vector3.up,
            TurnAmount
        );

        // Move the root by the amount needed to place the controller
        // exactly at the teleport destination.
        Vector3 destination = TeleportZoneObject.position;

        // Move to the bottom of the cube instead of its center
        destination.y -= TeleportZoneObject.localScale.y / 2f;

        Vector3 movement =
            destination - FirstPersonController.position;

        playerRoot.position += movement;

        yield return new WaitForSeconds(TeleportCooldown);

        teleporting = false;
    }
}