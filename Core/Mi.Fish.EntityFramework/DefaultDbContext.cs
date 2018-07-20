using System;
using System.Collections.Generic;
using System.Text;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mi.Fish.EntityFramework
{
    public abstract class DefaultDbContext : AbpDbContext
    {
        /// <summary>Constructor.</summary>
        protected DefaultDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}
