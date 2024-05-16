using UnityEngine;
using UnityEngine.UI;

//[RequireComponent(typeof(Slider))]
public class CircularSliderWithTicks : MonoBehaviour
{
    public Slider slider;
    public GameObject tickPrefab;
    public Transform ticksParent;

    public int maxTicks = 5;

    void Start()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }
        UpdateTicks();
    }

    void Update()
    {
        // Example to update the slider. Replace this with your actual logic.
        //slider.value = Mathf.PingPong(Time.time, slider.maxValue);

        //// For testing, change the maxTicks with keys 1 to 9
        //if (Input.GetKeyDown(KeyCode.Alpha1)) { SetMaxTicks(1); }
        //if (Input.GetKeyDown(KeyCode.Alpha2)) { SetMaxTicks(2); }
        //if (Input.GetKeyDown(KeyCode.Alpha3)) { SetMaxTicks(3); }
        //if (Input.GetKeyDown(KeyCode.Alpha4)) { SetMaxTicks(4); }
        //if (Input.GetKeyDown(KeyCode.Alpha5)) { SetMaxTicks(5); }
        //if (Input.GetKeyDown(KeyCode.Alpha6)) { SetMaxTicks(6); }
        //if (Input.GetKeyDown(KeyCode.Alpha7)) { SetMaxTicks(7); }
        //if (Input.GetKeyDown(KeyCode.Alpha8)) { SetMaxTicks(8); }
        //if (Input.GetKeyDown(KeyCode.Alpha9)) { SetMaxTicks(9); }
        UpdateTicks();
    }

    public void SetMaxTicks(int max)
    {
        maxTicks = max;
        UpdateTicks();
    }

    private void UpdateTicks()
    {
        // Remove existing ticks
        foreach (Transform child in ticksParent)
        {
            Destroy(child.gameObject);
        }

        // Create new ticks
        for (int i = 0; i < maxTicks; i++)
        {
            float angle = 360f / maxTicks * i;
            GameObject tick = Instantiate(tickPrefab, ticksParent);
            tick.transform.localPosition = Vector3.zero;
            tick.transform.localRotation = Quaternion.Euler(0, 0, -angle);
        }
    }
}
