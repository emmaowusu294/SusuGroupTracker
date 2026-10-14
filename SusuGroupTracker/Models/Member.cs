
namespace SusuGroupTracker.Models;

public class Member
{
    // Identifies the member within the group
    public string Name { get; }

    // Determines the member's payout position
    public int PayoutOrder { get; }

    public Member(string name, int payoutOrder)
    {
        Name = name;
        PayoutOrder = payoutOrder;
    }
}
