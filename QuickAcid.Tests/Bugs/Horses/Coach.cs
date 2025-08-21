namespace QuickAcid.Tests.Bugs.Horses;

public class Coach
{
    public int Id { get; private set; }

    private List<string> Competencies = new();

    public IReadOnlyList<string> competencies { get { return Competencies; } }

    private List<Booking> Bookings = new();

    public IReadOnlyList<Booking> bookings => Bookings;

    public string Name { get; }

    public EmailAddress Email { get; }

    // public List<Course> CourseList { get; private set; } = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public Coach() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


    public Coach(int id, string name, string mail)
    {
        Name = name;
        Email = EmailAddress.From(mail);
        Id = id;
        // CourseList = new();
    }

    public void AddCompetence(string comp)
    {
        if (!Competencies.Contains(comp))
        {
            Competencies.Add(comp);
        }
        else
            throw new Exception($"Coach {Name} already has this competence.");
    }

    public void RemoveCompetence(string comp)
    {
        if (!Competencies.Remove(comp)) { throw new Exception($"Coach {Name} does not have this competence."); }
    }

    public void BookIn(Booking newbooking)
    {
        if (!Bookings.Any(booking => booking.BookingOverlap(newbooking))) { Bookings.Add(newbooking); }
        else throw new CoachAlreadyBooked(); //Exception("Coach's schedule does not match with this planning.");
    }

    public bool IsCompetent(List<string> requirements)
    {
        var competent = requirements.All(c => Competencies.Contains(c));

        return competent;
    }

    public void OverwriteCompetenties(List<string> newComps)
    {
        Competencies = [];
        foreach (var comp in newComps)
        {
            AddCompetence(comp);
        }
    }

    // public void AddCourse(Course course)
    // {
    //     CourseList.Add(course);
    // }
}

public class CoachAlreadyBooked : Exception { }
