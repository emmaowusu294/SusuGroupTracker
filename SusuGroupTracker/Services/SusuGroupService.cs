
using SusuGroupTracker.Interfaces;
using SusuGroupTracker.Models;

namespace SusuGroupTracker.Services;

public class SusuGroupService : ISusuGroupService
{
    // Stores members in the order they joined
    private readonly List<Member> _members = new();

    // Stores every contribution made during the group's lifetime
    private readonly List<Contribution> _contributions = new();

    // Stores successful payouts
    private readonly List<PayoutRecord> _payoutHistory = new();

    // The contribution amount is fixed for every member and week
    public decimal WeeklyContribution { get; }

    // The group starts at week 1
    public int CurrentWeek { get; private set; } = 1;

    public SusuGroupService(decimal weeklyContribution)
    {
        // A group must have a positive weekly contribution
        if (weeklyContribution <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(weeklyContribution),
                "Weekly contribution must be greater than zero.");
        }

        WeeklyContribution = weeklyContribution;
    }

    public void AddMember(string name)
    {
        // Reject missing or whitespace-only names
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Member name cannot be empty.",
                nameof(name));
        }

        // Members cannot join after contributions have started
        if (_contributions.Count > 0 || _payoutHistory.Count > 0)
        {
            throw new InvalidOperationException(
                "Members cannot be added after contributions have started.");
        }

        // Member names must be unique, ignoring letter case
        bool alreadyExists = _members.Any(member =>
            string.Equals(
                member.Name,
                name.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                "A member with this name already exists.");
        }

        // The next member receives the next payout position
        int payoutOrder = _members.Count + 1;

        _members.Add(new Member(name.Trim(), payoutOrder));
    }

    public void RecordContribution(string memberName, decimal amount)
    {
        // Payments must be positive
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Contribution amount must be greater than zero.");
        }

        // Find the member without treating uppercase and lowercase as different
        Member? member = _members.FirstOrDefault(member =>
            string.Equals(
                member.Name,
                memberName?.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (member is null)
        {
            throw new KeyNotFoundException("Member was not found.");
        }

        // Find this member's contribution for the current week
        Contribution? contribution = _contributions.FirstOrDefault(item =>
            item.WeekNumber == CurrentWeek &&
            string.Equals(
                item.MemberName,
                member.Name,
                StringComparison.OrdinalIgnoreCase));

        decimal amountPaid = contribution?.AmountPaid ?? 0m;
        decimal amountOwed = WeeklyContribution - amountPaid;

        // Reject payments that exceed the remaining balance
        if (amount > amountOwed)
        {
            throw new InvalidOperationException(
                $"Payment exceeds the outstanding balance of {amountOwed:F2} GHS.");
        }

        // Create a contribution record when this is the member's first payment
        if (contribution is null)
        {
            contribution = new Contribution(member.Name, CurrentWeek);
            _contributions.Add(contribution);
        }

        contribution.AddPayment(amount);
    }

    public WeekCloseResult CloseWeek()
    {
        // A payout cannot be made without group members
        if (_members.Count == 0)
        {
            return new WeekCloseResult(
                false,
                "Cannot close a week without members.",
                new Dictionary<string, decimal>());
        }

        var outstandingBalances = new Dictionary<string, decimal>(
            StringComparer.OrdinalIgnoreCase);

        // Check every member's outstanding balance for the current week
        foreach (Member member in _members)
        {
            Contribution? contribution = _contributions.FirstOrDefault(item =>
                item.WeekNumber == CurrentWeek &&
                string.Equals(
                    item.MemberName,
                    member.Name,
                    StringComparison.OrdinalIgnoreCase));

            decimal amountPaid = contribution?.AmountPaid ?? 0m;
            decimal amountOwed = WeeklyContribution - amountPaid;

            if (amountOwed > 0)
            {
                outstandingBalances[member.Name] = amountOwed;
            }
        }

        // Do not pay anyone until every member has paid in full
        if (outstandingBalances.Count > 0)
        {
            return new WeekCloseResult(
                false,
                "Week cannot close because some members still owe money.",
                outstandingBalances);
        }

        // Rotate payouts according to the members' joining order
        int recipientIndex = _payoutHistory.Count % _members.Count;
        Member recipient = _members[recipientIndex];

        // The pot contains one full contribution from every member
        decimal potAmount = WeeklyContribution * _members.Count;

        _payoutHistory.Add(
            new PayoutRecord(CurrentWeek, recipient.Name, potAmount));

        // Advance only after a successful payout
        CurrentWeek++;

        return new WeekCloseResult(
            true,
            $"{recipient.Name} received {potAmount:F2} GHS.",
            new Dictionary<string, decimal>());
    }



    public IReadOnlyList<MemberPaymentReport> GetCurrentWeekReport()
    {
        var report = new List<MemberPaymentReport>();

        // Build a payment report for every registered member
        foreach (Member member in _members)
        {
            Contribution? contribution = _contributions.FirstOrDefault(item =>
                item.WeekNumber == CurrentWeek &&
                string.Equals(
                    item.MemberName,
                    member.Name,
                    StringComparison.OrdinalIgnoreCase));

            decimal amountPaid = contribution?.AmountPaid ?? 0m;
            decimal amountOwed = WeeklyContribution - amountPaid;

            report.Add(
                new MemberPaymentReport(
                    member.Name,
                    amountPaid,
                    amountOwed));
        }

        return report.AsReadOnly();
    }


    public IReadOnlyList<Member> GetMembers()
    {
        // Return a copy so callers cannot modify the internal member list
        return _members.ToList().AsReadOnly();
    }

    public IReadOnlyList<PayoutRecord> GetPayoutHistory()
    {
        // Return a copy so callers cannot modify the internal payout history
        return _payoutHistory.ToList().AsReadOnly();
    }
}
