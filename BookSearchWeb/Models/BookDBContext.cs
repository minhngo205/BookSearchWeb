using System;
using System.Data.Entity;
using System.Linq;

namespace BookSearchWeb.Models
{
    public class BookDBContext : DbContext
    {
        // Your context has been configured to use a 'BookDBContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'BookSearchWeb.BookDBContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'BookDBContext' 
        // connection string in the application configuration file.
        public BookDBContext()
            : base("name=BookDBContext")
        {
        }
        public DbSet<BookUserSearch> BookNameTable { get; set; }
        public DbSet<BookDetails> BookDetailsTable { get; set; }
        public DbSet<BookAuthorDetails> AuthorDetailsTable { get; set; }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}