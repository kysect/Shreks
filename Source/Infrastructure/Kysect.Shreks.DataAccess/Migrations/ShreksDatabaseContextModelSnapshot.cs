﻿// <auto-generated />
using System;
using Kysect.Shreks.DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Kysect.Shreks.DataAccess.Migrations
{
    [DbContext(typeof(ShreksDatabaseContext))]
    partial class ShreksDatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "fuzzystrmatch");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Kysect.Shreks.Core.DeadlinePolicies.DeadlinePolicy", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("SpanBeforeActivation")
                        .HasColumnType("bigint");

                    b.Property<Guid?>("SubjectCourseId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SubjectCourseId");

                    b.ToTable("DeadlinePolicy");

                    b.HasDiscriminator<string>("Discriminator").HasValue("DeadlinePolicy");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.Assignment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<double>("MaxPoints")
                        .HasColumnType("double precision");

                    b.Property<double>("MinPoints")
                        .HasColumnType("double precision");

                    b.Property<int>("Order")
                        .HasColumnType("integer");

                    b.Property<string>("ShortName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("SubjectCourseId")
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("SubjectCourseId");

                    b.HasIndex("ShortName", "SubjectCourseId")
                        .IsUnique();

                    b.ToTable("Assignments");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.GroupAssignment", b =>
                {
                    b.Property<Guid>("GroupId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AssignmentId")
                        .HasColumnType("uuid");

                    b.Property<DateOnly>("Deadline")
                        .HasColumnType("date");

                    b.HasKey("GroupId", "AssignmentId");

                    b.HasIndex("AssignmentId");

                    b.ToTable("GroupAssignments");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.StudentGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("StudentGroups");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.Subject", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.SubjectCourse", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("SubjectId")
                        .HasColumnType("uuid");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("WorkflowType")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SubjectId");

                    b.ToTable("SubjectCourses");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.SubjectCourseGroup", b =>
                {
                    b.Property<Guid>("SubjectCourseId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("StudentGroupId")
                        .HasColumnType("uuid");

                    b.HasKey("SubjectCourseId", "StudentGroupId");

                    b.HasIndex("StudentGroupId");

                    b.ToTable("SubjectCourseGroups");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.SubjectCourseAssociations.SubjectCourseAssociation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("SubjectCourseId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SubjectCourseId", "Discriminator")
                        .IsUnique();

                    b.ToTable("SubjectCourseAssociations");

                    b.HasDiscriminator<string>("Discriminator").HasValue("SubjectCourseAssociation");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.SubmissionAssociations.SubmissionAssociation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("SubmissionId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("SubmissionId", "Discriminator")
                        .IsUnique();

                    b.ToTable("SubmissionAssociations");

                    b.HasDiscriminator<string>("Discriminator").HasValue("SubmissionAssociation");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Submissions.Submission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("Code")
                        .HasColumnType("integer");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double?>("ExtraPoints")
                        .HasColumnType("double precision");

                    b.Property<Guid>("GroupAssignmentAssignmentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("GroupAssignmentGroupId")
                        .HasColumnType("uuid");

                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double?>("Rating")
                        .HasColumnType("double precision");

                    b.Property<int>("State")
                        .HasColumnType("integer");

                    b.Property<Guid>("StudentUserId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("SubmissionDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("StudentUserId");

                    b.HasIndex("GroupAssignmentGroupId", "GroupAssignmentAssignmentId");

                    b.ToTable("Submissions");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Submission");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.UserAssociations.UserAssociation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId", "Discriminator")
                        .IsUnique();

                    b.ToTable("UserAssociations");

                    b.HasDiscriminator<string>("Discriminator").HasValue("UserAssociation");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Users.Mentor", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId", "CourseId");

                    b.HasIndex("CourseId");

                    b.ToTable("Mentors");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Users.Student", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uuid");

                    b.HasKey("UserId");

                    b.HasIndex("GroupId");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MiddleName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.DeadlinePolicies.AbsoluteDeadlinePolicy", b =>
                {
                    b.HasBaseType("Kysect.Shreks.Core.DeadlinePolicies.DeadlinePolicy");

                    b.Property<double>("AbsoluteValue")
                        .HasColumnType("double precision");

                    b.HasDiscriminator().HasValue("AbsoluteDeadlinePolicy");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.DeadlinePolicies.CappingDeadlinePolicy", b =>
                {
                    b.HasBaseType("Kysect.Shreks.Core.DeadlinePolicies.DeadlinePolicy");

                    b.Property<double>("Cap")
                        .HasColumnType("double precision");

                    b.HasDiscriminator().HasValue("CappingDeadlinePolicy");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.DeadlinePolicies.FractionDeadlinePolicy", b =>
                {
                    b.HasBaseType("Kysect.Shreks.Core.DeadlinePolicies.DeadlinePolicy");

                    b.Property<double>("Fraction")
                        .HasColumnType("double precision");

                    b.HasDiscriminator().HasValue("FractionDeadlinePolicy");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.SubjectCourseAssociations.GithubSubjectCourseAssociation", b =>
                {
                    b.HasBaseType("Kysect.Shreks.Core.SubjectCourseAssociations.SubjectCourseAssociation");

                    b.Property<string>("GithubOrganizationName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TemplateRepositoryName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasIndex("GithubOrganizationName")
                        .IsUnique();

                    b.HasDiscriminator().HasValue("GithubSubjectCourseAssociation");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.SubjectCourseAssociations.GoogleTableSubjectCourseAssociation", b =>
                {
                    b.HasBaseType("Kysect.Shreks.Core.SubjectCourseAssociations.SubjectCourseAssociation");

                    b.Property<string>("SpreadsheetId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasDiscriminator().HasValue("GoogleTableSubjectCourseAssociation");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.SubmissionAssociations.GithubSubmissionAssociation", b =>
                {
                    b.HasBaseType("Kysect.Shreks.Core.SubmissionAssociations.SubmissionAssociation");

                    b.Property<string>("Organization")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("PrNumber")
                        .HasColumnType("bigint");

                    b.Property<string>("Repository")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasDiscriminator().HasValue("GithubSubmissionAssociation");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Submissions.GithubSubmission", b =>
                {
                    b.HasBaseType("Kysect.Shreks.Core.Submissions.Submission");

                    b.HasDiscriminator().HasValue("GithubSubmission");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.UserAssociations.GithubUserAssociation", b =>
                {
                    b.HasBaseType("Kysect.Shreks.Core.UserAssociations.UserAssociation");

                    b.Property<string>("GithubUsername")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasIndex("GithubUsername")
                        .IsUnique();

                    b.HasDiscriminator().HasValue("GithubUserAssociation");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.UserAssociations.IsuUserAssociation", b =>
                {
                    b.HasBaseType("Kysect.Shreks.Core.UserAssociations.UserAssociation");

                    b.Property<int>("UniversityId")
                        .HasColumnType("integer");

                    b.HasIndex("UniversityId")
                        .IsUnique();

                    b.HasDiscriminator().HasValue("IsuUserAssociation");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.DeadlinePolicies.DeadlinePolicy", b =>
                {
                    b.HasOne("Kysect.Shreks.Core.Study.SubjectCourse", null)
                        .WithMany("DeadlinePolicies")
                        .HasForeignKey("SubjectCourseId");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.Assignment", b =>
                {
                    b.HasOne("Kysect.Shreks.Core.Study.SubjectCourse", "SubjectCourse")
                        .WithMany("Assignments")
                        .HasForeignKey("SubjectCourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("SubjectCourse");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.GroupAssignment", b =>
                {
                    b.HasOne("Kysect.Shreks.Core.Study.Assignment", "Assignment")
                        .WithMany("GroupAssignments")
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Kysect.Shreks.Core.Study.StudentGroup", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Assignment");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.SubjectCourse", b =>
                {
                    b.HasOne("Kysect.Shreks.Core.Study.Subject", "Subject")
                        .WithMany("Courses")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.SubjectCourseGroup", b =>
                {
                    b.HasOne("Kysect.Shreks.Core.Study.StudentGroup", "StudentGroup")
                        .WithMany()
                        .HasForeignKey("StudentGroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Kysect.Shreks.Core.Study.SubjectCourse", "SubjectCourse")
                        .WithMany("Groups")
                        .HasForeignKey("SubjectCourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("StudentGroup");

                    b.Navigation("SubjectCourse");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.SubjectCourseAssociations.SubjectCourseAssociation", b =>
                {
                    b.HasOne("Kysect.Shreks.Core.Study.SubjectCourse", "SubjectCourse")
                        .WithMany("Associations")
                        .HasForeignKey("SubjectCourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("SubjectCourse");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.SubmissionAssociations.SubmissionAssociation", b =>
                {
                    b.HasOne("Kysect.Shreks.Core.Submissions.Submission", "Submission")
                        .WithMany("Associations")
                        .HasForeignKey("SubmissionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Submission");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Submissions.Submission", b =>
                {
                    b.HasOne("Kysect.Shreks.Core.Users.Student", "Student")
                        .WithMany()
                        .HasForeignKey("StudentUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Kysect.Shreks.Core.Study.GroupAssignment", "GroupAssignment")
                        .WithMany("Submissions")
                        .HasForeignKey("GroupAssignmentGroupId", "GroupAssignmentAssignmentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("GroupAssignment");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.UserAssociations.UserAssociation", b =>
                {
                    b.HasOne("Kysect.Shreks.Core.Users.User", "User")
                        .WithMany("Associations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Users.Mentor", b =>
                {
                    b.HasOne("Kysect.Shreks.Core.Study.SubjectCourse", "Course")
                        .WithMany("Mentors")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Kysect.Shreks.Core.Users.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Users.Student", b =>
                {
                    b.HasOne("Kysect.Shreks.Core.Study.StudentGroup", "Group")
                        .WithMany("Students")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Kysect.Shreks.Core.Users.User", "User")
                        .WithOne()
                        .HasForeignKey("Kysect.Shreks.Core.Users.Student", "UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.Assignment", b =>
                {
                    b.Navigation("GroupAssignments");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.GroupAssignment", b =>
                {
                    b.Navigation("Submissions");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.StudentGroup", b =>
                {
                    b.Navigation("Students");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.Subject", b =>
                {
                    b.Navigation("Courses");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Study.SubjectCourse", b =>
                {
                    b.Navigation("Assignments");

                    b.Navigation("Associations");

                    b.Navigation("DeadlinePolicies");

                    b.Navigation("Groups");

                    b.Navigation("Mentors");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Submissions.Submission", b =>
                {
                    b.Navigation("Associations");
                });

            modelBuilder.Entity("Kysect.Shreks.Core.Users.User", b =>
                {
                    b.Navigation("Associations");
                });
#pragma warning restore 612, 618
        }
    }
}
