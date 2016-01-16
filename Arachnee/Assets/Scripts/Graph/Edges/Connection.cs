using System;
using UnityEngine;

public abstract class Connection : MonoBehaviour
{
    public uint InitLeftId { get; set; }
    public uint InitRightId { get; set; }

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

    
    void Update()
    {
        //if (this.Left != null && this.Right != null)
        //{
            this.LineRenderer.SetPosition(0, this.Left.transform.position);
            this.LineRenderer.SetPosition(1, this.Right.transform.position);
        //}
    }

    /// <summary>
    /// Set up the two entries from the existing entries of the graph
    /// </summary>
    /// <param name="valueCollection"></param>
    internal abstract void InitEntries(System.Collections.Generic.List<Entry> valueCollection);


    public override int GetHashCode()
    {
        return this.InitLeftId.GetHashCode() + this.InitRightId.GetHashCode();
    }

    public override bool Equals(object o)
    {
        if (o is Connection)
        {
            var otherConnec = (Connection)o;
            return this.InitLeftId == otherConnec.InitLeftId && this.InitRightId == otherConnec.InitRightId;
        }
        return false;
    }
}
