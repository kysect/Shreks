using Kysect.Shreks.Application.Dto.Study;

namespace Kysect.Shreks.WebUI.AdminPanel.Models;

public class SelectableStudyGroup
{
    public SelectableStudyGroup(StudyGroupDto group, bool isSelected)
    {
        Group = group;
        IsSelected = isSelected;
    }

    public StudyGroupDto Group { get; set; }

    public bool IsSelected { get; set; }
}