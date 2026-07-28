using UnityEngine;

public class FlashlightViewLock : MonoBehaviour
{
    [SerializeField] private Vector3 fixedPosition =
        new Vector3(0.25f, -0.22f, 0.45f);

    [SerializeField] private Vector3 fixedRotation = Vector3.zero;

    private void LateUpdate()
    {
        transform.localPosition = fixedPosition;
        transform.localRotation = Quaternion.Euler(fixedRotation);
    }
}