using Microsoft.EntityFrameworkCore.Migrations;

namespace GetStat.Api.Migrations
{
    public partial class addTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Tests",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "B0BAB2ED-F61E-4E83-81D7-7B96267D473B",
                column: "ConcurrencyStamp",
                value: "a4999fb8-a112-45c5-8969-ca97d34a26cc");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "D3FCF42A-22C1-455E-825B-2BF65AA877FE",
                column: "ConcurrencyStamp",
                value: "e40a7aa5-f977-497a-8562-15d0ee568ebc");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "D9CBECB0-5A1E-41A7-A122-D3E1B0B73D94",
                column: "ConcurrencyStamp",
                value: "8289eeb7-59db-4132-b9d1-50b67c61944c");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_AccountId",
                table: "Tests",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_AspNetUsers_AccountId",
                table: "Tests",
                column: "AccountId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_AspNetUsers_AccountId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_AccountId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Tests");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "B0BAB2ED-F61E-4E83-81D7-7B96267D473B",
                column: "ConcurrencyStamp",
                value: "ae8a20e1-33d3-4aab-8311-f725c71f7484");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "D3FCF42A-22C1-455E-825B-2BF65AA877FE",
                column: "ConcurrencyStamp",
                value: "25784ee1-0c9e-4817-b0dc-f74ed8a02ff7");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "D9CBECB0-5A1E-41A7-A122-D3E1B0B73D94",
                column: "ConcurrencyStamp",
                value: "6a1c3e14-0651-40ac-ac2f-37bdc2e84fa7");
        }
    }
}
