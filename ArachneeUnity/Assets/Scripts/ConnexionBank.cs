using System.Collections.Generic;


public class ConnexionBank
{
    public static List<Connexion> connexions = new List<Connexion>();

	public static void highlightMovieConnexions(int mvId)
	{
		foreach (Connexion connection in connexions) 
		{
			if (connection.left.Id == mvId || connection.right.Id == mvId)
			{
				connection.highlight();
			}
		}
	}
}