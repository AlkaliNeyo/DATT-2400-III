using UnityEngine;
using System.IO;

public class TilemapExporter : MonoBehaviour
{
    public Camera tilemapCamera; // Assign your camera
    public RenderTexture renderTexture; // Assign your RenderTexture
    public string outputFileName = "TilemapExport.png";

    void Start()
    {
        ExportTilemapToPNG();
    }

    public void ExportTilemapToPNG()
    {
        if (renderTexture == null || tilemapCamera == null)
        {
            Debug.LogError("RenderTexture or Camera not assigned.");
            return;
        }

        // Set the camera's target texture
        tilemapCamera.targetTexture = renderTexture;

        // Create a Texture2D to hold the RenderTexture data
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        // Render the camera's view
        tilemapCamera.Render();

        // Copy the RenderTexture data to the Texture2D
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Reset the RenderTexture
        tilemapCamera.targetTexture = null;
        RenderTexture.active = null;

        // Save the Texture2D as a PNG
        byte[] pngData = texture.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, outputFileName);
        File.WriteAllBytes(path, pngData);

        Debug.Log($"Tilemap saved to: {path}");
    }
}
