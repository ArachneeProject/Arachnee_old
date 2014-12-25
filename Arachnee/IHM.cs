using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arachnee
{
    public static class IHM
    {
        public static String choice;

        public static void startMenu()
        {
            Console.WriteLine(  "Hello ! Welcome to Arachnee. What do you want to do ?");
            Console.WriteLine(  "1 - add a new movie");
            Console.WriteLine(  "2 - show movies");
            Console.WriteLine(  "0 - exit");
            Console.WriteLine("(type the number you want then press enter)");
            
            choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    IHM.addNewMovie();
                    break;
                case "2":
                    DataBase.showMovies();
                    break;
                default:
                    Program.run = false;
                    break;
            }
        }

        public static void addNewMovie()
        {
            Console.WriteLine("You've chosen to add a new movie.");
            Console.WriteLine("Type the title of the movie.");
            String title = Console.ReadLine();
            Console.WriteLine("Type its date of release.");
            String date = Console.ReadLine();

            Movie movie = new Movie(title, date);

            Console.WriteLine("Do you want to add its full cast now ?");
            Console.WriteLine("(type y for yes or n for no)");

            choice = Console.ReadLine();
            if (choice == "y")
            {
                IHM.addCast();
            }

            Console.WriteLine("Ok, here is your movie.");
            Console.WriteLine("");
            Console.WriteLine(movie.toString());
            Console.WriteLine("");
            Console.WriteLine("Do you confirm this movie ?");
            Console.WriteLine("(type y for yes or n for no)");

            choice = Console.ReadLine();
            if (choice == "n")
            {
                Console.WriteLine("This movie is going to be deleted. Are you sur you want to cancel this movie ?");
                Console.WriteLine("(type y for yes or n for no)");
                choice = Console.ReadLine();
                if (choice == "y")
                {
                    return;
                }
            }
            Console.WriteLine("Adding "+movie.title+" to database...");
            DataBase.addMovie(movie);
            Console.WriteLine("Completed !");
        }

        public static void addCast()
        {
            throw new NotImplementedException();
        }
    }
}
