using System;
using UnityEngine;

public abstract class Connection : MonoBehaviour
{
    public Entry Left
    {
        get;
        set;
    }

    public Entry Right
    {
        get;
        set;
    }

    public LineRenderer LineRenderer
    {
        get;
        set;
    }

    /// <summary>
    /// sets up the position of the linerenderer
    /// </summary>
    public void SetUpPositions()
    {
        this.LineRenderer.SetPosition(0, this.Left.transform.position);
        this.LineRenderer.SetPosition(1, this.Right.transform.position);
    }

    
    void Update()
    {
        this.SetUpPositions();
    }
}
