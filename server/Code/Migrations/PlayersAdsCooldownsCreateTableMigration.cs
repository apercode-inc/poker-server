using FluentMigrator;
using server.Code.MorpehFeatures.AdsFeature;
using server.Code.MorpehFeatures.DataBaseFeature.Utils;

namespace server.Code.Migrations;

[TimestampedMigration(2024, 11, 24, 10, 51)]
public class PlayersAdsCooldownsCreateTableMigration : Migration
{
    public override void Up()
    {
        Create.Table(DbPlayerAdsCooldownConstants.TableName)
            .WithColumn(DbPlayerAdsCooldownConstants.PlayerId).AsProperGuid()
            .WithColumn(DbPlayerAdsCooldownConstants.PanelId).AsString()
            .WithColumn(DbPlayerAdsCooldownConstants.EndTimestamp).AsInt32();

        Create.PrimaryKey().OnTable(DbPlayerAdsCooldownConstants.TableName)
            .Columns(DbPlayerAdsCooldownConstants.PlayerId, DbPlayerAdsCooldownConstants.PanelId);

        Create.ForeignKey()
            .FromTable(DbPlayerAdsCooldownConstants.TableName).ForeignColumn(DbPlayerAdsCooldownConstants.PlayerId)
            .ToTable("players").PrimaryColumn("unique_id");
    }

    public override void Down()
    {
        Delete.Table(DbPlayerAdsCooldownConstants.TableName);
    }
}