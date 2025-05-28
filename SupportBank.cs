using System;
using System.Globalization;
using CsvHelper;
using Microsoft.VisualBasic;
using static System.IO.StreamReader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SupportBank
{
    class SupportBank
    {
        //private Dictionary<string, Account> _accounts = new Dictionary<string, Account>();
        public List<Transaction> transactions = new List<Transaction>();

        public void ReadCSV(string path)
        {

            using var reader = new StreamReader(path);
            using var csv = new CsvReader(reader, CultureInfo.CurrentCulture);

            var records = new List<Transaction>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = new Transaction
                {
                    Date = csv.GetField<DateTime>("Date"),
                    From = csv.GetField("From"),
                    To = csv.GetField("To"),
                    Narrative = csv.GetField("Narrative"),
                    Amount = csv.GetField<float>("Amount"),
                };
                records.Add(record);
            }

             foreach (var record in records)
             {

                 Console.WriteLine($"{record.Date} - {record.From} - {record.To}- {record.Narrative}- {record.Amount}");
             }
        }

        static void Main(string[] args)
        {
            var bank = new SupportBank();
            bank.ReadCSV("./Transactions2014.csv");
        }
    }
}