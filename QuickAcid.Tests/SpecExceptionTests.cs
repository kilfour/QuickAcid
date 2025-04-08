using QuickAcid.Nuts;
using QuickAcid.Nuts.Bolts;

namespace QuickAcid.Tests
{
    public class SpecExceptionTests
    {
        [Fact]
        public void ExceptionThrownBySpecIsNotAQuickAcidException()
        {
            var ex =
                Assert.Throws<Exception>(() => (
                    from spec in "spec".Spec(Throw)
                    select Acid.Test
                ).Verify(1, 1));
            Assert.IsNotType<FalsifiableException>(ex);
            Assert.Contains("QuickAcid.Tests.SpecExceptionTests.Throw()", ex.StackTrace);
        }

        private bool Throw()
        {
            throw new Exception();
        }
    }
}