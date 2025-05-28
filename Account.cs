using System;

public class Account
{
    private string? Name { get; set; }
    private float Owed { get; set; }
    private float IsOwed { get; set; }
    private float TotalBalance { get; set;}
    private List<Transaction>? Transactions { get; set; } 
}