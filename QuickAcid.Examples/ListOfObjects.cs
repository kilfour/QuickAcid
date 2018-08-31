using System.Collections.Generic;
using System.Linq;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;
using Xunit;

namespace QuickAcid.Examples
{
    public class ListOfObjects
    {
        [Fact]
        public void Check()
        {
            TheRun.Verify(10, 20);
        }

        private static readonly QAcidRunner<Acid> TheRun =
            from container in "setup".OnceOnlyInput(() => new TheContainer())
            from addRemove in "add/remove"
                .Choose(
                    Add(container),
                    Remove(container))
            select Acid.Test;

        private static QAcidRunner<Acid> Add(TheContainer container)
        {
            return 
                from input in "add input".Input(() => MGen.One<TheObject>().Generate())
                from add in "add".Act(() => container.Add(input))
                from added in "added".Spec((() => container.List.Contains(input)))
                select Acid.Test;
        }

        private static QAcidRunner<Acid> Remove(TheContainer container)
        {
            return (
                from index in "remove index".Input(MGen.Int(0, container.List.Count - 1))
                from input in "remove get at index".Input(() => container.List[index])
                from remove in "remove".Act(() => container.Remove(input))
                from removed in "removed".Spec(() => !container.List.Contains(input))
                select Acid.Test)
                .If(() => container.List.Any());
        }

        public class TheContainer
        {
            public List<TheObject> List { get; set; }

            public TheContainer()
            {
                List = new List<TheObject>();
            }

            public void Add(TheObject obj)
            {
                List.Add(obj);
            }

            public void Remove(TheObject obj)
            {
                var ix = List.IndexOf(obj);
                if (ix == 0)
                    return;
                List.Remove(obj);
            }
        }

        public class TheObject
        {
            public int AnInt { get; set; }

            public override string ToString()
            {
                return AnInt.ToString();
            }
        }
    }
}