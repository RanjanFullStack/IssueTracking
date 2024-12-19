using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IssueTracking.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFantasySquadResponseCompositeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FantasySquadResponses",
                table: "FantasySquadResponses");

            migrationBuilder.AlterColumn<string>(
                name: "ApiName",
                table: "FantasySquadResponses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "FantasySquadResponses",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FantasySquadResponses",
                table: "FantasySquadResponses",
                columns: new[] { "Date", "ApiName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FantasySquadResponses",
                table: "FantasySquadResponses");

            migrationBuilder.AlterColumn<string>(
                name: "ApiName",
                table: "FantasySquadResponses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "FantasySquadResponses",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FantasySquadResponses",
                table: "FantasySquadResponses",
                column: "Date");
        }
    }
}
