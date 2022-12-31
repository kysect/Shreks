using Kysect.Shreks.Application.Dto.Study;
using Kysect.Shreks.Application.Dto.SubjectCourses;

namespace Kysect.Shreks.WebUI.AdminPanel.Models;

public record ExtendedSubjectCourseGroupDto(SubjectCourseGroupDto SubjectCourseGroup, StudyGroupDto Group);