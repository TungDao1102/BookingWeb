﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.ApplicationCore.Entities
{
    public class City
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? PostOffice { get; set; }
        public string? CountryName { get; set; }
    }
}
