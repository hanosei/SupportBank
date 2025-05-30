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
using System.Reflection;
using System.Text.Json;
using System.Reflection.Metadata.Ecma335;

class FileProcessor
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    public static CsvReader ReadCSV(string path)
    {
        string? methodName = MethodBase.GetCurrentMethod()?.Name;
        try
        {
            var reader = new StreamReader(path);
            var csv = new CsvReader(reader, CultureInfo.CurrentCulture);
            return csv;
        }
        catch (Exception e)
        {
            Logger.Error(methodName + " " + e);
        }
        return null!;
    }
    
    public static dynamic ReadJSON(string path)
    {
        string? methodName = MethodBase.GetCurrentMethod()?.Name;
        try
        {
            var jsonString = File.ReadAllText(path);
            List<dynamic>? data = JsonSerializer.Deserialize<List<dynamic>>(jsonString);
            // var data = JsonSerializer.Deserialize<dynamic>(jsonString);
            return data!;
        }
        catch (Exception e)
        {
            Logger.Error(methodName + " " + e);
        }
        return null!;
    }  
}