using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PhoneRingAnomaly : MonoBehaviour
{
    [SerializeField] private AudioSource ringSource;
    private bool hasRung;

    private void Reset()
    {
        ringSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        hasRung = false;

        if (ringSource != null)
            ringSource.Stop();
    }

    private void OnDisable()
    {
        if (ringSource != null)
            ringSource.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasRung || !other.transform.root.CompareTag("Player"))
            return;

        hasRung = true;
        ringSource.Play();
    }
}