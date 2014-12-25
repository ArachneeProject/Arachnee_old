using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


namespace Arachnee
{
    public static class DataBase
    {
        public static HashSet<Movie> movies
        {
            get;
            set;
        }


        public static HashSet<Artist> artists
        {
            get;
            set;
        }

        public static HashSet<Job> jobs
        {
            get;
            set;
        }

        /**
         * show in console all movies in database
         * 
         * */
        public static void showMovies ()
        {
            String resul = "\n";
            foreach (Movie movie in DataBase.movies)
            {
                resul += movie.toString()+"\n\n";
            }
            Console.WriteLine(resul);
            Console.WriteLine(("(press enter to come back to the main menu)"));
            Console.Read();
        }

        /**
         * initialize database from a string (raw data from file)
         * 
         * */
        public static void initializeDataBaseFromData (String rawData)
        {
            DataBase.movies = new HashSet<Movie>();
            DataBase.artists = new HashSet<Artist>();
            DataBase.jobs = new HashSet<Job>();

            String moviesMatch = Regex.Match(rawData, @"\<Movie\>(.*)\<\\Movie\>").Groups[1].Value;
            String[] movies = Regex.Split(moviesMatch, @"\=\=");
            foreach (String movie in movies)
            {
                DataBase.addMovie(MovieBuilder.buildMovie(movie));
            }
        }

        /**
         * add an new Movie to database
         * TO DO : sort movies and check if movie isn't already recorded
         * */
        public static void addMovie(Movie movie)
        {
            DataBase.movies.Add(movie);
        }

        /**
         * add an new Artist to database
         * TO DO : same as addMovie
         * */
        public static void addArtist(Artist artist)
        {
            DataBase.artists.Add(artist);
        }

        /**
         * add an new Job to database
         * TO DO
         * */
        public static void addJob(Job job)
        {
            throw new NotImplementedException();
        }

    }
}
