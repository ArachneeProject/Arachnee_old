using System.Collections.Generic;
using UnityEngine;

public class PhysicalVertex : MonoBehaviour
{
    public FloattingLabel label;
    
    private Entry _entry;
    public Entry Entry
    {
        get { return _entry ?? (_entry = Entry.DefaultEntry); }
        set
        {
            if (Entry.IsNullOrDefault(value))
            {
                return;
            }
            this._entry = value;
            this.label.Text = value.ToString();
        }
    }
    
    public Rigidbody RigidBody
    {
        get;
        private set;
    }

    /// <summary>
    /// Init
    /// </summary>
    void Start()
    {
        this.RigidBody = this.GetComponent<Rigidbody>();
    }
    
    public delegate void EntryClickHandler(PhysicalVertex v);
    public event EntryClickHandler EntryClickedEvent;
    
    void OnMouseDown()
    {
        if (EntryClickedEvent != null) // check if there is at least one suscriber
        {
            EntryClickedEvent(this);
        }
    }

    public override string ToString()
    {
        return this.GetType().Name + " of " + this.Entry;
    }
}

