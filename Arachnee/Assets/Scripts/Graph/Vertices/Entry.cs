using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class Entry : MonoBehaviour
{
    public GUIEntry GUI;

    public uint Id
    {
        get;
        set;
    }

    void Start()
    {
        this.GUI = this.GetComponentInChildren<GUIEntry>();
        this.GUI.text = this.ToString();

        this.GetComponent<Renderer>().material.mainTexture = Resources.Load(this.ToString()) as Texture;
    }

    void OnBecameVisible()
    {
        this.GUI.gameObject.SetActive(true);
    }

    void OnBecameInvisible()
    {
        this.GUI.gameObject.SetActive(false);
    }
}
