using System.Collections.Generic;


public class ConnexionBank
{
    public static List<Connexion> connexions = new List<Connexion>();

    private static List<Connexion> highlighted = new List<Connexion>();

    /// <summary>
    /// highlight all connexions between the specific T and other entries
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
	public static void highlightConnexions<T>(int id)
	{
		foreach (Connexion connection in connexions) 
		{
			if ((connection.left is T && connection.left.Id == id) || connection.right is T && connection.right.Id == id)
			{
				connection.highlight();
                highlighted.Add(connection);
			}
		}
	}

    /// <summary>
    /// 
    /// </summary>
    public static void maskAll()
    {
        foreach (Connexion connection in highlighted)
        {
            connection.mask();
        }
        highlighted.Clear();
    }
}