using System;

public class Account
{
    public string? Name { get; set; }
    public float Owed { get; set; }
    public float IsOwed { get; set; }
    public float TotalBalance { get; set;}
    public List<Transaction>? AccountTransactions { get; set; } 

    public Account(string name){
        this.Name = name;
        this.Owed = 0;
        this.IsOwed = 0;
        this.TotalBalance = 0;
        this.AccountTransactions = new List<Transaction>();
    }
    
   

    public void UpdateTransaction(Transaction record)
    {
       AccountTransactions?.Add(record);         

    }

    public void updateIsOwed(float amount){
        IsOwed += amount;
        TotalBalance += amount;
    }

    public void updateOwed(float amount){
        Owed += amount;
        TotalBalance -= amount;
    }
}