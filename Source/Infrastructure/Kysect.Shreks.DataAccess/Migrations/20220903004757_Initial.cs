using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kysect.Shreks.DataAccess.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudentGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

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
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectCourses_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionEvaluator",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    SortingOrder = table.Column<int>(type: "integer", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
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
                name: "Students",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Students_StudentGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAssociations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    GithubUsername = table.Column<string>(type: "text", nullable: true),
                    UniversityId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAssociations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAssociations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    ShortName = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    MinPoints = table.Column<double>(type: "double precision", nullable: false),
                    MaxPoints = table.Column<double>(type: "double precision", nullable: false),
                    SubjectCourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_SubjectCourses_SubjectCourseId",
                        column: x => x.SubjectCourseId,
                        principalTable: "SubjectCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeadlinePolicy",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpanBeforeActivation = table.Column<long>(type: "bigint", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    SubjectCourseId = table.Column<Guid>(type: "uuid", nullable: true),
                    AbsoluteValue = table.Column<double>(type: "double precision", nullable: true),
                    Cap = table.Column<double>(type: "double precision", nullable: true),
                    Fraction = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeadlinePolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeadlinePolicy_SubjectCourses_SubjectCourseId",
                        column: x => x.SubjectCourseId,
                        principalTable: "SubjectCourses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Mentors",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mentors", x => new { x.UserId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_Mentors_SubjectCourses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "SubjectCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mentors_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubjectCourseAssociations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectCourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    GithubOrganizationName = table.Column<string>(type: "text", nullable: true),
                    TemplateRepositoryName = table.Column<string>(type: "text", nullable: true),
                    SpreadsheetId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectCourseAssociations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectCourseAssociations_SubjectCourses_SubjectCourseId",
                        column: x => x.SubjectCourseId,
                        principalTable: "SubjectCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubjectCourseGroups",
                columns: table => new
                {
                    StudentGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectCourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    QueueId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectCourseGroups", x => new { x.SubjectCourseId, x.StudentGroupId });
                    table.ForeignKey(
                        name: "FK_SubjectCourseGroups_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectCourseGroups_SubjectCourses_SubjectCourseId",
                        column: x => x.SubjectCourseId,
                        principalTable: "SubjectCourses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubjectCourseGroups_SubmissionQueues_QueueId",
                        column: x => x.QueueId,
                        principalTable: "SubmissionQueues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "GroupAssignments",
                columns: table => new
                {
                    AssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Deadline = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAssignments", x => new { x.GroupId, x.AssignmentId });
                    table.ForeignKey(
                        name: "FK_GroupAssignments_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupAssignments_StudentGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<int>(type: "integer", nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StudentUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupAssignmentGroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupAssignmentAssignmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: true),
                    ExtraPoints = table.Column<double>(type: "double precision", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_GroupAssignments_GroupAssignmentGroupId_GroupAs~",
                        columns: x => new { x.GroupAssignmentGroupId, x.GroupAssignmentAssignmentId },
                        principalTable: "GroupAssignments",
                        principalColumns: new[] { "GroupId", "AssignmentId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Submissions_Students_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "Students",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionAssociations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    Organization = table.Column<string>(type: "text", nullable: true),
                    Repository = table.Column<string>(type: "text", nullable: true),
                    PrNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionAssociations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionAssociations_Submissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssignmentAssignmentGroupFilter_FiltersId",
                table: "AssignmentAssignmentGroupFilter",
                column: "FiltersId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_ShortName_SubjectCourseId",
                table: "Assignments",
                columns: new[] { "ShortName", "SubjectCourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_SubjectCourseId",
                table: "Assignments",
                column: "SubjectCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_DeadlinePolicy_SubjectCourseId",
                table: "DeadlinePolicy",
                column: "SubjectCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAssignments_AssignmentId",
                table: "GroupAssignments",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupQueueFilterStudentGroup_GroupsId",
                table: "GroupQueueFilterStudentGroup",
                column: "GroupsId");

            migrationBuilder.CreateIndex(
                name: "IX_Mentors_CourseId",
                table: "Mentors",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_GroupId",
                table: "Students",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCourseAssociations_GithubOrganizationName",
                table: "SubjectCourseAssociations",
                column: "GithubOrganizationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCourseAssociations_SubjectCourseId_Discriminator",
                table: "SubjectCourseAssociations",
                columns: new[] { "SubjectCourseId", "Discriminator" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCourseGroups_QueueId",
                table: "SubjectCourseGroups",
                column: "QueueId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCourseGroups_StudentGroupId",
                table: "SubjectCourseGroups",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectCourses_SubjectId",
                table: "SubjectCourses",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionAssociations_SubmissionId_Discriminator",
                table: "SubmissionAssociations",
                columns: new[] { "SubmissionId", "Discriminator" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionEvaluator_SubmissionQueueId",
                table: "SubmissionEvaluator",
                column: "SubmissionQueueId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionQueueFilter_SubmissionQueueId",
                table: "SubmissionQueueFilter",
                column: "SubmissionQueueId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_GroupAssignmentGroupId_GroupAssignmentAssignmen~",
                table: "Submissions",
                columns: new[] { "GroupAssignmentGroupId", "GroupAssignmentAssignmentId" });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_StudentUserId",
                table: "Submissions",
                column: "StudentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssociations_GithubUsername",
                table: "UserAssociations",
                column: "GithubUsername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAssociations_UniversityId",
                table: "UserAssociations",
                column: "UniversityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAssociations_UserId_Discriminator",
                table: "UserAssociations",
                columns: new[] { "UserId", "Discriminator" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssignmentAssignmentGroupFilter");

            migrationBuilder.DropTable(
                name: "DeadlinePolicy");

            migrationBuilder.DropTable(
                name: "GroupQueueFilterStudentGroup");

            migrationBuilder.DropTable(
                name: "Mentors");

            migrationBuilder.DropTable(
                name: "SubjectCourseAssociations");

            migrationBuilder.DropTable(
                name: "SubjectCourseGroups");

            migrationBuilder.DropTable(
                name: "SubmissionAssociations");

            migrationBuilder.DropTable(
                name: "SubmissionEvaluator");

            migrationBuilder.DropTable(
                name: "UserAssociations");

            migrationBuilder.DropTable(
                name: "SubmissionQueueFilter");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "SubmissionQueues");

            migrationBuilder.DropTable(
                name: "GroupAssignments");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "StudentGroups");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SubjectCourses");

            migrationBuilder.DropTable(
                name: "Subjects");
        }
    }
}
