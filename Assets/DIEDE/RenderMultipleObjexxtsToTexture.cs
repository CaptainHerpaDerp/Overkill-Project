using UnityEngine;

public class RenderMultipleObjectsToTexture : MonoBehaviour
{
    public Camera renderCamera;
    public Material normalMapMaterial;
    public Material baseColorMapMaterial;
    public RenderTexture normalMapRT;
    public RenderTexture baseColorMapRT;
    public GameObject[] objectsToRender;

    void Start()
    {
        RenderObjectsToTexture(normalMapMaterial, normalMapRT);
        RenderObjectsToTexture(baseColorMapMaterial, baseColorMapRT);
    }

    void RenderObjectsToTexture(Material material, RenderTexture renderTexture)
    {
        foreach (GameObject obj in objectsToRender)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            Material[] originalMaterials = renderer.materials;

            renderer.material = material;

            renderCamera.targetTexture = renderTexture;
            renderCamera.Render();
            renderCamera.targetTexture = null;

            renderer.materials = originalMaterials; // Restore original materials
        }
    }
}