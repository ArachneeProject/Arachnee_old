using UnityEngine;
using System.Collections;

public class CameraGUI : MonoBehaviour 
{
    public static string Principal
    {
        private get;
        set;
    }

    public static string Date
    {
        private get;
        set;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, Screen.width, 20), Principal);
        GUI.Label(new Rect(10, 40, Screen.width, 20), Date);
    }

    
}
