namespace EasyBBS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BoardEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Text = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BoardPostEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        BoardEntity_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BoardEntities", t => t.BoardEntity_Id)
                .Index(t => t.BoardEntity_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BoardPostEntities", "BoardEntity_Id", "dbo.BoardEntities");
            DropIndex("dbo.BoardPostEntities", new[] { "BoardEntity_Id" });
            DropTable("dbo.BoardPostEntities");
            DropTable("dbo.BoardEntities");
        }
    }
}
