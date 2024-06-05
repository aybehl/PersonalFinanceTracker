namespace PersonalFinanceTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FKeyTransactionAndCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "CategoryId", c => c.Int(nullable: false));
            CreateIndex("dbo.Transactions", "CategoryId");
            AddForeignKey("dbo.Transactions", "CategoryId", "dbo.Categories", "CategoryId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Transactions", new[] { "CategoryId" });
            DropColumn("dbo.Transactions", "CategoryId");
        }
    }
}
