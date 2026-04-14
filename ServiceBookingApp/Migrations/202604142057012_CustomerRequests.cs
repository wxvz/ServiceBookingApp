namespace ServiceBookingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerRequests : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Bookings", "CustomerId", "dbo.Customers");
            CreateTable(
                "dbo.CustomerRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BusinessId = c.Int(nullable: false),
                        BookingId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        BookingDateTime = c.DateTime(nullable: false),
                        CustomerName = c.String(),
                        Request = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bookings", t => t.BookingId)
                .ForeignKey("dbo.Businesses", t => t.BusinessId)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .Index(t => t.BusinessId)
                .Index(t => t.BookingId)
                .Index(t => t.CustomerId);
            
            AddForeignKey("dbo.Bookings", "CustomerId", "dbo.Customers", "CustomerId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Bookings", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.CustomerRequests", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.CustomerRequests", "BusinessId", "dbo.Businesses");
            DropForeignKey("dbo.CustomerRequests", "BookingId", "dbo.Bookings");
            DropIndex("dbo.CustomerRequests", new[] { "CustomerId" });
            DropIndex("dbo.CustomerRequests", new[] { "BookingId" });
            DropIndex("dbo.CustomerRequests", new[] { "BusinessId" });
            DropTable("dbo.CustomerRequests");
            AddForeignKey("dbo.Bookings", "CustomerId", "dbo.Customers", "CustomerId", cascadeDelete: true);
        }
    }
}
