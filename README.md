# Susu Group Tracker

A C# console application for managing a rotating savings group (susu). The application tracks members, weekly contributions, outstanding balances, and payouts.

## Features

- Create a group with a fixed weekly contribution in GHS.
- Add members with unique names.
- Record full or partial contributions.
- Reject invalid payments and payments above the outstanding balance.
- Prevent a week from closing while members still owe contributions.
- Pay the weekly pot to members in joining order.
- View current-week payment statuses and outstanding balances.
- View payout history.
- Automated unit tests for core business rules.

## Requirements

- .NET 10 SDK

## Run the application

From the solution root, run:

```powershell
dotnet run --project SusuGroupTracker
```

Follow the menu to create a group, add members, record contributions, close weeks, and view reports.

## Run the tests

From the solution root, run:

```powershell
dotnet test
```

The test project is `SusuGroupTracker.Tests`.

## How payouts work

Each member is expected to contribute the group's fixed weekly amount. The week can close only when all registered members have paid in full.

The payout equals the weekly contribution multiplied by the number of members. Payouts rotate according to the order in which members joined the group.

## Technology

- C#
- .NET 10
- xUnit
