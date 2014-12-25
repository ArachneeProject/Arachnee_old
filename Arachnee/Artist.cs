using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arachnee
{
    public class Artist : Entry
    {
        public String name
        {
            get;
            set;
        }

        public String surname
        {
            get;
            set;
        }

        public int yearOfBirth
        {
            get;
            set;
        }

        public Artist (String name,String surname)
        {
            this.name = name;
            this.surname = surname;
        }

        public Artist()
        {
            this.name = "";
            this.surname = "";
        }

        public String toString()
        {
            return this.surname +" "+ this.name;
        }

        public String toStringExtended()
        {
            return  "Name : "+this.name+"\n"+
                    "Surname : "+this.surname+"\n"+
                    "Year of birth : "+this.yearOfBirth+"\n";
        }
    }
}
