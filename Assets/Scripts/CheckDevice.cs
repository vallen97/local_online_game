using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckDevice : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        //check if device is a Desktop
        if (SystemInfo.deviceType == DeviceType.Desktop) { }
        else
        {
            //lock screen in Landscape
            Screen.orientation = ScreenOrientation.Landscape;
        }
    }

    void Update()
    {
        //Check device type and quit application on device
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            // Exit condition for Desktop devices
            if (Input.GetKey("escape"))
            {
                Application.Quit();
            }
        }
        else
        {
            // Exit condition for mobile devices
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}
