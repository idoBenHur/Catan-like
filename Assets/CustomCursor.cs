using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    public Texture2D handPointing;
    public Texture2D handPressing;

    private CursorControl Controls;
    public Vector2 hotspot;

    private void Awake()
    {
        Controls = new CursorControl();
        ChangeCursos(handPointing);
        Cursor.lockState = CursorLockMode.Confined;
        
    }

    private void OnEnable()
    {
        Controls.Enable();
    }

    private void OnDisable()
    {
        Controls.Disable();
    }

    private void Start()
    {
        Controls.Mouse.Click.started += _ => StartedClick();
        Controls.Mouse.Click.performed += _ => EndedClick();
    }



    private void StartedClick()
    {
        ChangeCursos(handPressing);
    }

    private void EndedClick()
    {
        ChangeCursos(handPointing);
    }


    private void ChangeCursos(Texture2D cursorType)
    {
      
        Cursor.SetCursor(cursorType, hotspot, CursorMode.Auto);
    }


}
