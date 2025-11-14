using QuickAcid.Lab.HorsesForCourses.Abstractions;
using QuickAcid.Lab.HorsesForCourses.Domain.Courses.InvalidationReasons;

namespace QuickAcid.Lab.HorsesForCourses.Domain.Courses;

public record CourseName : DefaultString<CourseNameCanNotBeEmpty, CourseNameCanNotBeTooLong>
{
    public CourseName(string value) : base(value) { }
    protected CourseName() { }
    public static CourseName Empty => new();
}
