using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Arachnee
{
    public class Movie : Entry
    {
        public String title
        {
            get;
            set;
        }

        public String date
        {
            get;
            set;
        }

        public Artist director
        {
            get;
            set;
        }

        public HashSet<Artist> actors
        {
            get;
            set;
        }

        /**
         * constructor with title, date, director
         * 
         * */
        public Movie (String title, String date, Artist director)
        {
            this.title = title;
            this.date = date;
            this.director = director;
            this.actors = new HashSet<Artist>();
        }

        /**
         * constructor with title, date
         * 
         * */
        public Movie(String title, String date)
        {
            this.title = title;
            this.date = date;
            this.director = new Artist();
            this.actors = new HashSet<Artist>();
        }

        /**
         * return a string showing the movie
         * 
         * */
        public String toString()
        {
            String actors = "";
            foreach (Artist artist in this.actors)
            {
                actors += artist.toString() + " ";
            }
            return   "Title : " + this.title +"\n"
                    +"Date : " + this.date+"\n"
                    +"Director : " + this.director.toString()+"\n"
                    + "Actors : "+actors+"\n";
        }

        /**
         * add an actor (an Artist) to the current movie
         * 
         * */
        public void addActor(Artist actor)
        {
            this.actors.Add(actor);
        }
    }
}
