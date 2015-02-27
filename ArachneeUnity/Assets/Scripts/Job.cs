using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class Job
    {
        public string Title
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        /// <summary>
        /// default constructor
        /// </summary>
        public Job ()
        {
            this.Title = "Human";
            this.Description = "No description available.";
        }
    }
}
