namespace ServiceBookingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOneToOneMappings : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Payments", "BookingId");
            DropColumn("dbo.ServiceSchedules", "ServiceId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ServiceSchedules", "ServiceId", c => c.Int(nullable: false));
            AddColumn("dbo.Payments", "BookingId", c => c.Int(nullable: false));
        }
    }
}
