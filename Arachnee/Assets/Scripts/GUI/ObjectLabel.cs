using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class ObjectLabel : MonoBehaviour
{
    public Transform target;  // Object that this label should follow
    private GUIText text; // text displayed

    //public float sizeCoeffOnHighlight = 20;
    //private float savedSize = 1;

    void Start()
    {
        this.text = this.gameObject.GetComponent<GUIText>();
        this.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// update the size of the label
    /// </summary>
    /// <param name="distance"></param>
    public void updateFontSize(float distance)
    {
        this.text.fontSize = (int)(3F / distance);
    }
     

    /*
    public void setHighlight(bool highlighted)
    {
        if (highlighted)
        { 
            this.savedSize = this.fontSize;
            this.fontSize = this.sizeCoeffOnHighlight * Vector3.Distance(target.transform.position, Camera.main.transform.position);
        }
        else
        {
            this.fontSize = this.savedSize;
        }

    }
     * */
}