namespace ServiceBookingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CitytoAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Businesses", "Address", c => c.String());
            DropColumn("dbo.Businesses", "City");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Businesses", "City", c => c.String());
            DropColumn("dbo.Businesses", "Address");
        }
    }
}
