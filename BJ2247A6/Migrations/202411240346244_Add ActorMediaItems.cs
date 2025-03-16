namespace BJ2247A5.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActorMediaItems : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActorMediaItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ContentType = c.String(),
                        Content = c.Binary(),
                        Caption = c.String(),
                        Actor_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Actors", t => t.Actor_Id)
                .Index(t => t.Actor_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ActorMediaItems", "Actor_Id", "dbo.Actors");
            DropIndex("dbo.ActorMediaItems", new[] { "Actor_Id" });
            DropTable("dbo.ActorMediaItems");
        }
    }
}
