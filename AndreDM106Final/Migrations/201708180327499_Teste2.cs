namespace AndreDM106Final.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Teste2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "codigo", c => c.String(nullable: false, maxLength: 8));
            AlterColumn("dbo.Products", "Url", c => c.String(maxLength: 80));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "Url", c => c.String());
            AlterColumn("dbo.Products", "codigo", c => c.String(nullable: false));
        }
    }
}
