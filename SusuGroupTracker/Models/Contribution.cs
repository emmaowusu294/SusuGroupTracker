
namespace SusuGroupTracker.Models;

public class Contribution
{
    // Identifies the member making the contribution
    public string MemberName { get; }

    // Identifies the week this contribution belongs to
    public int WeekNumber { get; }

    // Tracks the total amount the member has paid for this week
    public decimal AmountPaid { get; private set; }

    public Contribution(string memberName, int weekNumber)
    {
        MemberName = memberName;
        WeekNumber = weekNumber;
        AmountPaid = 0m;
    }

    // Adds a payment to the member's existing contributions
    public void AddPayment(decimal amount)
    {
        AmountPaid += amount;
    }
}
