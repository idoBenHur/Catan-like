using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCursor : MonoBehaviour
{
    // Start is called before the first frame update

    private SpriteRenderer spriteRenderer;
    public Sprite PointingHand;
    public Sprite PressingHand;

    public Sprite WrtinigHand;
    private bool IsWriting = false;

    public Vector2 StartingHotSpot;
    private Vector2 CurrentHotSpot;

    


    void Awake()
    {
        Cursor.visible = false;
        spriteRenderer = GetComponent<SpriteRenderer>();

        CurrentHotSpot = StartingHotSpot;

    }





    // Update is called once per frame
    void Update()
    {
        
       // Cursor.SetCursor(null, hotspot, CursorMode.Auto);
        Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        transform.position = cursorPos - CurrentHotSpot;

        if (IsWriting == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                spriteRenderer.sprite = PressingHand;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                spriteRenderer.sprite = PointingHand;
            }
        }
        else if (IsWriting == true) 
        {
            spriteRenderer.sprite = WrtinigHand;
            
        }


    }


    public void ChangeCursorHand()
    {
        IsWriting = (!IsWriting);



        if (IsWriting == true) 
        {
            CurrentHotSpot = new Vector2(-2.33F, 2.04F);
            transform.localScale = new Vector3(0.43f, 0.43f, 0.43f);
        }

        else if(IsWriting == false)
        {
            CurrentHotSpot = new Vector2(-0.3F, 0.6F);
            transform.localScale = new Vector3(0.10728f, 0.10728f, 0.10728f);
            spriteRenderer.sprite = PointingHand;
        }

    }
}
