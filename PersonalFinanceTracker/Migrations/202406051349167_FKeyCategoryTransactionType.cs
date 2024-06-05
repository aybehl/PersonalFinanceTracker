namespace PersonalFinanceTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FKeyCategoryTransactionType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "TransactionTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Categories", "TransactionTypeId");
            AddForeignKey("dbo.Categories", "TransactionTypeId", "dbo.TransactionTypes", "TransactionTypeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Categories", "TransactionTypeId", "dbo.TransactionTypes");
            DropIndex("dbo.Categories", new[] { "TransactionTypeId" });
            DropColumn("dbo.Categories", "TransactionTypeId");
        }
    }
}
