using Kysect.Shreks.WebApi.Sdk;

namespace Kysect.Shreks.WebUI.AdminPanel.Models;

public record ExtendedSubjectCourseGroupDto(SubjectCourseGroupDto SubjectCourseGroup, StudyGroupDto Group);