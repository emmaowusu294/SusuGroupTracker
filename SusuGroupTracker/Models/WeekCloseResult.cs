
namespace SusuGroupTracker.Models;

public class WeekCloseResult
{
    // Indicates whether the week was closed successfully
    public bool IsClosed { get; }

    // Explains the result of the close attempt
    public string Message { get; }

    // Stores the outstanding balances when a week cannot close
    public IReadOnlyDictionary<string, decimal> OutstandingBalances { get; }

    public WeekCloseResult(
        bool isClosed,
        string message,
        IReadOnlyDictionary<string, decimal> outstandingBalances)
    {
        IsClosed = isClosed;
        Message = message;
        OutstandingBalances = outstandingBalances;
    }
}
