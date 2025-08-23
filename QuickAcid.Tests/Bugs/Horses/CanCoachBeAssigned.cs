using QuickAcid;
using QuickAcid.Shrinking.Custom;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;
using WibblyWobbly;

namespace QuickAcid.Tests.Bugs.Horses;

public class CanCoachBeAssigned
{

    public class TimeSlotShrinker : IShrinker<TimeSlot>
    {
        public IEnumerable<TimeSlot> Shrink(TimeSlot value)
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void LetsSee()
    {
        var script =
            from coach in "Coach".Stashed(() =>
                {
                    var coach = new Coach(1, "coach", "coach@coaching.com");
                    return coach;
                })
            from booking in "Booking".Input(BookingGenerator)
            from bookCoach in "Book the Coach".Act(() => coach.BookIn(booking))
            select Acid.Test;
        QState.Run(script, 906172892)
            .Options(a => a with { FileAs = "CanCoachBeAssigned" })
            .With(10.Runs())
            .And(5.ExecutionsPerRun());
    }

    private static readonly Generator<DayOfWeek> WeekDayGenerator =
        Fuzz.Enum<DayOfWeek>().Where(a => a != DayOfWeek.Saturday && a != DayOfWeek.Sunday);

    private static Generator<TimeSlot> TimeslotGenerator =>
        from start in Fuzz.Int(9, 17)
        let startTime = start.OClock()
        from end in Fuzz.Int(start + 1, 18)
        let endTime = end.OClock()
        from day in WeekDayGenerator
        from timeslot in Fuzz.Constant(TimeSlot.From(day, startTime, endTime))
        select timeslot;

    private static Generator<Booking> BookingGenerator =>
        from start in Fuzz.Int(1, 31)
        let startDate = DateOnly.FromDateTime(start.January(2025))
        from end in Fuzz.Int(start, 32)
        let endDate = DateOnly.FromDateTime(end.January(2025))
        from key in Fuzz.Int().Unique("booking")
        from timeslots in TimeslotGenerator.Many(1, 5)
        from booking in Fuzz.Constant(new Booking([.. timeslots], startDate, endDate))
        select booking;
}