using ITMO.Dev.ASAP.Application.Dto.Study;
using ITMO.Dev.ASAP.Application.Dto.SubjectCourses;

namespace ITMO.Dev.ASAP.WebUI.AdminPanel.Models;

public record ExtendedSubjectCourseGroupDto(SubjectCourseGroupDto SubjectCourseGroup, StudyGroupDto Group);