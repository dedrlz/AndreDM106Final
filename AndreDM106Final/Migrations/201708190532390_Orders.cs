namespace AndreDM106Final.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Orders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        userEmail = c.String(nullable: false, maxLength: 80),
                        dataPedido = c.DateTime(nullable: false),
                        dataEntrega = c.DateTime(nullable: false),
                        statusPedido = c.String(nullable: false, maxLength: 10),
                        precoPedido = c.Decimal(nullable: false, precision: 18, scale: 2),
                        pesoPedido = c.Decimal(nullable: false, precision: 18, scale: 2),
                        precoFrete = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        quantidade = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.OrderId);
            
            AddColumn("dbo.Products", "cor", c => c.String());
            AddColumn("dbo.Products", "modelo", c => c.String(nullable: false));
            AddColumn("dbo.Products", "peso", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "altura", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "largura", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "comprimento", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "diametro", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderItems", "ProductId", "dbo.Products");
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.OrderItems", new[] { "ProductId" });
            DropColumn("dbo.Products", "diametro");
            DropColumn("dbo.Products", "comprimento");
            DropColumn("dbo.Products", "largura");
            DropColumn("dbo.Products", "altura");
            DropColumn("dbo.Products", "peso");
            DropColumn("dbo.Products", "modelo");
            DropColumn("dbo.Products", "cor");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Orders");
        }
    }
}
