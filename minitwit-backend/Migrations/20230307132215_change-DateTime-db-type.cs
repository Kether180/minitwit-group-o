using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiniTwit.Migrations
{
    /// <inheritdoc />
    public partial class changeDateTimedbtype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PubDate",
                table: "Messages",
                type: "Timestamp",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "Date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "PubDate",
                table: "Messages",
                type: "Date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "Timestamp");
        }
    }
}
