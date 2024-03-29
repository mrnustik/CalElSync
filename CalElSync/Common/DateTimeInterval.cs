﻿namespace CalElSync.Core.Common;

public record DateTimeInterval
{
    public DateTimeInterval(
        DateTime Start,
        DateTime End)
    {
        if (End < Start)
        {
            throw new ArgumentException("End of the interval has to be after its Start", nameof(End));
        }
        
        this.Start = Start;
        this.End = End;
    }

    public DateTime Start { get; }
    public DateTime End { get; }
}