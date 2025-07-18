using HorsesForCourses.Core;
using QuickMGenerate;
using QuickAcid;
using QuickMGenerate.Data;
using QuickPulse;
using QuickAcid.Reporting;
using System.Text;
using QuickPulse.Show;


namespace HorsesForCourses.Tests;

public class TheStringCatcher : IArtery
{
    private readonly StringBuilder builder = new StringBuilder();
    public void Flow(params object[] data)
    {
        foreach (var item in data)
        {
            builder.Append(item);
        }
    }
    public string GetText() { return builder.ToString(); }
}

public class InMemoryStorage
{
    public List<Coach> Coaches { get; } = [];
    public Coach AddCoach(Coach coach) { Coaches.Add(coach); return coach; }

}

public class AcidTests
{
    [Fact]
    public void Foo()
    {
        var coachGen =
            from firstName in MGen.ChooseFrom(DataLists.FirstNames).Unique("coach name")
            from email in MGen.Constant($"{firstName}@CoursesGalore.com")
            from _ in MGen.For<Coach>().Construct(() => new Coach(firstName, email))
            from coach in MGen.One<Coach>()
            select coach;
        Signal.ToFile<string>().Pulse(Introduce.This(coachGen.Generate()));
    }

    [Fact]
    public void JustChecking()
    {
        1.Times(() =>
        {
            var script =
                from storage in "Storage".Stashed(() => new List<Coach>())
                from operations in "Operations".Choose(
                    from coachName in "Coach Name".Input(MGen.ChooseFrom(DataLists.FirstNames).Unique("coach name"))
                    from coach in "Coach".Derived(MGen.Constant(new Coach(coachName)))
                    from createCoach in "Create Coach".Act(() => storage.Add(coach))
                    select Acid.Test,
                    from name in "Coach AddSkill Name".Input(MGen.ChooseFromWithDefaultWhenEmpty(storage.Select(a => a.Name)))
                    from coachToAddSkillTo in "Coach AddSkill".Derived(MGen.Constant(storage.FirstOrDefault(a => a.Name == name)))
                    from _ in AddSkillToCoach(coachToAddSkillTo)
                    select Acid.Test)
                select Acid.Test;
            var ex = Assert.Throws<FalsifiableException>(() =>
                QState.Run(script, 1470894469)
                    .Options(a => a with { ReportTo = "JustChecking", Verbose = true })
                    .With(50.Runs())
                    .And(20.ExecutionsPerRun()));
            var report = ex.QAcidReport;
            Assert.Equal(3, report.OfType<ReportExecutionEntry>().Count());
        });
    }

    [Fact]
    public void Simplified()
    {
        QAcidScript<Acid> script =
            from coach in "Storage".Stashed(() => new Coach("Jimmy"))
            from _ in AddSkillToCoach(coach)
            select Acid.Test;

        var ex = Assert.Throws<FalsifiableException>(() =>
            QState.Run(script, 1071386449) // this one starts with 5 executions
                .Options(a => a with { ReportTo = "Simplified" })
                .With(50.Runs())
                .And(20.ExecutionsPerRun()));

        var report = ex.QAcidReport;
        Assert.Equal(2, report.OfType<ReportExecutionEntry>().Count());
    }

    private QAcidScript<Acid> AddSkillToCoach(Coach coach) =>
            from _ in Acid.Script
            let skills = (string[])["DotNet", "Javascript", "Agile", "HTML", "Css", "Entity Framework", "Web API"]
            from coachSkill in "Coach Skill".Input(MGen.ChooseFrom(skills))
            from addSkillToCoach in "Add Skill To Coach".ActIf(() => coach != null, () => coach.AddSkill(coachSkill))
            select Acid.Test;
}

// var script =
//             from storage in "Storage".Stashed(() => new List<Coach>())
//             let coachGen =
//                     from firstName in MGen.ChooseFrom(DataLists.FirstNames).Unique("coach name")
//                     from email in MGen.Constant($"{firstName}@CoursesGalore.com")
//                     from _ in MGen.For<Coach>().Construct(() => new Coach(firstName, email))
//                     from coach in MGen.One<Coach>()
//                     select coach
//             from operations in "Operations".Choose(
//             from coachInput in "CoachInput".Input(coachGen)
//             from createCoach in "Create Coach".Act(() => storage.Add(coachInput))
//             from traceCoach in "Coach".Trace(() => Introduce.This(coachInput, false))
//             select Acid.Test,
//                 from coachSkill in "Coach Skill".Input(MGen.ChooseFrom(skills))
//                 from coachToAddSkillTo in "Coach To Add Skill To"
//                     .Input(MGen.ChooseFromWithDefaultWhenEmpty(storage.Select(a => a.Name)))
//                 from addSkillToCoach in "Add Skill To Coach".ActIf(() => !string.IsNullOrWhiteSpace(coachToAddSkillTo),
//                     () => storage.Single(a => a.Name == coachToAddSkillTo).AddSkill(coachSkill))
//                 from tr in "tr".Trace(() => Introduce.This(storage))
//                 select Acid.Test
//             )
//             select Acid.Test;