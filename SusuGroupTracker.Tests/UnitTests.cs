
using SusuGroupTracker.Services;

namespace SusuGroupTracker.Tests;

public class SusuGroupServiceTests
{
    [Fact]
    public void Constructor_RejectsZeroWeeklyContribution()
    {
        // A susu group must have a positive weekly contribution
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new SusuGroupService(0m));
    }

    [Fact]
    public void AddMember_RejectsDuplicateNamesIgnoringCase()
    {
        // Member names must be unique regardless of letter case
        var service = new SusuGroupService(50m);

        service.AddMember("Ama");

        Assert.Throws<InvalidOperationException>(
            () => service.AddMember("ama"));
    }

    [Fact]
    public void RecordContribution_RejectsUnknownMember()
    {
        // Only registered members can contribute
        var service = new SusuGroupService(50m);
        service.AddMember("Ama");

        Assert.Throws<KeyNotFoundException>(
            () => service.RecordContribution("Kofi", 50m));
    }

    [Fact]
    public void RecordContribution_RejectsNegativeAmount()
    {
        // Negative payments must never be accepted
        var service = new SusuGroupService(50m);
        service.AddMember("Ama");

        Assert.Throws<ArgumentOutOfRangeException>(
            () => service.RecordContribution("Ama", -10m));
    }

    [Fact]
    public void CloseWeek_DoesNotPayOutWhenMemberOwesMoney()
    {
        // A week cannot close while any member has an outstanding balance
        var service = new SusuGroupService(50m);
        service.AddMember("Ama");
        service.AddMember("Kofi");

        service.RecordContribution("Ama", 50m);
        service.RecordContribution("Kofi", 30m);

        var result = service.CloseWeek();

        Assert.False(result.IsClosed);
        Assert.Equal(20m, result.OutstandingBalances["Kofi"]);
        Assert.Empty(service.GetPayoutHistory());
    }

    [Fact]
    public void RecordContribution_AllowsPartialPayments()
    {
        // Multiple payments can complete one weekly contribution
        var service = new SusuGroupService(50m);
        service.AddMember("Ama");

        service.RecordContribution("Ama", 30m);
        service.RecordContribution("Ama", 20m);

        var result = service.CloseWeek();

        Assert.True(result.IsClosed);
        Assert.Single(service.GetPayoutHistory());
        Assert.Equal(50m, service.GetPayoutHistory()[0].Amount);
    }

    [Fact]
    public void RecordContribution_RejectsOverpayment()
    {
        // A member cannot pay more than their outstanding balance
        var service = new SusuGroupService(50m);
        service.AddMember("Ama");

        service.RecordContribution("Ama", 30m);

        Assert.Throws<InvalidOperationException>(
            () => service.RecordContribution("Ama", 21m));
    }

    [Fact]
    public void CloseWeek_PaysMembersInJoiningOrder()
    {
        // Payouts rotate according to the order members joined
        var service = new SusuGroupService(50m);
        service.AddMember("Ama");
        service.AddMember("Kofi");

        service.RecordContribution("Ama", 50m);
        service.RecordContribution("Kofi", 50m);

        service.CloseWeek();

        service.RecordContribution("Ama", 50m);
        service.RecordContribution("Kofi", 50m);

        service.CloseWeek();

        var history = service.GetPayoutHistory();

        Assert.Equal("Ama", history[0].RecipientName);
        Assert.Equal("Kofi", history[1].RecipientName);
    }



    [Fact]
    public void GetCurrentWeekReport_ShowsPaidAndOutstandingAmounts()
    {
        // The report should show each member's current payment status
        var service = new SusuGroupService(50m);
        service.AddMember("Ama");
        service.AddMember("Kofi");

        service.RecordContribution("Ama", 40m);
        service.RecordContribution("Kofi", 50m);

        var report = service.GetCurrentWeekReport();

        Assert.Equal(2, report.Count);

        Assert.Equal("Ama", report[0].MemberName);
        Assert.Equal(40m, report[0].AmountPaid);
        Assert.Equal(10m, report[0].AmountOwed);
        Assert.False(report[0].HasPaidInFull);

        Assert.Equal("Kofi", report[1].MemberName);
        Assert.Equal(50m, report[1].AmountPaid);
        Assert.Equal(0m, report[1].AmountOwed);
        Assert.True(report[1].HasPaidInFull);
    }

    [Fact]
    public void GetCurrentWeekReport_ShowsZeroPaymentsForNewWeek()
    {
        // A successful payout should start a fresh payment report for the next week
        var service = new SusuGroupService(50m);
        service.AddMember("Ama");
        service.AddMember("Kofi");

        service.RecordContribution("Ama", 50m);
        service.RecordContribution("Kofi", 50m);

        var closeResult = service.CloseWeek();

        Assert.True(closeResult.IsClosed);
        Assert.Equal(2, service.CurrentWeek);

        var report = service.GetCurrentWeekReport();

        Assert.Equal(2, report.Count);

        foreach (var memberReport in report)
        {
            Assert.Equal(0m, memberReport.AmountPaid);
            Assert.Equal(50m, memberReport.AmountOwed);
            Assert.False(memberReport.HasPaidInFull);
        }
    }

    [Fact]
    public void CloseWeek_WhenPaymentIsOutstanding_DoesNotAdvanceWeek()
    {
        // An unsuccessful week closure must not advance the current week
        var service = new SusuGroupService(50m);
        service.AddMember("Ama");
        service.AddMember("Kofi");

        service.RecordContribution("Ama", 50m);
        service.RecordContribution("Kofi", 30m);

        var result = service.CloseWeek();

        Assert.False(result.IsClosed);
        Assert.Equal(1, service.CurrentWeek);
        Assert.Empty(service.GetPayoutHistory());
        Assert.Equal(20m, result.OutstandingBalances["Kofi"]);
    }

}
