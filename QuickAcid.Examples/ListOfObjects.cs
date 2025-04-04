using QuickMGenerate;

namespace QuickAcid.Examples
{
    public class ListOfObjects
    {
        [Fact(Skip = "Explicit")]
        public void Check()
        {
            TheTest.Verify(10.Runs(), 100.ActionsPerRun());
        }

        private static readonly QAcidRunner<Acid> TheTest =
            from container in "setup".TrackedInput(() => new TheContainer())
            from addRemove in "add/remove"
                .Choose(
                    Add(container),
                    Remove(container))
            select Acid.Test;

        private static QAcidRunner<Acid> Add(TheContainer container)
        {
            return
                from input in "add input".Input(() => MGen.One<TheObject>().Generate())
                from add in "TheContainer.Add".Act(() => container.Add(input))
                from added in "added".Spec(() => container.List.Contains(input))
                select Acid.Test;
        }

        private static QAcidRunner<Acid> Remove(TheContainer container)
        {
            return (
                from input in "remove input".Input(
                    () => container.List[MGen.Int(0, container.List.Count - 1).Generate()])
                from remove in "TheContainer.Remove".Act(() => container.Remove(input))
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
                if (ix == 3)
                    return;
                List.Remove(obj);
            }
        }

        public class TheObject
        {
            private readonly Guid id = Guid.NewGuid();
            public int AnInt { get; set; }

            public override string ToString()
            {
                return $"Id : {id}, Int : {AnInt}";
            }
        }
    }
}