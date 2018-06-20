﻿using System.Collections.Generic;

namespace System.Diagnostics.Tests.Testdata
{
    public class Organization
    {
        public Organization()
        {
            this.Persons = new HashSet<Person>();
        }

        public string Name { get; set; }

        public ICollection<Person> Persons { get; set; }
    }
}