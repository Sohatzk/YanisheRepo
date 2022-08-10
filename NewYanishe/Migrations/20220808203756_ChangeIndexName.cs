using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewYanishe.Migrations
{
    public partial class ChangeIndexName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Messages_Text",
                table: "Messages",
                newName: "IX_Text");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_Text",
                table: "Messages",
                newName: "IX_Messages_Text");
        }
    }
}
