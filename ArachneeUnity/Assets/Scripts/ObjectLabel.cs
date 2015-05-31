using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUIText))]
public class ObjectLabel : MonoBehaviour
{
    public float fontSize = 100;
    public Transform target;  // Object that this label should follow
    private GUIText text; // text displayed

    void Start()
    {
        this.text = this.gameObject.GetComponent<GUIText>();
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if (this.target == null)
        {
            return;
        }
        this.transform.position = Camera.main.WorldToViewportPoint(target.position + Vector3.up);
        this.text.fontSize = (int) (this.fontSize / (1F + Vector3.Distance(target.transform.position, Camera.main.transform.position)));
        if (this.text.fontSize == 0)
        {
            this.text.fontSize = 1;
        }
    }

    public void setHighlight(bool highlighted)
    {
        if (highlighted)
        {
            this.fontSize *= 2;
        }
        else
        {
            this.fontSize /= 2;
        }

    }
}