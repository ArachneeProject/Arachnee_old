using System;
using UnityEngine;

public class FloattingLabel : MonoBehaviour 
{
    public GUIStyle customStyle;
    public float fontSize = 30;
    public float maxDistance = 100;

    public string Text
    {
        get;
        set;
    }

    void OnGUI()
    {
        if (!MiniMath.CanSee(Camera.main.transform, this.transform))
        {
            return;
        }
        
        float distance = Vector3.Distance(this.transform.position,Camera.main.transform.position);
        if (distance > this.maxDistance)
        {
            // camera is too far away
            return;
        }

        this.customStyle.fontSize = (int) (10 * fontSize / Math.Max(1, distance));
        var pos = Camera.main.WorldToScreenPoint(this.transform.position);
        GUI.Label(new Rect(pos.x - 100, Screen.height - pos.y, 200, 30), this.Text, this.customStyle);
    }
}
