using Microsoft.EntityFrameworkCore.Migrations;

namespace GetStat.Api.Migrations
{
    public partial class init1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "B0BAB2ED-F61E-4E83-81D7-7B96267D473B",
                column: "ConcurrencyStamp",
                value: "bfbdf490-b1b4-4362-8f4f-1de3a08f9cb7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "D3FCF42A-22C1-455E-825B-2BF65AA877FE",
                column: "ConcurrencyStamp",
                value: "ab06659b-afc7-45b6-b8a2-4fbce3c25d83");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "D9CBECB0-5A1E-41A7-A122-D3E1B0B73D94",
                column: "ConcurrencyStamp",
                value: "c071311e-7c1f-4c34-bfe2-83f29ffa2f50");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "B0BAB2ED-F61E-4E83-81D7-7B96267D473B",
                column: "ConcurrencyStamp",
                value: "098f4e70-86f0-4d4c-91f6-f341f80ff32f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "D3FCF42A-22C1-455E-825B-2BF65AA877FE",
                column: "ConcurrencyStamp",
                value: "5adee6d4-263e-4321-8ca9-53e929bdac4a");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "D9CBECB0-5A1E-41A7-A122-D3E1B0B73D94",
                column: "ConcurrencyStamp",
                value: "9c1ffe28-659f-4854-b2ac-5b30a1f5983c");
        }
    }
}
