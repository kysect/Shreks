using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kysect.Shreks.DataAccess.Migrations
{
    public partial class AddedCompromisedSubmission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompromised",
                table: "Submissions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompromised",
                table: "Submissions");
        }
    }
}
