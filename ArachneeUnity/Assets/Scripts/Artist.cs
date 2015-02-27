using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class Artist
    {
        // name
        public string Name
        {
            get;
            private set;
        }

        // surname
        public string Surname
        {
            get;
            private set;
        }

        // date of birth
        public int Birthdate
        {
            get;
            private set;
        }

        // job
        public Job Job
        {
            get;
            private set;
        }

        public Artist()
        {
            this.Name = "Allan";
            this.Surname = "Smithee";
            this.Birthdate = 1950;
        }
    }
}
