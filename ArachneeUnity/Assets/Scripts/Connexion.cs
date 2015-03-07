using UnityEngine;

public class Connexion
{
    public Entry left
    {
        get;
        set;
    }

    public Entry right
    {
        get;
        set;
    }

    public Nature nature
    {
        get;
        set;
    }


    public Connexion(Entry eLeft, Entry eRight, Nature nature)
    {
        this.left = eLeft;
        this.right = eRight;
        this.nature = nature;
    }

    public Connexion (Entry left, Nature nat)
    {
        this.left = left;
        this.right = new Entry();
        this.nature = nat;
    }

    public void addRight(Entry e)
    {
        this.right = e;
    }

	public void highlight()
	{
		Debug.DrawLine(this.left.transform.position,this.right.transform.position);
	}

}

