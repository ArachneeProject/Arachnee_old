using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class ObjectLabel : MonoBehaviour
{
    public Transform target;  // Object that this label should follow
    private GUIText text; // text displayed

    private float maxDistance = 100;

    //public float sizeCoeffOnHighlight = 20;
    //private float savedSize = 1;
    private Transform kamera;

    void Start()
    {
        this.text = this.gameObject.GetComponent<GUIText>();
        this.gameObject.SetActive(false);
        this.kamera = Camera.main.transform;
    }
    
    void Update()
    {
        if (this.target == null)
        {
            return;
        }
        float distance = Vector3.Distance(target.position, kamera.position);

        if (distance > this.maxDistance)
        {
            this.text.fontSize = 1;
        }
        else
        {
            //this.text.fontSize = (int)((200F + this.fontSize) / (1F + Vector3.Distance(target.transform.position, kamera.position)));
            this.text.fontSize = (int)(3F / Vector3.Distance(target.transform.position, kamera.position));
            this.transform.position = Camera.main.WorldToViewportPoint(target.position - 4 * Vector3.up);
        }
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