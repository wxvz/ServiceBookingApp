namespace ServiceBookingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedingAndNaming : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Bookings", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Payments", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Payments", "Method", c => c.Int(nullable: false));
            DropColumn("dbo.Bookings", "BookingStatus");
            DropColumn("dbo.Payments", "PaymentStatus");
            DropColumn("dbo.Payments", "PaymentMethod");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Payments", "PaymentMethod", c => c.Int(nullable: false));
            AddColumn("dbo.Payments", "PaymentStatus", c => c.Int(nullable: false));
            AddColumn("dbo.Bookings", "BookingStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Payments", "Method");
            DropColumn("dbo.Payments", "Status");
            DropColumn("dbo.Bookings", "Status");
        }
    }
}
