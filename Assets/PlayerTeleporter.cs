using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private Transform destination;

    [Header("Player")]
    [SerializeField] private Transform firstPersonController;

    [Header("Settings")]
    [SerializeField] private bool matchRotation = true;
    [SerializeField] private float cooldown = 0.25f;

    [Header("Grounding")]
    [SerializeField] private LayerMask groundLayers = ~0;
    [SerializeField] private float rayStartHeight = 3f;
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private float groundClearance = 0.03f;

    private CharacterController characterController;
    private float lastTeleportTime = -999f;

    private void Awake()
    {
        if (firstPersonController != null)
        {
            characterController =
                firstPersonController.GetComponent<CharacterController>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Time.time < lastTeleportTime + cooldown)
            return;

        if (firstPersonController == null || destination == null)
            return;

        bool isPlayer =
            other.transform == firstPersonController ||
            other.transform.IsChildOf(firstPersonController);

        if (!isPlayer)
            return;

        TeleportPlayer();
    }

    private void TeleportPlayer()
    {
        lastTeleportTime = Time.time;

        // Get the player's position relative to the entry trigger.
        Vector3 localPosition =
            transform.InverseTransformPoint(
                firstPersonController.position
            );

        // Preserve the corresponding horizontal position.
        Vector3 newPosition =
            destination.TransformPoint(localPosition);

        // Important:
        // Don't trust the transformed Y position.
        // Find the actual destination floor instead.
        newPosition = SnapToGround(newPosition);

        Quaternion newRotation =
            firstPersonController.rotation;

        if (matchRotation)
        {
            Quaternion relativeRotation =
                Quaternion.Inverse(transform.rotation) *
                firstPersonController.rotation;

            newRotation =
                destination.rotation *
                relativeRotation;
        }

        // Disable CharacterController before changing transform directly.
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        firstPersonController.position = newPosition;
        firstPersonController.rotation = newRotation;

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }

    private Vector3 SnapToGround(Vector3 targetPosition)
    {
        Vector3 rayOrigin =
            targetPosition +
            Vector3.up * rayStartHeight;

        float totalRayDistance =
            rayStartHeight + rayDistance;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out RaycastHit hit,
            totalRayDistance,
            groundLayers,
            QueryTriggerInteraction.Ignore))
        {
            float transformToFeet = 0f;

            if (characterController != null)
            {
                // Calculates how far the player's transform origin
                // is above the bottom of the CharacterController.
                transformToFeet =
                    (characterController.height * 0.5f)
                    - characterController.center.y;
            }

            targetPosition.y =
                hit.point.y +
                transformToFeet +
                groundClearance;
        }
        else
        {
            Debug.LogWarning(
                "PlayerTeleporter: No floor found at destination.",
                this
            );
        }

        return targetPosition;
    }
}