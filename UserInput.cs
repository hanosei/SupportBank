using System;

namespace SupportBank;

public class UserInput
{

  public static string? getUserInput()

        {
            Console.WriteLine("Enter a name: ");
            string? userInput = Console.ReadLine();
            return userInput;

        }
        public static int getUserOptions()
        {

            Console.WriteLine("Select from the following options \n1. Get list of all accounts \n2. Get accounts by name\nEnter 1 or 2");
            int option;
            int.TryParse(Console.ReadLine(), out option);
            return option;
        }
}
