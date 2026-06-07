using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddJwtTokenBlackList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "test");

            migrationBuilder.CreateTable(
                name: "jwt_token_blacklist",
                schema: "test",
                columns: table => new
                {
                    Token = table.Column<string>(type: "text", nullable: false),
                    expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jwt_token_blacklist", x => x.Token);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jwt_token_blacklist",
                schema: "test");
        }
    }
}
