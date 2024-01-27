namespace CalElSync.Core.Events;

public record Event(
    string Title,
    DateTime StartTime,
    DateTime EndTime);