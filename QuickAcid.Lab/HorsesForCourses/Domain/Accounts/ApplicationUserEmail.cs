using QuickAcid.Lab.HorsesForCourses.Abstractions;
using QuickAcid.Lab.HorsesForCourses.Domain.Accounts.InvalidationReasons;

namespace QuickAcid.Lab.HorsesForCourses.Domain.Accounts;

public record ApplicationUserEmail : DefaultString<ApplicationUserEmailCanNotBeEmpty, ApplicationUserEmailCanNotBeTooLong>
{
    public ApplicationUserEmail(string value) : base(value) { }
    protected ApplicationUserEmail() { }
    public static ApplicationUserEmail Empty => new();
}
