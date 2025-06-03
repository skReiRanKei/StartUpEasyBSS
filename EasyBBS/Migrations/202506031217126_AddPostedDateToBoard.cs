namespace EasyBBS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPostedDateToBoard : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BoardEntities", "PostedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BoardEntities", "PostedDate");
        }
    }
}
