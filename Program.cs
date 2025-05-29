using System;
using System.Globalization;
using System.Threading.Tasks.Dataflow;
using CsvHelper;
using Microsoft.VisualBasic;
using static System.IO.StreamReader;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Fluent;

namespace SupportBank;

public class Program
{   
    static void Main(string[] args)
    {

        var config = new LoggingConfiguration();
        string startupPath = System.IO.Directory.GetCurrentDirectory();
        Console.WriteLine($"{startupPath}/SupportBank.log");
        var target = new FileTarget
        { FileName = $"{startupPath}/SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
        config.AddTarget("File Logger", target);
        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
        LogManager.Configuration = config;

        var bank = new SupportBank();
        bank.ReadCSV("./DodgyTransactions2015.csv");
        bank.CreateAccounts();
        bool validUserOption = false;
        while (!validUserOption)
        {
            int userOption = UserInput.getUserOptions();
            if (userOption == 1 || userOption == 2)
            {
                validUserOption = true;
                if (userOption == 1)
                {
                    bank.PrintAllAccounts();
                }
                else
                {
                    bank.PrintAnAccount();
                }
            }

        }

    }

}
