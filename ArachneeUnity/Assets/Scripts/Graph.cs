using System.Collections.Generic;
using UnityEngine;

public class Graph
{
    // vertices
    public static Dictionary<int, Movie> buildedMovies = new Dictionary<int, Movie>();
    public static Dictionary<int, Artist> buildedArtists = new Dictionary<int, Artist>();

    // edges
    public static List<Connexion> connexions = new List<Connexion>();

    // highlighted edges
    private static List<Connexion> highlighted = new List<Connexion>();

    /// <summary>
    /// highlight all connexions between the specific T and other entries
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
	public static void highlightConnexions<T>(int id)
	{
        foreach (Connexion connexion in connexions) 
		{
            if ((connexion.left is T && connexion.left.Id == id) || connexion.right is T && connexion.right.Id == id)
			{
                connexion.highlight();
                highlighted.Add(connexion);
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