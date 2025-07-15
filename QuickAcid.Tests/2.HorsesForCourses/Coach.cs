

namespace HorsesForCourses.Core;


public class Coach
{
    public string Name { get; }
    public string Email { get; }
    public HashSet<string> Skills { get; }

    public List<Course> AssignedCourses { get; }

    public Coach(string name, string email)
    {
        Name = name;                        // ?? throw new ArgumentNullException(nameof(name));
        Email = email;                      // ?? throw new ArgumentNullException(nameof(email));
        Skills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);      // ?? throw new ArgumentNullException(nameof(skills));
        AssignedCourses = [];               // ?? throw new ArgumentNullException(nameof(assignedCourse));   
    }

    public void AddSkill(string skill)
    {
        if (Skills.Contains(skill))
            throw new ArgumentException("Skill already added");

        Skills.Add(skill);
    }

    public void RemoveSkill(string skill)
    {
        if (!Skills.Contains(skill))
            throw new ArgumentException("There is no skill in list");

        Skills.Remove(skill);
    }


    public bool HasAllSkills(IEnumerable<string> requiredSkills)
        => requiredSkills.All(skill => Skills.Contains(skill));

    public void AssignCourse(Course course)
    {
        if (AssignedCourses.Contains(course))
            throw new ArgumentException("Course is already assinged");

        AssignedCourses.Add(course);
    }

    // AI helped
    public bool IsAvailableCoach()
    {
        if (!AssignedCourses.Any()) return true;

        var allTimeSlots = AssignedCourses.SelectMany(c => c.Schedule).ToList();

        for (int i = 0; i < allTimeSlots.Count; i++)
        {
            for (int j = i + 1; j < allTimeSlots.Count; j++)
            {
                if (AreTimeSlotsOverlapping(allTimeSlots[i], allTimeSlots[j]))
                    throw new ArgumentException("Lesson time is overlapping");
            }
        }

        return true;
    }

    private bool AreTimeSlotsOverlapping(TimeSlot slot1, TimeSlot slot2)
    {
        if (slot1.Day != slot2.Day) return false;

        return slot1.Start < slot2.End && slot1.End > slot2.Start;    
    }
}


