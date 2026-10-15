
using SusuGroupTracker.Interfaces;
using SusuGroupTracker.Services;

// Keep the service reference so the menu can use the business logic.
ISusuGroupService? service = null;

while (true)
{
    Console.Clear();
    Console.WriteLine("================================");
    Console.WriteLine("       SUSU GROUP TRACKER");
    Console.WriteLine("================================");
    Console.WriteLine("1. Create group");
    Console.WriteLine("2. Add member");
    Console.WriteLine("3. Record contribution");
    Console.WriteLine("4. Close current week");
    Console.WriteLine("5. View report");
    Console.WriteLine("0. Exit");
    Console.Write("\nSelect an option: ");

    string? option = Console.ReadLine();

    if (option == "0")
    {
        Console.WriteLine("Thank you for using Susu Group Tracker!");
        break;
    }

    try
    {
        switch (option)
        {
            case "1":
                Console.Write("Enter weekly contribution (GHS): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                {
                    Console.WriteLine("Invalid amount.");
                    break;
                }

                // Create a new group with a fixed weekly contribution.
                service = new SusuGroupService(amount);
                Console.WriteLine("Group created successfully.");
                break;

            case "2":
                if (service is null)
                {
                    Console.WriteLine("Create a group first.");
                    break;
                }

                Console.Write("Enter member name: ");
                string? name = Console.ReadLine();

                service.AddMember(name ?? "");
                Console.WriteLine("Member added successfully.");
                break;

            case "3":
                if (service is null)
                {
                    Console.WriteLine("Create a group first.");
                    break;
                }

                Console.Write("Enter member name: ");
                string? memberName = Console.ReadLine();

                Console.Write("Enter contribution amount (GHS): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal payment))
                {
                    Console.WriteLine("Invalid amount.");
                    break;
                }

                service.RecordContribution(memberName ?? "", payment);
                Console.WriteLine("Contribution recorded successfully.");
                break;

            case "4":
                if (service is null)
                {
                    Console.WriteLine("Create a group first.");
                    break;
                }

                // The service decides whether the week can close.
                var result = service.CloseWeek();
                Console.WriteLine(result.Message);

                // Display outstanding balances when members still owe money.
                foreach (var balance in result.OutstandingBalances)
                {
                    Console.WriteLine(
                        $"{balance.Key} owes GH₵{balance.Value:F2}");
                }

                break;



            case "5":
                if (service is null)
                {
                    Console.WriteLine("Create a group first.");
                    break;
                }

                Console.WriteLine($"\nCurrent week: {service.CurrentWeek}");
                Console.WriteLine(
                    $"Weekly contribution: GH₵{service.WeeklyContribution:F2}");

                Console.WriteLine("\nCurrent week payments:");

                foreach (var memberReport in service.GetCurrentWeekReport())
                {
                    string status = memberReport.HasPaidInFull
                        ? "COMPLETE"
                        : $"Owes GH₵{memberReport.AmountOwed:F2}";

                    Console.WriteLine(
                        $"{memberReport.MemberName} | " +
                        $"Paid: GH₵{memberReport.AmountPaid:F2} | {status}");
                }

                Console.WriteLine("\nPayout history:");

                var history = service.GetPayoutHistory();

                if (history.Count == 0)
                {
                    Console.WriteLine("No payouts recorded yet.");
                }
                else
                {
                    foreach (var payout in history)
                    {
                        Console.WriteLine(
                            $"Week {payout.WeekNumber}: " +
                            $"{payout.RecipientName} received GH₵{payout.Amount:F2}");
                    }
                }

                break;



            default:
                Console.WriteLine("Invalid option. Try again.");
                break;
        }
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Input error: {ex.Message}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"Operation rejected: {ex.Message}");
    }
    catch (KeyNotFoundException ex)
    {
        Console.WriteLine($"Member error: {ex.Message}");
    }

    Console.WriteLine("\nPress Enter to continue...");
    Console.ReadLine();
}

// Read the current week through the service implementation.
//static int GetCurrentWeek(ISusuGroupService service)
//{
//    return ((SusuGroupService)service).CurrentWeek;
//}
