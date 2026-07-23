using UnityEngine;

public class PaintingMaterialAnomaly : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private int materialIndex = 0;
    [SerializeField] private Material anomalyMaterial;

    private Material normalMaterial;
    private bool initialized;

    private void Awake()
    {
        CacheNormalMaterial();
    }

    private void OnEnable()
    {
        CacheNormalMaterial();
        ApplyMaterial(anomalyMaterial);
    }

    private void OnDisable()
    {
        if (initialized)
            ApplyMaterial(normalMaterial);
    }

    private void CacheNormalMaterial()
    {
        if (initialized || targetRenderer == null)
            return;

        Material[] materials = targetRenderer.sharedMaterials;

        if (materialIndex < 0 || materialIndex >= materials.Length)
        {
            Debug.LogError("Invalid painting material index.", this);
            return;
        }

        normalMaterial = materials[materialIndex];
        initialized = true;
    }

    private void ApplyMaterial(Material newMaterial)
    {
        if (targetRenderer == null || newMaterial == null)
            return;

        Material[] materials = targetRenderer.sharedMaterials;

        if (materialIndex < 0 || materialIndex >= materials.Length)
            return;

        materials[materialIndex] = newMaterial;
        targetRenderer.sharedMaterials = materials;
    }
}