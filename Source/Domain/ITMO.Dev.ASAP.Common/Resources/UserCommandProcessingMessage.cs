namespace ITMO.Dev.ASAP.Common.Resources;

public static class UserCommandProcessingMessage
{
    public static string DomainExceptionWhileProcessingUserCommand(string commandName)
    {
        return string.Format(UserMessages.DomainExceptionWhileProcessingUserCommand, commandName);
    }

    public static string InternalExceptionWhileProcessingUserCommand()
    {
        return string.Format(UserMessages.InternalExceptionWhileProcessingUserCommand);
    }

    public static string SubmissionActivatedSuccessfully()
    {
        return string.Format(UserMessages.SubmissionActivatedSuccessfully);
    }

    public static string SubmissionCreated(string submissionDetails)
    {
        return string.Format(UserMessages.SubmissionCreated, submissionDetails);
    }

    public static string SubmissionDeactivatedSuccessfully()
    {
        return string.Format(UserMessages.SubmissionDeactivatedSuccessfully);
    }

    public static string SubmissionDeletedSuccessfully()
    {
        return string.Format(UserMessages.SubmissionDeletedSuccessfully);
    }

    public static string SubmissionRated(string submissionDetails)
    {
        return string.Format(UserMessages.SubmissionRated, submissionDetails);
    }

    public static string SubmissionUpdated(string submissionDetails)
    {
        return string.Format(UserMessages.SubmissionUpdated, submissionDetails);
    }

    public static string SubmissionMarkedAsReviewed()
    {
        return string.Format(UserMessages.SubmissionMarkedAsReviewed);
    }

    public static string ReviewRatedSubmission(double points)
    {
        return string.Format(UserMessages.ReviewRatedSubmission, points);
    }

    public static string ReviewWithoutRate()
    {
        return UserMessages.ReviewWithoutRate;
    }

    public static string MergePullRequestWithoutRate(int submissionCode)
    {
        return string.Format(UserMessages.MergePullRequestWithoutRate, submissionCode);
    }

    public static string SubmissionMarkAsReviewedAndNeedDefense()
    {
        return UserMessages.SubmissionMarkAsReviewedAndNeedDefense;
    }

    public static string ClosePullRequestWithUnratedSubmission(int submissionCode)
    {
        return string.Format(UserMessages.ClosePullRequestWithUnratedSubmission, submissionCode);
    }

    public static string MergePullRequestAndMarkAsCompleted()
    {
        return UserMessages.MergePullRequestAndMarkAsCompleted;
    }

    public static string MentorMergeUnratedSubmission()
    {
        return UserMessages.MentorMergeUnratedSubmission;
    }

    public static string SubmissionBanned(Guid submissionId)
    {
        return string.Format(UserMessages.SubmissionBanned, submissionId);
    }

    public static string SubmissionMarkAsNotAccepted(int submissionCode)
    {
        return string.Format(UserMessages.SubmissionNotAccepted, submissionCode);
    }
}