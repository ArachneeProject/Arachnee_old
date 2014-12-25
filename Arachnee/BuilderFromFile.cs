using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Arachnee
{
    public static class BuilderFromFile
    {
        /**
         * write in the file (stupid)
         * 
         * */
        public static void writeLinesInFile(String[] lines, String fileName)
        {
            // create a writer and open the file
            TextWriter tw = new StreamWriter(fileName, true);

            foreach (String line in lines)
            {
                // write a line of text to the file
                tw.WriteLine(line);
            }


            // close the stream
            tw.Close();
        }

        /**
         * read the file (stupid)
         * return a big string[] (I said stupid)
         * */
        public static String[] readFromFile(String fileName)
        {
            return File.ReadAllLines(fileName);
        }

        /**
         * build a new Artist from a string ; add it to database
         * 
         * */
        public static Artist buildArtist(String rawData)
        {
            Match artistMatch = Regex.Match(rawData, @"\<Artist\>\<name\>(.*)\<\\name\>\<surname\>(.*)\<\\surname\>\<\\Artist\>");
            String name = artistMatch.Groups[1].Value;
            String surname = artistMatch.Groups[2].Value;

            Artist artist = new Artist(name, surname);

            DataBase.addArtist(artist);
            return artist;
        }

        /**
         * build a movie from a string (raw data from a file)
         * 
         * */
        public static Movie buildMovie(String rawData)
        {
            String title = Regex.Match(rawData, @"\<title\>(.*)\<\\title\>").Groups[1].Value;
            String date = Regex.Match(rawData, @"\<date\>(\d+)\<\\date\>").Groups[1].Value;

            String directorMatch = Regex.Match(rawData, @"\<director\>(.*)\<\\director\>").Groups[1].Value;
            Artist director = BuilderFromFile.buildArtist(directorMatch);

            Movie movie = new Movie(title, date, director);

            String actorsMatch = Regex.Match(rawData, @"\<actors\>(.*)\<\\actors\>").Groups[1].Value;
            String[] actors = Regex.Split(actorsMatch, @",");
            foreach (String actor in actors)
            {
                movie.addActor(BuilderFromFile.buildArtist(actor));
            }

            return movie;
        }
    }
}
