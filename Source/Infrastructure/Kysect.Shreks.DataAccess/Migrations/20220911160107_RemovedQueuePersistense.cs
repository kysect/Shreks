using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kysect.Shreks.DataAccess.Migrations;

public partial class RemovedQueuePersistense : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_SubjectCourseGroups_SubmissionQueues_QueueId",
            table: "SubjectCourseGroups");

        migrationBuilder.DropTable(
            name: "AssignmentAssignmentGroupFilter");

        migrationBuilder.DropTable(
            name: "GroupQueueFilterStudentGroup");

        migrationBuilder.DropTable(
            name: "SubmissionEvaluator");

        migrationBuilder.DropTable(
            name: "SubmissionQueueFilter");

        migrationBuilder.DropTable(
            name: "SubmissionQueues");

        migrationBuilder.DropIndex(
            name: "IX_SubjectCourseGroups_QueueId",
            table: "SubjectCourseGroups");

        migrationBuilder.DropColumn(
            name: "QueueId",
            table: "SubjectCourseGroups");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "QueueId",
            table: "SubjectCourseGroups",
            type: "uuid",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.CreateTable(
            name: "SubmissionQueues",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SubmissionQueues", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "SubmissionEvaluator",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Discriminator = table.Column<string>(type: "text", nullable: false),
                Position = table.Column<int>(type: "integer", nullable: false),
                SortingOrder = table.Column<int>(type: "integer", nullable: false),
                SubmissionQueueId = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SubmissionEvaluator", x => x.Id);
                table.ForeignKey(
                    name: "FK_SubmissionEvaluator_SubmissionQueues_SubmissionQueueId",
                    column: x => x.SubmissionQueueId,
                    principalTable: "SubmissionQueues",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "SubmissionQueueFilter",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Discriminator = table.Column<string>(type: "text", nullable: false),
                SubmissionQueueId = table.Column<Guid>(type: "uuid", nullable: true),
                CourseId = table.Column<Guid>(type: "uuid", nullable: true),
                State = table.Column<int>(type: "integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SubmissionQueueFilter", x => x.Id);
                table.ForeignKey(
                    name: "FK_SubmissionQueueFilter_SubmissionQueues_SubmissionQueueId",
                    column: x => x.SubmissionQueueId,
                    principalTable: "SubmissionQueues",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "AssignmentAssignmentGroupFilter",
            columns: table => new
            {
                AssignmentsId = table.Column<Guid>(type: "uuid", nullable: false),
                FiltersId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AssignmentAssignmentGroupFilter", x => new { x.AssignmentsId, x.FiltersId });
                table.ForeignKey(
                    name: "FK_AssignmentAssignmentGroupFilter_Assignments_AssignmentsId",
                    column: x => x.AssignmentsId,
                    principalTable: "Assignments",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AssignmentAssignmentGroupFilter_SubmissionQueueFilter_Filte~",
                    column: x => x.FiltersId,
                    principalTable: "SubmissionQueueFilter",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "GroupQueueFilterStudentGroup",
            columns: table => new
            {
                FiltersId = table.Column<Guid>(type: "uuid", nullable: false),
                GroupsId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_GroupQueueFilterStudentGroup", x => new { x.FiltersId, x.GroupsId });
                table.ForeignKey(
                    name: "FK_GroupQueueFilterStudentGroup_StudentGroups_GroupsId",
                    column: x => x.GroupsId,
                    principalTable: "StudentGroups",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_GroupQueueFilterStudentGroup_SubmissionQueueFilter_FiltersId",
                    column: x => x.FiltersId,
                    principalTable: "SubmissionQueueFilter",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_SubjectCourseGroups_QueueId",
            table: "SubjectCourseGroups",
            column: "QueueId");

        migrationBuilder.CreateIndex(
            name: "IX_AssignmentAssignmentGroupFilter_FiltersId",
            table: "AssignmentAssignmentGroupFilter",
            column: "FiltersId");

        migrationBuilder.CreateIndex(
            name: "IX_GroupQueueFilterStudentGroup_GroupsId",
            table: "GroupQueueFilterStudentGroup",
            column: "GroupsId");

        migrationBuilder.CreateIndex(
            name: "IX_SubmissionEvaluator_SubmissionQueueId",
            table: "SubmissionEvaluator",
            column: "SubmissionQueueId");

        migrationBuilder.CreateIndex(
            name: "IX_SubmissionQueueFilter_SubmissionQueueId",
            table: "SubmissionQueueFilter",
            column: "SubmissionQueueId");

        migrationBuilder.AddForeignKey(
            name: "FK_SubjectCourseGroups_SubmissionQueues_QueueId",
            table: "SubjectCourseGroups",
            column: "QueueId",
            principalTable: "SubmissionQueues",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }
}
