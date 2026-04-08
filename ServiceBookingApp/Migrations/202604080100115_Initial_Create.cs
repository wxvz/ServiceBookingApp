namespace ServiceBookingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial_Create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        BusinessId = c.Int(nullable: false),
                        ServiceId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Time = c.Time(nullable: false, precision: 7),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BookingId)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .ForeignKey("dbo.Businesses", t => t.BusinessId)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId)
                .Index(t => t.BusinessId)
                .Index(t => t.ServiceId);
            
            CreateTable(
                "dbo.Businesses",
                c => new
                    {
                        BusinessId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        City = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.BusinessId);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentId = c.Int(nullable: false),
                        BusinessId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentDate = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        Method = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentId)
                .ForeignKey("dbo.Businesses", t => t.BusinessId)
                .ForeignKey("dbo.Bookings", t => t.PaymentId)
                .Index(t => t.PaymentId)
                .Index(t => t.BusinessId);
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        ServiceId = c.Int(nullable: false, identity: true),
                        BusinessId = c.Int(nullable: false),
                        Name = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Duration = c.Time(nullable: false, precision: 7),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ServiceId)
                .ForeignKey("dbo.Businesses", t => t.BusinessId, cascadeDelete: true)
                .Index(t => t.BusinessId);
            
            CreateTable(
                "dbo.ServiceSchedules",
                c => new
                    {
                        ServiceScheduleId = c.Int(nullable: false, identity: true),
                        ServiceId = c.Int(nullable: false),
                        DayOfWeek = c.Int(nullable: false),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(nullable: false, precision: 7),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ServiceScheduleId)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.ServiceId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        Address = c.String(),
                        PhoneNumber = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Payments", "PaymentId", "dbo.Bookings");
            DropForeignKey("dbo.Bookings", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Bookings", "BusinessId", "dbo.Businesses");
            DropForeignKey("dbo.ServiceSchedules", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.Services", "BusinessId", "dbo.Businesses");
            DropForeignKey("dbo.Bookings", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.Payments", "BusinessId", "dbo.Businesses");
            DropIndex("dbo.ServiceSchedules", new[] { "ServiceId" });
            DropIndex("dbo.Services", new[] { "BusinessId" });
            DropIndex("dbo.Payments", new[] { "BusinessId" });
            DropIndex("dbo.Payments", new[] { "PaymentId" });
            DropIndex("dbo.Bookings", new[] { "ServiceId" });
            DropIndex("dbo.Bookings", new[] { "BusinessId" });
            DropIndex("dbo.Bookings", new[] { "CustomerId" });
            DropTable("dbo.Customers");
            DropTable("dbo.ServiceSchedules");
            DropTable("dbo.Services");
            DropTable("dbo.Payments");
            DropTable("dbo.Businesses");
            DropTable("dbo.Bookings");
        }
    }
}
