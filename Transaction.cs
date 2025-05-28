using System;

public class Transaction
{
    public DateTime Date { get; set; }
    public string? Narrative { get; set; }
    public string? From { get; set; }
    public string? To { get; set;}
    public float Amount { get; set;}

    internal object getFrom()
    {
        throw new NotImplementedException();
    }
}