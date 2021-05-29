namespace BookSearchWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookAuthorDetails",
                c => new
                    {
                        AuthorId = c.Int(nullable: false, identity: true),
                        AuthorLink = c.String(),
                        AuthorName = c.String(),
                        PlaceOfBirthLink = c.String(),
                        PlaceOfBirth = c.String(),
                        Latitude = c.Single(nullable: false),
                        Longitude = c.Single(nullable: false),
                        DataAndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AuthorId);
            
            CreateTable(
                "dbo.BookDetails",
                c => new
                    {
                        BookId = c.Int(nullable: false, identity: true),
                        BookLink = c.String(),
                        Name = c.String(),
                        Abstract = c.String(),
                        NumberOfPages = c.Int(nullable: false),
                        Comment = c.String(),
                        AuthorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BookId)
                .ForeignKey("dbo.BookAuthorDetails", t => t.AuthorId, cascadeDelete: true)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.BookUserSearches",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SearchedFor = c.String(),
                        Name = c.String(),
                        AuthorLink = c.String(),
                        Author = c.String(),
                        BookLink = c.String(),
                        DataAndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookDetails", "AuthorId", "dbo.BookAuthorDetails");
            DropIndex("dbo.BookDetails", new[] { "AuthorId" });
            DropTable("dbo.BookUserSearches");
            DropTable("dbo.BookDetails");
            DropTable("dbo.BookAuthorDetails");
        }
    }
}
