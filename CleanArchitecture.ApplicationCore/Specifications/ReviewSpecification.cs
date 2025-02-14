﻿using Ardalis.Specification;
using CleanArchitecture.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.ApplicationCore.Specifications
{
    public class ReviewSpecification : Specification<Review>
    {
        public ReviewSpecification(int villaId)
        {
            Query.Where(x => x.VillaId == villaId).Take(5).OrderByDescending(x => x.Id);
        }
    }
}
