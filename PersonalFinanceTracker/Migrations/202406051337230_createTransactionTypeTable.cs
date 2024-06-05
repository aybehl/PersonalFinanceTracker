namespace PersonalFinanceTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createTransactionTypeTable : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Categories");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryId = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                        CategoryType = c.String(),
                    })
                .PrimaryKey(t => t.CategoryId);
            
        }
    }
}
