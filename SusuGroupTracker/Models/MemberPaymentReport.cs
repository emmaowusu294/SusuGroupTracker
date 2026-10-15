
namespace SusuGroupTracker.Models;

public class MemberPaymentReport
{
    // Identifies the member
    public string MemberName { get; }

    // Tracks how much the member has paid this week
    public decimal AmountPaid { get; }

    // Tracks how much the member still owes
    public decimal AmountOwed { get; }

    // Indicates whether the member has paid in full
    public bool HasPaidInFull { get; }

    public MemberPaymentReport(
        string memberName,
        decimal amountPaid,
        decimal amountOwed)
    {
        MemberName = memberName;
        AmountPaid = amountPaid;
        AmountOwed = amountOwed;
        HasPaidInFull = amountOwed == 0m;
    }
}
