namespace QuickAcid.Tests.Linqy.Spec
{
    public class SpecExceptionTests
    {
        [Fact]
        public void ExceptionThrownBySpecIsNotAQuickAcidException()
        {
            var ex =
                Assert.Throws<Exception>(() =>
                    new QState(
                        from spec in "spec".Spec(Throw) select Acid.Test
                    ).TestifyOnce()
                );
            Assert.IsNotType<FalsifiableException>(ex);
            Assert.Contains("QuickAcid.Tests.Linqy.Spec.SpecExceptionTests.Throw()", ex.StackTrace);
        }

        private bool Throw()
        {
            throw new Exception();
        }
    }
}