using UnityEngine;

public abstract class Entry : MonoBehaviour
{
    // id
    public int Id
    {
        get;
        set;
    }

    // label of the entry
    public ObjectLabel label 
    { 
        get;
        set;
    }

    // quad on which the texture is displayed
    private GameObject quad;

    // true if the entry is visible
    private bool isVisible = false;

    // max distance of visibility of entries
    private int maxDistance = 100;

    // main camera
    private Camera kamera;

    // true if the user clicked on this entry
    private bool isSelected = false;


    

    /// <summary>
    /// setup the image displayed in this entry
    /// </summary>
    void Start()
    {
        Texture text = (Texture) Resources.Load("" + this.Id);
        this.quad = this.transform.GetChild(0).gameObject;
        this.quad.GetComponent<Renderer>().material.mainTexture = text;
        this.kamera = Camera.main;
    }

    /// <summary>
    /// highlights the movie and its connexions
    /// </summary>
    void OnMouseEnter()
    {
        this.goOnGUI();        
    }

    public abstract void goOnGUI();

    /// <summary>
    ///  stop highlighting and mask connexions
    /// </summary>
    void OnMouseExit()
    {
        CameraGUI.Principal = "";
        CameraGUI.Date = "";
    }

    /// <summary>
    /// highlight or mask the edges connected to the entry
    /// </summary>
    void OnMouseDown()
    {
        if (!isSelected)
        {
            Graph.highlightEdges(this.Id);
            AudioManager.manager.playOnceAt(0, this.transform.position);
            this.isSelected = true;
        }
        else
        {
            this.isSelected = false;
            Graph.maskEdges(this.Id);
        }
        
    }

    /// <summary>
    /// update the position of the label
    /// </summary>
    void Update()
    {
        float distance = Vector3.Distance(this.transform.position, kamera.transform.position);
        if (distance > this.maxDistance || !this.isVisible)
        {
            this.label.gameObject.SetActive(false);
        }
        else
        {
            this.label.gameObject.SetActive(true);
            this.label.transform.position = this.kamera.WorldToViewportPoint(this.transform.position - 4 * Vector3.up);
            this.label.updateFontSize(distance);
        }
        if (distance > 2 * this.maxDistance)
        {
            this.quad.SetActive(false);
        }
        else
        {
            this.quad.SetActive(true);
        }
    }

   
    /// <summary>
    /// set the visible field to true
    /// </summary>
    void OnBecameVisible()
    {
        this.isVisible = true;
    }

    /// <summary>
    /// set the visible field to false
    /// </summary>
    void OnBecameInvisible()
    {
        this.isVisible = false;
    }
}

