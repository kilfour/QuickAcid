using QuickAcid.Lab.HorsesForCourses.Abstractions;
using QuickAcid.Lab.HorsesForCourses.Domain.Accounts.InvalidationReasons;

namespace QuickAcid.Lab.HorsesForCourses.Domain.Accounts;

public record ApplicationUserName : DefaultString<ApplicationUserNameCanNotBeEmpty, ApplicationUserNameCanNotBeTooLong>
{
    public ApplicationUserName(string value) : base(value) { }
    protected ApplicationUserName() { }
    public static ApplicationUserName Empty => new();
}
