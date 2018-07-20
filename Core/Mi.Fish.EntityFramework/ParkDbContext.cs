using System;
using System.Collections.Generic;
using System.Text;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mi.Fish.EntityFramework
{
    public class ParkDbContext : AbpDbContext
    {
        public const string ConnectionKey = "ParkDb";

        /// <summary>Constructor.</summary>
        public ParkDbContext(DbContextOptions<ParkDbContext> options) : base(options)
        {
        }
    }
}
