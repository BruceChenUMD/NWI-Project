using UnityEngine;

public class HallwayDoor : MonoBehaviour
{
    [SerializeField] private DoorType doorType;

    private bool playerNearby;

    private void Update()
    {
        if (!playerNearby)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseDoor();
        }
    }

    private void UseDoor()
    {
        GameManager.Instance.ChooseDoor(doorType);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}