using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Mi.Fish.EntityFramework
{
    public class DbContextOptionsFactory
    {
        private readonly IDbConnectionProvider _dbConnectionProvider;

        /// <summary>Initializes a new instance of the <see cref="T:Mi.Fish.EntityFramework.LocalDbContextOptionsFactory"></see> class.</summary>
        public DbContextOptionsFactory(IDbConnectionProvider dbConnectionProvider)
        {
            _dbConnectionProvider = dbConnectionProvider;
        }

        public DbContextOptions<TDbContext> GetDbContextOptions<TDbContext>() where TDbContext : DbContext
        {
            var builder = new DbContextOptionsBuilder<TDbContext>();

            builder.UseSqlServer(_dbConnectionProvider.GetConnectionString<TDbContext>());

            return builder.Options;
        }
    }
}
