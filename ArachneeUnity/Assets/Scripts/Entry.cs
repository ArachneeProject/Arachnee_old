using UnityEngine;

public class Entry : MonoBehaviour
{
    public ObjectLabel label;

    // id
    public int Id
    {
        get;
        set;
    }


    protected ParticleSystem particles
    {
        get;
        private set;
    }

    void Start()
    {
        this.particles = this.GetComponent<ParticleSystem>();
    }


    void OnBecameVisible()
    {
        label.gameObject.SetActive(true);
    }
    void OnBecameInvisible()
    {
        if (this.label == null)
        {
            return;
        }
        label.gameObject.SetActive(false);
    }

}

