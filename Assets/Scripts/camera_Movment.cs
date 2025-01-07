using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;


public class camera_Movment : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the camera movement
    public float screenEdgeThickness = 10f; // The thickness of the screen edge to detect the mouse
    public float minX = -10f, maxX = 10f; // Threshold limits for X axis movement
    public float minY = -10f, maxY = 10f; // Threshold limits for Y axis movement

    public float zoomSpeed = 2f; // Speed of zooming in and out
    public float minZoom = 5f; // Minimum orthographic size for zoom (zoomed in)
    public float maxZoom = 20f; // Maximum orthographic size for zoom (zoomed out)

    private Vector3 DragOrigin;
    [SerializeField] private Camera MainCamera;
    private bool isDragging = false;


    private float StratzoomInSize = 2f; // Target size for zoom-in
    private float normalSize = 5f; // Default size to zoom out to
    private float duration = 1.5f; // Duration of the animation

    private void Start()
    {



    }


    public void startingzoom()
    {
        MainCamera.orthographicSize = normalSize;

        // Animate the camera to zoom in and then zoom out with a bounce effect
        MainCamera.DOOrthoSize(StratzoomInSize, 0.4f) // Zoom in
            .OnComplete(() => // Once zoomed in
            {
                MainCamera.DOOrthoSize(normalSize, 0.3f) // Zoom out with bounce
                    .SetEase(Ease.OutBounce)
                    .OnComplete(() => // Once zoomed out
                    {
                        DOVirtual.DelayedCall(0.4f, () =>
                        {
                            BoardManager.instance.StartGame(); // Call StartGame method
                        });
                    });
            });
    }


    void Update()
    {
        Vector3 pos = transform.position;

        //// Check if mouse is near the edges of the screen for X axis movement (left and right)
        //if (Input.mousePosition.x >= Screen.width - screenEdgeThickness && pos.x < maxX)
        //{
        //    pos.x += moveSpeed * Time.deltaTime;
        //}
        //if (Input.mousePosition.x <= screenEdgeThickness && pos.x > minX)
        //{
        //    pos.x -= moveSpeed * Time.deltaTime;
        //}

        //// Check if mouse is near the edges of the screen for Y axis movement (up and down)
        //if (Input.mousePosition.y >= Screen.height - screenEdgeThickness && pos.y < maxY)
        //{
        //    pos.y += moveSpeed * Time.deltaTime;
        //}
        //if (Input.mousePosition.y <= screenEdgeThickness && pos.y > minY)
        //{
        //    pos.y -= moveSpeed * Time.deltaTime;
        //}

        // Zooming in and out with the mouse wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            MainCamera.orthographicSize -= scroll * zoomSpeed;
            MainCamera.orthographicSize = Mathf.Clamp(MainCamera.orthographicSize, minZoom, maxZoom);
        }

        // Apply the new position
        transform.position = pos;

        MouseDragCameraMovment();
    }

    private void MouseDragCameraMovment()
    {
        // Check if the pointer is over a UI element only when clicking
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            DragOrigin = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true; // Allow dragging only if we started on a non-UI element
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 difference = DragOrigin - MainCamera.ScreenToWorldPoint(Input.mousePosition);
            MainCamera.transform.position += difference;

            MainCamera.transform.position = new Vector3(
                Mathf.Clamp(MainCamera.transform.position.x, minX, maxX),
                Mathf.Clamp(MainCamera.transform.position.y, minY, maxY),
                MainCamera.transform.position.z);




        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false; // Stop dragging when the mouse button is released
        }
    }
}
