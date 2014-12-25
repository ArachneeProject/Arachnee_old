using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arachnee
{
    class Program
    {
        public static bool run = true;
        static void Main(string[] args)
        {
            String fileName = "movies.txt";

            /* write in file
            String[] test = new String[] { "ones", "two", "three", "motherfucker" };
            FileManager.writeLinesInFile(test,fileName);
             * */
            String[] fileIn = FileManager.readFromFile(fileName);
            String rawData = "";
            foreach (String line in fileIn)
            {
                rawData += line;
            }
            DataBase.initializeDataBaseFromData(rawData);

            while (run)
            {
                IHM.startMenu();
            }
        }
    }
}
