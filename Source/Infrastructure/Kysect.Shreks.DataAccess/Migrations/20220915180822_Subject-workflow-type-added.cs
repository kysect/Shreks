using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kysect.Shreks.DataAccess.Migrations;

public partial class Subjectworkflowtypeadded : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "WorkflowType",
            table: "SubjectCourses",
            type: "integer",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "WorkflowType",
            table: "SubjectCourses");
    }
}
