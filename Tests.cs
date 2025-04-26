namespace Refined.By.QuickAcid;

public class UnitTests
{
[Fact]
public void No_Overdraft()
{
    var account = new Account();
    account.Withdraw(61);
    Assert.True(account.Balance >= 0);
}

[Fact]
public void Balance_Has_Maximum()
{
    var account = new Account();
    account.Deposit(89);
    account.Deposit(89);
    Assert.True(account.Balance <= 100);
}


}
