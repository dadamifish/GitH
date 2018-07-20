using System;
using System.Collections.Generic;
using System.Text;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mi.Fish.EntityFramework
{
    public class LocalDbContext : AbpDbContext
    {
        public const string ConnectionKey = "LocalDb";

        /// <summary>Constructor.</summary>
        public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options)
        {
        }
    }
}
