using System;
using UnityEngine;

public abstract class Connection : MonoBehaviour
{
    public Int64 InitLeftId { get; set; }
    public Int64 InitRightId { get; set; }

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

    /// <summary>
    /// Return the opposite entry
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public Entry OppositeEntry(Entry e)
    {
        if (e.Equals(this.Left))
        {
            return this.Right;
        }
        if (e.Equals(this.Right))
        {
            return this.Left;
        }
        return null;
    }

    public override int GetHashCode()
    {
        return this.InitLeftId.GetHashCode() + this.InitRightId.GetHashCode();
    }

    public override bool Equals(object o)
    {
        if (o is Connection)
        {
            var otherConnec = (Connection)o;
            return this.GetType() == o.GetType() && this.InitLeftId == otherConnec.InitLeftId && this.InitRightId == otherConnec.InitRightId;
        }
        return false;
    }
}
