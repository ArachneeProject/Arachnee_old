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


    // true if the user clicked on this entry
    private bool isSelected = false;
    
    void Start()
    {
        Texture text = (Texture) Resources.Load("" + this.Id);
        this.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = text;
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

    void OnBecameVisible()
    {
        this.label.gameObject.SetActive(true);
    }

    void OnBecameInvisible()
    {
        if (this.label == null)
        {
            return;
        }
        this.label.gameObject.SetActive(false);
    }
}

