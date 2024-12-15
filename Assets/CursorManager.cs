using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }



    [SerializeField] private Texture2D pointingFinger;
    [SerializeField] private Texture2D ClickingFinger;
    [SerializeField] private Texture2D OpenHand;
    [SerializeField] private Texture2D CloseHand;

    private CursorControl Control;

    private Vector2 hotSpot = Vector2.zero; // Adjust this if the cursor hotspot is not at the top-left
    private bool isDragging = false;
   

    public enum CursorType
    {
        Pointing,
        Clicking,
        OpenHand,
        CloseHand
    }



    private void Awake()
    {



        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Control = new CursorControl();


    }


    // using input system part"

    private void OnEnable()
    {
        Control.Enable();
    }

    private void OnDisable()
    {
        Control.Disable();
    }

    private void Start()
    {
        Control.Mouse.Click.started += _ => StartedClick();
        Control.Mouse.Click.performed += _ => EndedClick();
    }


    // The transition to the clicking Finger is managed by the input system and is separate from cursor changes based on hovering or dragging.
    // Returning to the pointing Finger occurs when the click action is finished BUT also when the player "unhovers" over a die.(logic for that is further down the script)

    private void StartedClick()
    {
        Cursor.SetCursor(ClickingFinger, hotSpot, CursorMode.Auto);
    }

    private void EndedClick()
    {
        Cursor.SetCursor(pointingFinger, hotSpot, CursorMode.Auto);
    }



    public void UpdateDragging(bool drag)
    {
        isDragging = drag;
    }



    public void SetCursor(CursorType cursor)
    {


        switch (cursor)
        {          


            // called only when hoverd over a die
            case CursorType.OpenHand:

                if (isDragging == false)
                {
                    Cursor.SetCursor(OpenHand, hotSpot, CursorMode.Auto); Cursor.SetCursor(OpenHand, hotSpot, CursorMode.Auto);
                }             
                break;


            // called only when dragging a die
            case CursorType.CloseHand: 
                Cursor.SetCursor(CloseHand, hotSpot, CursorMode.Auto);
                break;


            // called when hovering a die (BUT also at the end of a "cliking" action)
            case CursorType.Pointing: 
                
                if(isDragging == false)
                {
                    Cursor.SetCursor(pointingFinger, hotSpot, CursorMode.Auto);
                }
                                           
                break;



        }




    }


}
