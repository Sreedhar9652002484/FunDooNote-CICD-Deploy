using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace RepoLayer.Migrations
{
    public partial class NotesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.CreateTable(
            name: "Notes",
            columns: table => new
            {
                NotesId = table.Column<long>(nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
                Title = table.Column<string>(nullable: true),
                TakeANote = table.Column<string>(nullable: true),
                Remainder = table.Column<DateTime>(nullable: false),
                Colour = table.Column<string>(nullable: true),
                Image = table.Column<string>(nullable: true),
                IsArchive = table.Column<bool>(nullable: false),
                IsPin = table.Column<bool>(nullable: false),
                IsTrash = table.Column<bool>(nullable: false),
                UserId = table.Column<long>(nullable: false)
            });
            migrationBuilder.AddForeignKey(
                name: "FK_Notes_User_UserId",
                table: "Notes",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.CreateIndex(
                    name: "IX_Notes_UserId",
                    table: "Notes",
                    column: "UserId");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_User_UserId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Notes_UserId",
                table: "Notes");
        }
    }
}
