using UnityEngine;

public class ExitDoor : MonoBehaviour
{
    [SerializeField] private ExitChoice choice;
    [SerializeField] private Animator doorAnimator;

    public void Interact()
    {
        HallwayRoundManager manager = HallwayRoundManager.Instance;

        if (manager != null && manager.SubmitChoice(choice))
        {
            if (doorAnimator != null)
                doorAnimator.SetTrigger("Open");
        }
    }
}