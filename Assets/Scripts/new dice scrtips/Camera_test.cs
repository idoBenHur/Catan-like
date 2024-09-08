using UnityEngine;

public class camera_test : MonoBehaviour
{
    public float panSpeed = 20f;         // Speed of camera movement
    public float scrollSpeed = 20f;      // Speed of zooming with the scroll wheel
    public float edgeThreshold = 10f;    // Distance from the edge of the screen to trigger movement

    public Vector2 panLimitMin;          // Minimum boundary for the camera movement
    public Vector2 panLimitMax;          // Maximum boundary for the camera movement
    public float minZoom = 5f;           // Minimum zoom level
    public float maxZoom = 50f;          // Maximum zoom level

    private float fixedZ = -10f;         // Fixed Z position for the camera

    void Update()
    {
        Vector3 pos = transform.position;

        // Pan camera when mouse is at the edge of the screen (X and Y movement)
        if (Input.mousePosition.x >= Screen.width - edgeThreshold)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= edgeThreshold)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y >= Screen.height - edgeThreshold)
        {
            pos.y += panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= edgeThreshold)
        {
            pos.y -= panSpeed * Time.deltaTime;
        }

        // Scroll wheel for zooming (affects orthographic size if using 2D)
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scroll * scrollSpeed * 100f * Time.deltaTime, minZoom, maxZoom);

        // Clamp camera position within the specified boundaries for X and Y
        pos.x = Mathf.Clamp(pos.x, panLimitMin.x, panLimitMax.x);
        pos.y = Mathf.Clamp(pos.y, panLimitMin.y, panLimitMax.y);

        // Keep the Z position fixed at -10
        pos.z = fixedZ;

        // Apply the new camera position
        transform.position = pos;
    }
}
