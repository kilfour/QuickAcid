using HorsesForCourses.Core;
using QuickMGenerate;
using QuickAcid;
using QuickExplainIt.TimeyWimey;
using QuickMGenerate.Data;
using System.Diagnostics;
using QuickAcid.Bolts.ShrinkStrats;
using QuickPulse.Bolts;
using QuickPulse;


namespace HorsesForCourses.Tests;

public class InMemoryStorage
{
    public List<Coach> Coaches { get; } = [];
    public Coach AddCoach(Coach coach) { Coaches.Add(coach); return coach; }

}

public class AcidTests
{
    [Fact(Skip = "mofo of a test, this one")]
    public void JustChecking()
    {
        List<string> skills = ["DotNet", "Javascript", "Agile", "HTML", "Css", "Entity Framework", "Web API"];
        var script =
            from storage in "Storage".Stashed(() => new InMemoryStorage())
            from coachName in "Coach Name".Input(MGen.ChooseFrom(DataLists.FirstNames).Unique("coach name"))
            from coachEmail in "Coach Email".Input(MGen.String())
            from coachSkill in "Coach Skill".Input(MGen.ChooseFrom(skills))

            from operations in "Operations".Choose(
                from createCoach in "Create Coach".Act(() => storage.AddCoach(new Coach(coachName, coachEmail)))
                select Acid.Test,
                from coachToAddSkillTo in "Coach To Add Skill To".Derived(MGen.ChooseFromWithDefaultWhenEmpty(storage.Coaches.Select(a => a.Name)))
                from addSkillToCoach in "Add Skill To Coach".ActIf(() => !string.IsNullOrWhiteSpace(coachToAddSkillTo),
                    () => storage.Coaches.Single(a => a.Name == coachToAddSkillTo).AddSkill(coachSkill))
                select Acid.Test
            )
            select Acid.Test;
        QState.Run(script, 2100155760)
            .Options(a => a with { Verbose = true })
            .Options(a => a with { AddShrinkInfoToReport = true, ReportTo = "temp" })
            .WithOneRun()
            .And(20.ExecutionsPerRun());
        // QState.Run(script, 1364686977)
        //     .Options(a => a with { AddShrinkInfoToReport = true, Verbose = true, ReportTo = "temp" })
        //     .WithOneRun()
        //     .And(3.ExecutionsPerRun());
        // QState.Run("just-checking", script)
        //     .Options(a => a with { AddShrinkInfoToReport = true })
        //     .With(50.Runs())
        //     .And(20.ExecutionsPerRun());
    }
}