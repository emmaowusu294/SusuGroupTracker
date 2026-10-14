
using SusuGroupTracker.Models;

namespace SusuGroupTracker.Interfaces;

public interface ISusuGroupService
{
    decimal WeeklyContribution { get; }
    int CurrentWeek { get; }

    // Adds a member to the group
    void AddMember(string name);

    // Records a contribution for a member in the current week
    void RecordContribution(string memberName, decimal amount);

    // Closes the current week and reports any outstanding payments
    WeekCloseResult CloseWeek();

    // Returns the members in the group
    IReadOnlyList<Member> GetMembers();

    // Returns the payout history
    IReadOnlyList<PayoutRecord> GetPayoutHistory();

    // Returns payment details for every member in the current week
    IReadOnlyList<MemberPaymentReport> GetCurrentWeekReport();
}
