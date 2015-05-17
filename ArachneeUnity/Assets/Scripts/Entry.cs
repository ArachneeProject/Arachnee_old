using UnityEngine;

public class Entry : MonoBehaviour
{
    public GameObject label;

    // id
    public int Id
    {
        get;
        set;
    }

    void OnBecameVisible()
    {
        if (label == null)
        {
            return;
        }
        label.SetActive(true);
    }
    void OnBecameInvisible()
    {
        if (label == null)
        {
            return;
        }
        label.SetActive(false);
    }
}

