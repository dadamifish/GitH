using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.EntityFramework
{
    public interface IDbConnectionProvider
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the database context.</typeparam>
        /// <returns></returns>
        string GetConnectionString<TDbContext>();
    }
}
