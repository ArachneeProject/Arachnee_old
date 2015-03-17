using UnityEngine;

public class Connexion : MonoBehaviour
{
    // left -> right
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

    // renderer of the connexion
    private LineRenderer line;

    /// <summary>
    /// initialize the lineRenderer 
    /// </summary>
    /// <param name="lr"></param>
    public void initializeLineRenderer(LineRenderer lr)
    {
        this.line = lr;
        line.SetPosition(0, this.right.transform.position);
        line.SetPosition(1, this.left.transform.position);
        line.SetWidth(0.5F, 0.5F);
        this.renderer.enabled = false;
    }

	public void highlight()
	{
		this.renderer.enabled = true;
	}

    public void mask()
    {
        this.renderer.enabled = false;
    }

}

