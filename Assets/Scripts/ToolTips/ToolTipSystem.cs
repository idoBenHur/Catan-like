using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipSystem : MonoBehaviour
{

    private static ToolTipSystem instance;
    public Tooltips tooltip;

    public void Awake()
    {
        instance = this; 
    }

    public static void Show(string text, string header = "")
    {
        instance.tooltip.SetText(text, header);
        instance.tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        instance.tooltip.gameObject.SetActive(false);

    }
}
