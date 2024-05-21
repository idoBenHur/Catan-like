using UnityEngine;
using UnityEngine.UI;

public class UIDrawer : MonoBehaviour
{
    public Camera uiCamera; // Assign the camera that renders the UI
    public RawImage[] rawImages; // Assign the RawImage components
    public Color drawColor = Color.black; // Color of the drawing
    public float brushSize = 5.0f; // Size of the brush

    private Texture2D[] textures;
    private RenderTexture[] renderTextures;
    private Vector2? previousPosition = null;
    private int currentImageIndex = -1;

    void Start()
    {
        textures = new Texture2D[rawImages.Length];
        renderTextures = new RenderTexture[rawImages.Length];

        for (int i = 0; i < rawImages.Length; i++)
        {
            // Get the RenderTexture from the RawImage
            renderTextures[i] = rawImages[i].texture as RenderTexture;

            // Create a Texture2D to draw on
            textures[i] = new Texture2D(renderTextures[i].width, renderTextures[i].height, TextureFormat.RGBA32, false);
            ClearTexture(textures[i]);

            // Assign the texture to the RenderTexture
            Graphics.Blit(textures[i], renderTextures[i]);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Determine which RawImage the mouse is over
            currentImageIndex = -1;
            for (int i = 0; i < rawImages.Length; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(rawImages[i].rectTransform, Input.mousePosition, uiCamera))
                {
                    currentImageIndex = i;
                    break;
                }
            }
        }

        if (Input.GetMouseButton(0) && currentImageIndex >= 0)
        {
            Vector2 currentPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImages[currentImageIndex].rectTransform, Input.mousePosition, uiCamera, out currentPosition);

            currentPosition.x = (currentPosition.x + rawImages[currentImageIndex].rectTransform.rect.width * 0.5f) / rawImages[currentImageIndex].rectTransform.rect.width * renderTextures[currentImageIndex].width;
            currentPosition.y = (currentPosition.y + rawImages[currentImageIndex].rectTransform.rect.height * 0.5f) / rawImages[currentImageIndex].rectTransform.rect.height * renderTextures[currentImageIndex].height;

            if (previousPosition == null)
            {
                previousPosition = currentPosition;
            }

            DrawLine(previousPosition.Value, currentPosition, textures[currentImageIndex]);
            previousPosition = currentPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            previousPosition = null;
            currentImageIndex = -1;
        }
    }

    void DrawLine(Vector2 start, Vector2 end, Texture2D texture)
    {
        float distance = Vector2.Distance(start, end);
        int steps = Mathf.CeilToInt(distance);

        for (int i = 0; i < steps; i++)
        {
            float t = (float)i / steps;
            Vector2 position = Vector2.Lerp(start, end, t);
            DrawCircle(position, texture);
        }

        texture.Apply();
        Graphics.Blit(texture, renderTextures[currentImageIndex]);
    }

    void DrawCircle(Vector2 position, Texture2D texture)
    {
        int centerX = Mathf.Clamp((int)position.x, 0, texture.width);
        int centerY = Mathf.Clamp((int)position.y, 0, texture.height);
        int radius = Mathf.CeilToInt(brushSize / 2);

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    int pixelX = centerX + x;
                    int pixelY = centerY + y;

                    if (pixelX >= 0 && pixelX < texture.width && pixelY >= 0 && pixelY < texture.height)
                    {
                        texture.SetPixel(pixelX, pixelY, drawColor);
                    }
                }
            }
        }
    }

    void ClearTexture(Texture2D texture)
    {
        Color32[] colors = new Color32[texture.width * texture.height];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = Color.clear;
        }
        texture.SetPixels32(colors);
        texture.Apply();
    }

    public void ClearDrawing(int imageIndex)
    {
        if (imageIndex >= 0 && imageIndex < textures.Length)
        {
            ClearTexture(textures[imageIndex]);
            Graphics.Blit(textures[imageIndex], renderTextures[imageIndex]);
        }
    }
}
