using System;
using System.Diagnostics;
using System.IO;  //file access
using System.Speech.Synthesis;  //text to speech

namespace Console_Budget_Report
{
    internal class Program
    {
        #region VARIABLES
        public static string BudgetPath = Environment.GetEnvironmentVariable("onedriveconsumer") + "\\documents\\Console Budget Report\\";  //path to save and read from
        public static string IncomeList = BudgetPath + "income.csv";  //file path for income list
        public static string ExpenseList = BudgetPath + "expenses.csv";
        public static string BudgetReport = BudgetPath + "Monthly Budget.txt"; //budget report file path
        public static double IncomeTotal, ExpenseTotal, LeftOver;  //income and expense totals

        public struct Transaction //struct for storing income, expense, and percentage  amounts
        {
            public string Name;
            public double Amount;
            public Double Percentage;
        }

        public static Transaction[] Income = new Transaction[20];  //income array
        public static Transaction[] Expense = new Transaction[20];
        #endregion

        static void Main()
        {
            Console.Title = "Console Budget Report by Charles Martin";  //console window title
            Console.ForegroundColor = ConsoleColor.White;  //text color for console

            Directory.CreateDirectory(BudgetPath);  //make sure directory exists
            Print("Console Budget Report by Charles Martin");
            Print("This program will help you create a budget,");
            Print("All that's needed is to type in your");
            Print("income and expense names and amounts.");
            LoadFiles();  //open data files
            MainMenu();   //choose what to do
        }

        static void MainMenu(bool PrintOptions = true)  //menu of options for the user
        {
            if (PrintOptions)  //for certain cases, menu can be rerun without the options printed
            {
                Print("\n\n-----MAIN MENU");
                Print("     D - Display Budget");
                Print("     I - Edit Income List");
                Print("     E - Edit Expense List");
                Print("     X - Exit");
            }

            char KP = WaitForKeyPress("\n \nWhat would you like to do?  ");  //get a single key, converts all to lowercase

            switch (KP)  //decide what to do based on key hit
            {
                case 'i':  //income list
                    EditFile(IncomeList);
                    break;
                case 'e':  //exense list
                    EditFile(ExpenseList);
                    break;
                case 'd':
                    DisplayBudget();
                    break;
                case 'x':
                    Environment.Exit(0);
                    break;
                default:
                    MainMenu(false);  //not a valid key, rerun
                    break;
            }
        }

        static void SaveReport()  //write budget to file
        {
            LoadFiles();  //make sure data arrays and totals are up to date
            GetTotals();
            StreamWriter BW = new StreamWriter(BudgetReport);  //construct report
            BW.WriteLine("INCOME SOURCES▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓");

            #region Income
            for (int i = 0; i < Income.Length; i++)  //loop through income array
            {
                if (Income[i].Amount > 0)  //if amount not zero, write to file in 2 columns 
                {
                    BW.WriteLine("    {0,-20} {1,12} {2,12}", Income[i].Name, "$" + Income[i].Amount.ToString("n2"), Income[i].Percentage.ToString("n2") + "%");
                }
            }
            BW.WriteLine("    {0,-20} {1,12}", "INCOME TOTAL", "$" + IncomeTotal.ToString("n2"));  //income total
            BW.WriteLine();
            #endregion

            #region Expenses
            BW.WriteLine("EXPENSES▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓");
             
            for (int i = 0; i < Expense.Length; i++)
            {
                if (Expense[i].Amount > 0)
                {
                    BW.WriteLine("    {0,-20} {1,12} {2,12}", Expense[i].Name, "$" + Expense[i].Amount.ToString("n2"), Expense[i].Percentage.ToString("n2") + "%");
                }
            }
            BW.WriteLine("    {0,-20} {1,12}", "EXPENSES TOTAL", "$" + ExpenseTotal.ToString("n2"));
            BW.WriteLine();
            #endregion
            BW.WriteLine("    {0,-20} {1,12}", "LEFTOVER MONEY", "$" + LeftOver.ToString("n2"));
            BW.Close();  //close file
        }

        static void DisplayBudget(bool PrintOptions = true)
        {
            if (PrintOptions)  //if you want menu wit no options, set to false
            {
                Print("\nBudget report is ready, What would you like to do?  ");
                Print("     M - Return to Main Menu");
                Print("     X - Exit");
                Print("     D - Display on Screen");
                Print("     O - Open Report File");
                Print("     S - Speak & Display");
            }

            SaveReport();  //write report to file
            string BudgetContents = File.ReadAllText(BudgetReport);  //contents of file to string
            char KP = WaitForKeyPress();  //get a key

            switch (KP)  //actions based on key M X D O S
            {
                case 'm':
                    MainMenu();
                    break;
                case 'x':
                    Environment.Exit(0);  //exit app
                    break;
                case 'd':
                    Console.Clear();  //clear screen
                    Console.WriteLine(BudgetContents);//write report to screen
                    WaitForKeyPress("\nPress a key to return to main menu...");
                    MainMenu();
                    break;
                case 's':
                    var Bud = new SpeechSynthesizer
                    {
                        Rate = 3  //speech speed
                    };  //new speech synth
                    Console.Clear();
                    Console.WriteLine(BudgetContents);  //display report first
                    Bud.SpeakAsync(BudgetContents); //speak then immediately continue program
                    WaitForKeyPress("\nPress a key to return to main menu...");
                    MainMenu();
                    break;
                case 'o':  //queso
                    Process.Start(BudgetReport);  //open text file with associated program
                    MainMenu();
                    break;
                default:  //bad key
                    DisplayBudget(false);
                    break;
            }
        }

        static char WaitForKeyPress(string prompt = "")  //get a keypress and return it lowercase
        {
            Console.Write(prompt + "  ");  //print prompt string
            ConsoleKeyInfo keyInfo = Console.ReadKey(true); // Prevents the key from being displayed
            return Char.ToLower(keyInfo.KeyChar);
        }

        static void GetTotals()  //get income and expense totals
        {
            IncomeTotal = 0; ExpenseTotal = 0;  //clear values
            for (int i = 0; i < Income.Length; i++)  //loop through entire array
            {
                IncomeTotal += Income[i].Amount;  //keep running income total
            }  //total all income array values

            for (int i = 0; i < Income.Length; i++)  //for each element, store percentage of total income
            {
                Income[i].Percentage = Income[i].Amount / IncomeTotal * 100;
            }

            for (int i = 0; i < Expense.Length; i++) //expense loop
            {
                ExpenseTotal += Expense[i].Amount;
            }

            for (int i = 0; i < Expense.Length; i++)  //for each element, store percentage of total expenses
            {
                Expense[i].Percentage = Expense[i].Amount / ExpenseTotal * 100;
            }

            LeftOver = IncomeTotal - ExpenseTotal;  //get left over money
        }

        static double GetNumber(String Prompt, int Low, int High)  //get a number from thee user
        {
            string line; double rtn;  //line read and number returned
            while (true)  //loop until valid input
            {
                Console.Write(Prompt);  //display prompt message before getting input
                line = Console.ReadLine();  //store input

                if (double.TryParse(line, out rtn))  //if string can convert to double
                {
                    // Successfully parsed, exit the loop
                    break;
                }
                else
                {
                    // Invalid input, prompt the user again
                    System.Media.SystemSounds.Asterisk.Play();  //play a sound 
                }
            }

            if (rtn < Low || rtn > High)  //bounds check
            {
                System.Media.SystemSounds.Asterisk.Play();
                GetNumber(Prompt, Low, High);
            }

            return rtn;  //return number, all checks passed
        }

        static void Print(string msg = "", bool SameLine = false)  //write to console.  default is blank line with new line.
        {
            if (SameLine == true)  //if no newline desired
            {
                Console.Write(msg);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }

        static void LoadFiles()  //load income and expense files
        {
            Array.Clear(Income, 0, 19);  //clear arrays
            Array.Clear(Expense, 0, 19);
            string line; int Index = 0;
            StreamReader IncomeReader = new StreamReader(IncomeList);

            while (!IncomeReader.EndOfStream)  //loop until EOF
            {
                line = IncomeReader.ReadLine();  //read a line from file
                string[] subs = line.Split(',');  //split string by ,
                Income[Index].Name = subs[0];//store names in array.  :) Sub Zero
                Income[Index].Amount = Convert.ToDouble(subs[1]);  //amount
                Index++;  //go to next slot
            }
            IncomeReader.Close();  //close file

            StreamReader ExpenseReader = new StreamReader(ExpenseList);

            Index = 0;
            while (!ExpenseReader.EndOfStream)  //loop until EOF
            {
                line = ExpenseReader.ReadLine();  //read a line from file
                string[] subs = line.Split(',');  //split string by ,
                Expense[Index].Name = subs[0];  //sub zero, johnny cage, raiden, kano, llu kankg
                Expense[Index].Amount = Convert.ToDouble(subs[1]);
                Index++;
            }
            ExpenseReader.Close();
        }

        static void EditFile(string filename)  //edit file
        {
            Console.Clear();
            Print("HOW TO EDIT THE INCOME & EXPENSE FILES:\n");  //describe how to construct file
            Print("The Columns Name and Amount are separated");
            Print("by commas, for exampple,");
            Print("NAME,AMOUNT");
            Print("Social Security,900.00.");
            Print("Work Income - Microsoft,9598.00");
            Print("cell phone,266.86");
            WaitForKeyPress("\nPress a key to open the file...");

            Process P = new Process();  //process
            P.StartInfo.FileName = "notepad.exe"; //program to run
            P.StartInfo.Arguments = filename;  //file to open
            P.StartInfo.UseShellExecute = false;

            P.Start();  //start process
            P.WaitForExit();  //wait until program exits

            LoadFiles();  //reload arrays with updated file
            MainMenu();
        }

    }  //end class
}  //end namespace