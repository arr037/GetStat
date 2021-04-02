using Microsoft.EntityFrameworkCore.Migrations;

namespace GetStat.Api.Migrations
{
    public partial class addTestName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestName",
                table: "QueueTests",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestName",
                table: "QueueTests");
        }
    }
}
