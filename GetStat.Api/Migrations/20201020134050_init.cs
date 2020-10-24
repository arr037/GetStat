﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GetStat.Api.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Admins",
                table => new
                {
                    AdminId = table.Column<Guid>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Admins", x => x.AdminId); });

            migrationBuilder.CreateTable(
                "AspNetRoles",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_AspNetRoles", x => x.Id); });

            migrationBuilder.CreateTable(
                "AspNetUsers",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Surname = table.Column<string>(nullable: false),
                    MiddleName = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_AspNetUsers", x => x.Id); });

            migrationBuilder.CreateTable(
                "Groups",
                table => new
                {
                    GroupId = table.Column<Guid>(nullable: false),
                    GroupName = table.Column<string>(nullable: true),
                    StudentCount = table.Column<int>(nullable: false),
                    Course = table.Column<int>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Groups", x => x.GroupId); });

            migrationBuilder.CreateTable(
                "Moderators",
                table => new
                {
                    ModeratorId = table.Column<Guid>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Moderators", x => x.ModeratorId); });

            migrationBuilder.CreateTable(
                "Teachers",
                table => new
                {
                    TeacherId = table.Column<Guid>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Teachers", x => x.TeacherId); });

            migrationBuilder.CreateTable(
                "Tutors",
                table => new
                {
                    TutorId = table.Column<Guid>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Tutors", x => x.TutorId); });

            migrationBuilder.CreateTable(
                "AspNetRoleClaims",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        x => x.RoleId,
                        "AspNetRoles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserClaims",
                table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        "FK_AspNetUserClaims_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserLogins",
                table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new {x.LoginProvider, x.ProviderKey});
                    table.ForeignKey(
                        "FK_AspNetUserLogins_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserRoles",
                table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new {x.UserId, x.RoleId});
                    table.ForeignKey(
                        "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        x => x.RoleId,
                        "AspNetRoles",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_AspNetUserRoles_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "AspNetUserTokens",
                table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new {x.UserId, x.LoginProvider, x.Name});
                    table.ForeignKey(
                        "FK_AspNetUserTokens_AspNetUsers_UserId",
                        x => x.UserId,
                        "AspNetUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Students",
                table => new
                {
                    StudentId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    GroupId = table.Column<Guid>(nullable: true),
                    TutorId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                    table.ForeignKey(
                        "FK_Students_Groups_GroupId",
                        x => x.GroupId,
                        "Groups",
                        "GroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        "FK_Students_Tutors_TutorId",
                        x => x.TutorId,
                        "Tutors",
                        "TutorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                "AspNetRoles",
                new[] {"Id", "ConcurrencyStamp", "Name", "NormalizedName"},
                new object[,]
                {
                    {"2BF94763-FE0B-48CD-B413-C8BC705380DC", "5ae93f04-7643-4f09-888a-e4d1236f5d48", "User", "USER"},
                    {
                        "CD03DD4F-E829-4784-869D-AE4612A7EA0F", "287845e0-a00b-465b-9991-c9c0489dcf4e", "Student",
                        "STUDENT"
                    },
                    {
                        "BA6736CC-6D97-4314-92D3-803655469BC1", "4fb9914d-893a-4126-8c66-5717501de394", "Teacher",
                        "TEACHER"
                    },
                    {"3C3C481E-C832-42A9-B18D-2A157CC09457", "51633455-5e53-4c8b-992a-284b21429b3b", "Tutor", "TUTOR"},
                    {
                        "D3FCF42A-22C1-455E-825B-2BF65AA877FE", "ccfce3e5-556c-47cb-b7e7-e70d42734644", "Moderator",
                        "MODERATOR"
                    },
                    {"B0BAB2ED-F61E-4E83-81D7-7B96267D473B", "852d2ba4-2f00-401d-a3e1-3a90fae30986", "Admin", "ADMIN"},
                    {
                        "D9CBECB0-5A1E-41A7-A122-D3E1B0B73D94", "fa194670-4626-4ce4-84ef-c58f083c85c6", "MainAdmin",
                        "MAINADMIN"
                    }
                });

            migrationBuilder.InsertData(
                "AspNetUsers",
                new[]
                {
                    "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled",
                    "LockoutEnd", "MiddleName", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash",
                    "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Surname", "TwoFactorEnabled", "UserName"
                },
                new object[]
                {
                    "9FE2BC2C-82E6-4724-9F08-B8B6B279662D", 0, "400cf447-feee-4694-aacd-014e1813f6c1",
                    "arr073099@mail.ru", true, false, null, "Ravilevich", "Ruslan", "ARR073099@MAIL.RU", "RUSLAN",
                    "AQAAAAEAACcQAAAAEEM4mxJlKMecuqjgd40ED03MDc1dsQNP/H98Z206MIxLcZfGnXpYxugPjlSG3RPTrQ==", null, false,
                    "", "Akhmetov", false, "Akhmet0ff"
                });

            migrationBuilder.InsertData(
                "AspNetUserRoles",
                new[] {"UserId", "RoleId"},
                new object[] {"9FE2BC2C-82E6-4724-9F08-B8B6B279662D", "D9CBECB0-5A1E-41A7-A122-D3E1B0B73D94"});

            migrationBuilder.CreateIndex(
                "IX_AspNetRoleClaims_RoleId",
                "AspNetRoleClaims",
                "RoleId");

            migrationBuilder.CreateIndex(
                "RoleNameIndex",
                "AspNetRoles",
                "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserClaims_UserId",
                "AspNetUserClaims",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserLogins_UserId",
                "AspNetUserLogins",
                "UserId");

            migrationBuilder.CreateIndex(
                "IX_AspNetUserRoles_RoleId",
                "AspNetUserRoles",
                "RoleId");

            migrationBuilder.CreateIndex(
                "EmailIndex",
                "AspNetUsers",
                "NormalizedEmail");

            migrationBuilder.CreateIndex(
                "UserNameIndex",
                "AspNetUsers",
                "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                "IX_Students_GroupId",
                "Students",
                "GroupId");

            migrationBuilder.CreateIndex(
                "IX_Students_TutorId",
                "Students",
                "TutorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Admins");

            migrationBuilder.DropTable(
                "AspNetRoleClaims");

            migrationBuilder.DropTable(
                "AspNetUserClaims");

            migrationBuilder.DropTable(
                "AspNetUserLogins");

            migrationBuilder.DropTable(
                "AspNetUserRoles");

            migrationBuilder.DropTable(
                "AspNetUserTokens");

            migrationBuilder.DropTable(
                "Moderators");

            migrationBuilder.DropTable(
                "Students");

            migrationBuilder.DropTable(
                "Teachers");

            migrationBuilder.DropTable(
                "AspNetRoles");

            migrationBuilder.DropTable(
                "AspNetUsers");

            migrationBuilder.DropTable(
                "Groups");

            migrationBuilder.DropTable(
                "Tutors");
        }
    }
}