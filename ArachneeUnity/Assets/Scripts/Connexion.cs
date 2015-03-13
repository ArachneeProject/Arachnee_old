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

    private GameObject lineGO;


    public Connexion(Entry eLeft, Entry eRight, Nature nature)
    {
        this.left = eLeft;
        this.right = eRight;
        this.nature = nature;

        lineGO = new GameObject();
        LineRenderer line = lineGO.AddComponent<LineRenderer>();
        line.SetPosition(0, this.right.transform.position);
        line.SetPosition(1, this.left.transform.position);
        line.SetWidth(0.05F, 0.05F);
        lineGO.renderer.enabled = false;
    }

	public void highlight()
	{
		lineGO.renderer.enabled = true;
	}

    public void mask()
    {
        lineGO.renderer.enabled = false;
    }

}

