# Design Notes

## Assumptions and Decisions

- **Member identity:** Each member is identified by their name. Names must be unique, ignoring letter case and leading or trailing whitespace.
- **Fixed contribution:** Every member must contribute the same fixed amount each week, specified when the group is created.
- **Partial payments:** Members may pay in multiple installments, provided each payment is positive and does not exceed their remaining balance.
- **Week closure:** A week can close only when every registered member has paid their full contribution. If anyone owes money, the week remains open and no payout is made.
- **Payout order:** Members receive payouts in the order they joined the group. The order rotates after each successful weekly closure.
- **Payout amount:** The weekly pot is calculated as the fixed weekly contribution multiplied by the number of registered members.
- **Membership changes:** Members cannot be added after contributions have started or a payout has been recorded.
- **Money representation:** The application uses C# `decimal` for monetary calculations to avoid binary floating-point errors.
- **Reporting:** Reports show each member's payments and outstanding balance for the current week, alongside the payout history.

## Limitations

- The application runs in the console and does not have a graphical interface.
- Group data is stored in memory and is lost when the application exits.
- Members cannot be removed or added after contributions have started.
- Member names are used as identifiers; the application does not assign separate member IDs.
- The application does not integrate with mobile money or bank payment services. Contributions are recorded manually.
- The application supports one group per running session.

## Possible Improvements

- Persist group, contribution, and payout data in a database.
- Introduce unique member IDs.
- Add support for managing multiple groups.
- Improve input validation and currency formatting across different terminal environments.
- Add integration tests and more edge-case unit tests.
