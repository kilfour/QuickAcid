namespace QuickAcid.Tests.Bugs.Horses;

public record TimeSlot
{
    public DayOfWeek Day { get; set; }

    public TimeOnly Start { get; set; }

    public TimeOnly End { get; set; }

    public TimeSlot(DayOfWeek day, TimeOnly start, TimeOnly end) { Day = day; Start = start; End = end; }

    public static TimeSlot From(DayOfWeek day, TimeOnly start, TimeOnly end)
    {
        if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday) throw new ArgumentException("Course cannot take place during the weekend.");
        if (start < new TimeOnly(9, 0, 0) || end > new TimeOnly(17, 0, 0)) throw new ArgumentException("Course must be planned during working hours (9:00 - 17:00).");
        if (end - start < new TimeSpan(1, 0, 0)) throw new ArgumentException("Course must be one hour minimum.");

        return new TimeSlot(day, start, end);
    }


    public bool Overlap(TimeSlot slot)
    {
        return OverlapEarly(slot) || OverlapAfter(slot) || OverlapContain(slot) || OverlapEqual(slot);
    }

    private bool OverlapEarly(TimeSlot slot)
    {
        return slot.Start < Start && slot.End > Start && slot.Day == Day;
    }

    private bool OverlapContain(TimeSlot slot)
    {
        return (slot.Start > Start && slot.End < End && slot.Day == Day) || (slot.Start == Start && slot.End == End && slot.Day == Day);
    }

    private bool OverlapAfter(TimeSlot slot)
    {
        return slot.Start < End && slot.End > End && slot.Day == Day;
    }

    private bool OverlapEqual(TimeSlot slot)
    {
        return slot.Start == Start && slot.End == End && slot.Day == Day;
    }
}
