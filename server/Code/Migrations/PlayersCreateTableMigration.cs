using FluentMigrator;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;

namespace server.Code.Migrations;

[TimestampedMigration(2024,4,24,16,40)]
public class PlayersCreateTableMigration : Migration
{
    public override void Up()
    {
        Create.Table("players")
            .WithColumn("unique_id").AsProperGuid().PrimaryKey()
            .WithColumn("nickname").AsString()
            .WithColumn("level").AsInt32()
            .WithColumn("experience").AsInt32()
            .WithColumn("chips").AsInt64()
            .WithColumn("gold").AsInt64()
            .WithColumn("stars").AsInt64()
            .WithColumn("registration_date").AsDateTime();
    }

    public override void Down()
    {
        Delete.Table("players");
    }
}