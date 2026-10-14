
namespace SusuGroupTracker.Models;

public class PayoutRecord
{
    // Identifies the week the payout belongs to
    public int WeekNumber { get; }

    // Identifies the member who received the pot
    public string RecipientName { get; }

    // Stores the total amount paid out
    public decimal Amount { get; }

    public PayoutRecord(int weekNumber, string recipientName, decimal amount)
    {
        WeekNumber = weekNumber;
        RecipientName = recipientName;
        Amount = amount;
    }
}
