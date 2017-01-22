using System.Collections.Generic;
using UnityEngine;

public class PhysicalVertex : MonoBehaviour
{
    public FloattingLabel label;
    
    private Entry _entry;
    public Entry Entry
    {
        get
        {
            if (_entry == null)
            {
                return Entry.DefaultEntry;
            }
            return _entry;
        }
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
        if (string.IsNullOrEmpty(this.label.Text))
        {
            this.label.Text = "<no entry>";
        }
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

