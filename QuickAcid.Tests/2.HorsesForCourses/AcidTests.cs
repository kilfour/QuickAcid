using HorsesForCourses.Core;
using QuickFuzzr;
using QuickAcid;
using QuickFuzzr.Data;
using QuickPulse;
using QuickAcid.Reporting;


namespace HorsesForCourses.Tests;

public class AcidTests
{
    [Fact]
    public void JustChecking()
    {
        var ex = Assert.Throws<FalsifiableException>(() =>
            QState.Run(
                from storage in "Storage".Stashed(() => new List<Coach>())
                from operations in "Operations".Choose(CreateCoach(storage), AddSkillToCoach(storage))
                select Acid.Test, 1470894469)
            // .Options(a => a with { ReportTo = "HfC" })
            // .Options(a => a with { Verbose = true })
            .With(1.Runs())
            .And(20.ExecutionsPerRun()));
        var report = ex.QAcidReport;
        Assert.Equal(3, report.OfType<ReportExecutionEntry>().Count());
    }

    private static QAcidScript<Acid> CreateCoach(List<Coach> storage) =>
        from name in "CC Name".Input(MGen.ChooseFrom(DataLists.FirstNames).Unique("coach name"))
        from email in "CC Email".Input(MGen.Constant($"{name}@CoursesGalore.com"))
        from coach in "CC Coach".Derived(MGen.Constant(new Coach(name, email)))
        from _ in "Create Coach".Act(() => storage.Add(coach))
        select Acid.Test;

    private static QAcidScript<Acid> AddSkillToCoach(List<Coach> storage) =>
        from name in "AStC Name".Input(MGen.ChooseFromWithDefaultWhenEmpty(storage.Select(a => a.Name)))
        from coach in "AStC Coach".Derived(MGen.Constant(storage.FirstOrDefault(a => a.Name == name)))
        let skills = (string[])["DotNet", "Javascript", "Agile", "HTML", "Css", "Entity Framework", "Web API"]
        from skill in "AStC Skill".Input(MGen.ChooseFrom(skills))
        from _ in "Add Skill to Coach".ActIf(() => coach != null, () => coach.AddSkill(skill))
        select Acid.Test;
}