using System.Collections.Generic;


public class ConnexionBank
{
    public static List<Connexion> connexions = new List<Connexion>();

	public static void highlightConnexions<T>(int id)
	{
		foreach (Connexion connection in connexions) 
		{
			if ((connection.left is T && connection.left.Id == id) || connection.right is T && connection.right.Id == id)
			{
				connection.highlight();
			}
		}
	}
}