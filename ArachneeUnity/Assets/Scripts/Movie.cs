using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{
    public class Movie : MonoBehaviour
    {
        // store the number of movie
        public static int movieCounter = 0 ;

        // identifier of the movie
        public int Id
        {
            get;
            private set;
        }

        // title of the movie
        public string Title
        {
            get;
            private set;
        }

        // date of release
        public int Date
        {
            get;
            private set;
        }

        public Artist Director
        {
            get;
            private set;
        }


        /// <summary>
        /// default constructor
        /// </summary>
        public Movie()
        {
            this.Id = ++movieCounter;
        }



        /// <summary>
        /// highlights the movie
        /// </summary>
        public void OnMouseEnter()
        {
            renderer.material.color = Color.red;
            Debug.Log(this.Id + " : " + this.Title + "," + this.Date);
        }

        /// <summary>
        /// stops highlighting the movie
        /// </summary>
        public void OnMouseExit()
        {
            renderer.material.color = Color.white;
        }



    }
}