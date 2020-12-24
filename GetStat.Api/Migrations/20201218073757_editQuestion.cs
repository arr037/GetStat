using Microsoft.EntityFrameworkCore.Migrations;

namespace GetStat.Api.Migrations
{
    public partial class editQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CorrectAnswer",
                table: "Questions",
                nullable: false,
                defaultValue: -1);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "B0BAB2ED-F61E-4E83-81D7-7B96267D473B",
                column: "ConcurrencyStamp",
                value: "35ca7d43-6b71-4eb4-9a86-643a5bd04124");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "D3FCF42A-22C1-455E-825B-2BF65AA877FE",
                column: "ConcurrencyStamp",
                value: "846f3002-1025-4c9f-80ec-f579427b7d4d");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "D9CBECB0-5A1E-41A7-A122-D3E1B0B73D94",
                column: "ConcurrencyStamp",
                value: "27db5fac-c103-421d-a872-67fe2c5c7c97");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectAnswer",
                table: "Questions");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "B0BAB2ED-F61E-4E83-81D7-7B96267D473B",
                column: "ConcurrencyStamp",
                value: "178ffc1a-0a64-431b-849b-089f10b3a1b7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "D3FCF42A-22C1-455E-825B-2BF65AA877FE",
                column: "ConcurrencyStamp",
                value: "65c70bef-57b9-4d08-a14e-e2eca0ce3d84");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "D9CBECB0-5A1E-41A7-A122-D3E1B0B73D94",
                column: "ConcurrencyStamp",
                value: "0516870c-3eaf-45bc-8983-746856265332");
        }
    }
}
