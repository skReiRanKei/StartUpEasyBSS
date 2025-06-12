namespace EasyBBS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPostedDateToBoardPostEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BoardPostEntities", "PostedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BoardPostEntities", "PostedDate");
        }
    }
}
