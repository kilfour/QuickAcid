using QuickPulse.Explains;

namespace QuickAcid.Tests._Tools.Models;

[CodeExample]
public class Account
{
    public int Balance = 0;
    public void Deposit(int amount) { Balance += amount; }
    public void Withdraw(int amount) { Balance -= amount; }
}