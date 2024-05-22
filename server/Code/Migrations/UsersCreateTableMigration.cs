using FluentMigrator;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;

namespace server.Code.Migrations;

[TimestampedMigration(2024,5,2,16,40)]
public class UsersCreateTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("users")
            .WithColumn("unique_id").AsString().PrimaryKey()
            .WithColumn("player_id").AsProperGuid().ProperForeignKey("players", "unique_id");
    }

    public override void Down()
    {
        Delete.Table("users");
    }
}