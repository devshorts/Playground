using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace NoNulls
{    
    public class User
    {
        public virtual School School { get; set; }

        public School GetSchool()
        {
            return School;
        }

        public int Number { get; set; }

        public User Field;

        public virtual IEnumerable<User> ClassMatesEnumerable { get; set; }

        public virtual List<User> ClassMatesList { get; set; }

        public virtual Dictionary<User, User> ClassMatesDict { get; set; }

        public virtual HashSet<User> ClassMatesHash { get; set; }
    }

    public class School
    {
        public virtual District District { get; set; }

        public District GetDistrict()
        {
            return District;
        }
    }

    public class District
    {
        public virtual Street Street { get; set; }

        public Street GetStreet()
        {
            return Street;
        }
    }

    public class Street
    {
        public String Name { get; set; }

        public int Number { get; set; }

        public String GetName(int i)
        {
            return Name + i;
        }
    }
}


    



