using UnityEngine;
using System.Collections;

public class Constants
{
    public const string PP_MainFolder = "MainFolder";
    public const string PP_PreviousLevel = "PreviousLevel";
    public const string PP_SeedVertexIdentifier = "SeedVertexIdentifier";
    public const string PP_EntriesToUpdate = "EntriesToUpdate";

    public const string DefaultImageName = "default";

    public const char EntryIdentifierSeparator = '-';
    
    public const string MovieUrl = "https://api.themoviedb.org/3/movie/";
    public const string ArtistUrl = "https://api.themoviedb.org/3/person/";
    public const string SerieUrl = "https://api.themoviedb.org/3/tv/";

    public const string MovieSearchUrl = "https://api.themoviedb.org/3/search/movie?query=";
    public const string ArtistSearchUrl = "https://api.themoviedb.org/3/search/person?query=";
    public const string SerieSearchUrl = "https://api.themoviedb.org/3/search/tv?query=";
    
    public const string PosterUrl = "http://image.tmdb.org/t/p/w500";
    public const string MovieGenresUrl = "http://api.themoviedb.org/3/genre/movie/list";
    public const string SerieGenresUrl = "http://api.themoviedb.org/3/genre/tv/list";
    
    public const string CastQuery = "/credits?";
    public const string CreditsQuery = "/movie_credits?";
    
    // search example https://api.themoviedb.org/3/search/movie?query=cube&api_key= x x x
    // poster example http://image.tmdb.org/t/p/w500/u50r6chJGO2iqxVAvtQ07obiCkB.jpg
    // movie example https://api.themoviedb.org/3/movie/137?api_key= x x x
}
