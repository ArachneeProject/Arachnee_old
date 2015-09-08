using UnityEngine;
using System.Collections;

public class GUIEntry : MonoBehaviour 
{
    public GUIStyle customStyle;
    public float fontSize = 30;
    public float maxDistance = 100;

    public string text
    {
        get;
        set;
    }


    void OnGUI()
    {
        float distance = Vector3.Distance(this.transform.position,Camera.main.transform.position);
        if (distance > this.maxDistance)
        {
            return;
        }

        this.customStyle.fontSize = (int) (10 * fontSize / (1+distance));
        var p = Camera.main.WorldToScreenPoint(this.transform.position);
        GUI.Label(new Rect(p.x - 100, Screen.height - p.y, 200, 30), this.text, this.customStyle);
    }

    
}
