﻿using Kysect.Shreks.Application.Dto.Tables;

namespace Kysect.Shreks.Integration.Google.Models;

public record struct CourseStudentsDto(IReadOnlyCollection<StudentPointsDto> StudentsPoints);