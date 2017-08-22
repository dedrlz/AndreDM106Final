namespace AndreDM106Final.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderData : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "userEmail", c => c.String());
            AlterColumn("dbo.Orders", "statusPedido", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "statusPedido", c => c.String(nullable: false, maxLength: 10));
            AlterColumn("dbo.Orders", "userEmail", c => c.String(nullable: false, maxLength: 80));
        }
    }
}
