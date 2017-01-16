using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class MovieSearch : MonoBehaviour 
{
    public MovieSearchResult[] results;
    private OnlineRetriever onlineRetriever = new OnlineRetriever();

    
    public IEnumerator GetResults(string input, bool isMovie)
    {
        foreach (MovieSearchResult res in this.results)
        {
            Debug.LogError("shit over here");
            //res.Id = Constants.NULL_MOVIE_ID;
            res.Title = "";
            res.Year = "";
            res.Texture = null;
        }

        yield return StartCoroutine(this.onlineRetriever.RetrieveResultOfSerach(input, isMovie));

        JSONNode resultsNode = this.onlineRetriever.NodeRetrieved;
        if (resultsNode == null)
        {
            yield break;
        }

        for (int i = 0; i < resultsNode.Count && i < this.results.Length; i++)
        {
            // id
            this.results[i].Id = int.Parse(resultsNode[i]["id"].Value);            

            // overview
            this.results[i].Overview = resultsNode[i]["overview"].Value;

            // title & date
            if (isMovie)
            {
                this.results[i].Title = resultsNode[i]["original_title"].Value;
                this.results[i].Year = resultsNode[i]["release_date"].Value;                
            }
            else
            {
                this.results[i].Title = resultsNode[i]["original_name"].Value;
                this.results[i].Year = resultsNode[i]["first_air_date"].Value;
            }
            
            // poster
            string posterUrl = resultsNode[i]["poster_path"].Value;
            this.results[i].PosterPath = posterUrl;

            if (posterUrl != "null")
            {
                yield return StartCoroutine(this.onlineRetriever.RetrievePoster(posterUrl));
                this.results[i].Texture = this.onlineRetriever.Texture;
            }
            if (this.results[i].Texture == null)
            {
                this.results[i].Texture = Resources.Load(Constants.Res_DefaultImage) as UnityEngine.Texture2D;
            }

            // genres
            for (int j = 0; j < resultsNode[i]["genre_ids"].Count; j++)
            {
                this.results[i].Genres.Add(resultsNode[i]["genre_ids"][j].Value);
            }

            // tricky update of genres
            if (isMovie)
            {
                yield return StartCoroutine(this.onlineRetriever.RetrieveMovieGenres());                
            }
            else
            {
                yield return StartCoroutine(this.onlineRetriever.RetrieveSerieGenres());
            }
            JSONNode nodyGenres = this.onlineRetriever.NodeRetrieved["genres"];

            Dictionary<string, string> dicoGenre = new Dictionary<string, string>();
            for (int j = 0; j < nodyGenres.Count; j++)
            {
                dicoGenre.Add(nodyGenres[j]["id"].Value, nodyGenres[j]["name"]);
            }
            List<string> finalGenres = new List<string>();
            foreach (string idG in this.results[i].Genres)
            {
                string theG = "";
                if (dicoGenre.TryGetValue(idG, out theG))
                {
                    finalGenres.Add(theG);
                }
                else
                {
                    Debug.LogError("genre id not found: " + idG);
                }
                
            }
            this.results[i].Genres = finalGenres;

            // I don't recomand using resultsNode anymore here
        }
    }


}
