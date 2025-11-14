using QuickAcid.Lab.HorsesForCourses.Abstractions;
using QuickAcid.Lab.HorsesForCourses.Domain.Coaches.InvalidationReasons;
using QuickAcid.Lab.HorsesForCourses.Domain.Courses;
using QuickAcid.Lab.HorsesForCourses.Domain.Skills;
using QuickAcid.Lab.HorsesForCourses.ValidationHelpers;
using QuickAcid.Lab.HorsesForCourses.Domain.Accounts;

namespace QuickAcid.Lab.HorsesForCourses.Domain.Coaches;

public class Coach : DomainEntity<Coach>
{
    public CoachName Name { get; init; } = CoachName.Empty;
    public CoachEmail Email { get; init; } = CoachEmail.Empty;

    public IReadOnlySet<Skill> Skills => skills;
    private readonly HashSet<Skill> skills = [];

    public IReadOnlyList<Course> AssignedCourses => assignedCourses;
    private readonly List<Course> assignedCourses = [];

    private Coach() { /*** EFC Was Here ****/ }
    protected Coach(string name, string email)
    {
        Name = new CoachName(name);
        Email = new CoachEmail(email);
    }

    public static Coach Create(Actor actor, string name, string email)
    {
        OnlyActorsWithAdminRoleCanCreateCoach();
        return new(name, email);
        void OnlyActorsWithAdminRoleCanCreateCoach()
            => actor.CanCreateCoach();
    }

    public virtual Coach UpdateSkills(Actor actor, IEnumerable<string> newSkills)
    {
        OnlyAdminsAndActorsWhoRegisteredAsCoachCanEdit();
        NotAllowedWhenThereAreDuplicateSkills();
        OverwriteSkills();
        return this;
        // ------------------------------------------------------------------------------------------------
        // --
        void OnlyAdminsAndActorsWhoRegisteredAsCoachCanEdit()
            => actor.CanEditCoach(Email.Value);
        void NotAllowedWhenThereAreDuplicateSkills()
            => newSkills.NoDuplicatesAllowed(a => new CoachAlreadyHasSkill(string.Join(",", a)));
        void OverwriteSkills()
        {
            skills.Clear();
            newSkills.Select(Skill.From)
                .ToList()
                .ForEach(a => skills.Add(a));
        }
        // ------------------------------------------------------------------------------------------------
    }

    public bool IsSuitableFor(Course course)
        => course.RequiredSkills.All(Skills.Contains);

    public bool IsAvailableFor(Course course)
        => CheckIf.ImAvailable(this).For(course);

    public void AssignCourse(Course course)
        => assignedCourses.Add(course);
}
