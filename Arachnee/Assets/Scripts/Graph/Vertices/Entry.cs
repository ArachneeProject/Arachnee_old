using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Entry : MonoBehaviour
{
    public GUIEntry GUI;

    public Rigidbody RigidBody
    {
        get;
        set;
    }

    public uint GraphId
    {
        get;
        set;
    }

    public Int64 DatabaseId
    {
        get;
        set;
    }

    public string PosterPath
    {
        get;
        set;
    }

    /// <summary>
    /// Init
    /// </summary>
    void Start()
    {
        // init the label
        this.GUI = this.GetComponentInChildren<GUIEntry>();
        this.GUI.text = this.ToString();
        this.GUI.gameObject.SetActive(false);

        // load the texture
        /*
        var loadedTexture = Resources.Load(this.ToString()) as Texture;
        if (loadedTexture == null)
        {
            loadedTexture = Resources.Load("default") as Texture;
        }
        this.GetComponent<Renderer>().material.mainTexture = loadedTexture;
        */
        this.GetComponent<Renderer>().material.mainTexture = Resources.Load("default") as Texture;

        // init the rigidbody
        this.RigidBody = this.GetComponent<Rigidbody>();
            
    }

    /// <summary>
    /// show the label when visible
    /// </summary>
    void OnBecameVisible()
    {
        this.GUI.gameObject.SetActive(true);
    }

    /// <summary>
    /// mask the label when not visible
    /// </summary>
    void OnBecameInvisible()
    {
        this.GUI.gameObject.SetActive(false);
    }

}
