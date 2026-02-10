using UnityEngine;

public class PlacementGhost : MonoBehaviour
{
    public Material validMat;   // Assign a green transparent material
    public Material invalidMat; // Assign a red transparent material
    private MeshRenderer[] renderers;
    private Material[][] originalMaterials;
    private int overlapCount = 0;
    private bool isPlaced = false;

    void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalMaterials[i] = renderers[i].sharedMaterials;
        }
    }

    public bool CanPlace() => overlapCount == 0;

    void Update()
    {
        if (isPlaced)
        {
            return;
        }

        if (validMat != null && invalidMat != null)
        {
            Material targetMat = CanPlace() ? validMat : invalidMat;
            ApplyPreviewMaterial(targetMat);
        }
        else
        {
            Color targetColor = CanPlace() ? Color.green : Color.red;
            foreach (var r in renderers) r.material.color = targetColor;
        }
    }

    public void SetAsPlaced()
    {
        isPlaced = true;
        RestoreOriginalMaterials();
        enabled = false;
    }

    private void ApplyPreviewMaterial(Material mat)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] mats = renderers[i].sharedMaterials;
            for (int j = 0; j < mats.Length; j++)
            {
                mats[j] = mat;
            }
            renderers[i].sharedMaterials = mats;
        }
    }

    private void RestoreOriginalMaterials()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterials = originalMaterials[i];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlaced) overlapCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPlaced) overlapCount--;
    }
}