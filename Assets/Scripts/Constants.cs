using UnityEngine;
using System.Collections;

public class Constants
{
    public const string PP_MainFolder = "MainFolder";
    public const string PP_NewFolderName = "NewFolderName";
    public const string PP_FolderHash = "FolderHash";
    public const string PP_PreviousLevel = "PreviousLevel";
    public const string PP_SeedVertexIdentifier = "SeedVertexIdentifier";
    public const string PP_MoviesToUpdate = "MoviesUpdate";

    public const string Res_DefaultImage = "default";
    public const string Res_LoadingImage = "loading";

    public const char EntryIdentifierSeparator = '-';

    public const string urlMovieSearch = "https://api.themoviedb.org/3/search/movie?query=";
    public const string urlTvSearch = "https://api.themoviedb.org/3/search/tv?query=";
    public const string posterUrl = "http://image.tmdb.org/t/p/w500";
    public const string movieUrl = "https://api.themoviedb.org/3/movie/";
    public const string serieUrl = "https://api.themoviedb.org/3/tv/";
    public const string movieGenresUrl = "http://api.themoviedb.org/3/genre/movie/list";
    public const string serieGenresUrl = "http://api.themoviedb.org/3/genre/tv/list";
    public const string personUrl = "https://api.themoviedb.org/3/person/";
    public const string castQuery = "/credits?";
    public const string creditsQuery = "/movie_credits?";
    public const string apiKey = "api_key=The Skeleton Key";

    // search example https://api.themoviedb.org/3/search/movie?query=cube&api_key= x x x
    // poster example http://image.tmdb.org/t/p/w500/u50r6chJGO2iqxVAvtQ07obiCkB.jpg
    // movie example https://api.themoviedb.org/3/movie/137?api_key= x x x
}
