using ShadowFlame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO;
using System.Media;

namespace Console_Budget_Report
{
    internal class Program
    {
        #region TYPES
        // Changed to class to avoid struct mutation issues
        public class Transaction
        {
            public string Name { get; set; }
            public double Amount { get; set; }
            public double Percentage { get; set; }

            public Transaction(string name, double amount)
            {
                Name = name;
                Amount = amount;
                Percentage = 0;
            }
        }
        #endregion

        #region CONFIG
        // Improved fallback to My Documents with better null handling
        public static readonly string BaseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Console Budget Report");

        public static readonly string IncomeListPath = Path.Combine(BaseFolder, "income.csv");
        public static readonly string ExpenseListPath = Path.Combine(BaseFolder, "expenses.csv");
        public static readonly string BudgetReportPath = Path.Combine(BaseFolder, "Monthly Budget.txt");

        private const string ExampleIncome = "Example Income,1000.00";
        private const string ExampleExpense = "Example Expense,100.00";
        #endregion

        #region STATE
        public static List<Transaction> Income { get; } = new List<Transaction>();
        public static List<Transaction> Expense { get; } = new List<Transaction>();

        public static double IncomeTotal { get; private set; }
        public static double ExpenseTotal { get; private set; }
        public static double LeftOver { get; private set; }
        #endregion

        #region SPEECH
        private static ConsoleTTS _tts = new ConsoleTTS(); //text to speech object
        #endregion


        static void Main()
        {
            Console.Title = "Console Budget Report";
            Console.ForegroundColor = ConsoleColor.White;
            Task.WaitAll(_tts?.InitializeAsync() ?? Task.CompletedTask);  //

            try
            {
                Directory.CreateDirectory(BaseFolder);
            }
            catch (Exception ex)
            {
                _tts.SpeakAndDisplay($"Error creating application directory: {ex.Message}");
                WaitForKeyPress("Press any key to exit...");
                return;
            }

            _tts.SpeakAndDisplay("Console Budget Report");
            _tts.SpeakAndDisplay("This program will help you create a budget.");
            _tts.SpeakAndDisplay("Type income and expense names and amounts (CSV: NAME,AMOUNT).");

            EnsureDataFilesExist();
            LoadFiles();
            RunMainMenuLoop();
        }

        // Replaced recursive pattern with loop to prevent stack overflow
        static void RunMainMenuLoop()
        {
            bool running = true;
            bool printOptions = true;
            _tts.Stop();

            while (running)
            {
                if (printOptions)
                {
                    _tts.SpeakAndDisplay("\n\n-----MAIN MENU");
                    _tts.SpeakAndDisplay("     D - Display Budget");
                    _tts.SpeakAndDisplay("     I - Edit Income List");
                    _tts.SpeakAndDisplay("     E - Edit Expense List");
                    _tts.SpeakAndDisplay("     X - Exit");
                }

                char choice = WaitForKeyPress("\n\nWhat would you like to do?");
                printOptions = true; // Reset for next iteration
                _tts.Stop();

                switch (choice)
                {
                    case 'i':
                        EditFile(IncomeListPath);
                        break;
                    case 'e':
                        EditFile(ExpenseListPath);
                        break;
                    case 'd':
                        DisplayBudgetLoop();
                        break;
                    case 'x':
                        running = false;
                        break;
                    default:
                        printOptions = false;
                        _tts.Stop();
                        break;
                }
            }
        }

        static void EnsureDataFilesExist()
        {
            try
            {
                if (!File.Exists(IncomeListPath))
                {
                    File.WriteAllText(IncomeListPath, ExampleIncome + Environment.NewLine);
                }
                if (!File.Exists(ExpenseListPath))
                {
                    File.WriteAllText(ExpenseListPath, ExampleExpense + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating data files: {ex.Message}");
                WaitForKeyPress("Press any key to continue...");
            }
        }

        static void SaveReport()
        {
            LoadFiles();
            ComputeTotals();

            try
            {
                using (var sw = new StreamWriter(BudgetReportPath, false))
                {
                    WriteIncomeSection(sw);
                    sw.WriteLine();
                    WriteExpenseSection(sw);
                    sw.WriteLine();
                    sw.WriteLine("    {0,-20} {1,12}", "LEFTOVER MONEY", FormatCurrency(LeftOver));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving report: {ex.Message}");
                WaitForKeyPress("Press any key to continue...");
            }
        }

        static void WriteIncomeSection(StreamWriter sw)
        {
            sw.WriteLine("INCOME SOURCES▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓");
            foreach (var t in Income)
            {
                if (t.Amount > 0)
                {
                    sw.WriteLine("    {0,-20} {1,12} {2,12}",
                        t.Name,
                        FormatCurrency(t.Amount),
                        FormatPercentage(t.Percentage));
                }
            }
            sw.WriteLine("    {0,-20} {1,12}", "INCOME TOTAL", FormatCurrency(IncomeTotal));
        }

        static void WriteExpenseSection(StreamWriter sw)
        {
            sw.WriteLine("EXPENSES▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓");
            foreach (var t in Expense)
            {
                if (t.Amount > 0)
                {
                    sw.WriteLine("    {0,-20} {1,12} {2,12}",
                        t.Name,
                        FormatCurrency(t.Amount),
                        FormatPercentage(t.Percentage));
                }
            }
            sw.WriteLine("    {0,-20} {1,12}", "EXPENSES TOTAL", FormatCurrency(ExpenseTotal));
        }

        static string FormatCurrency(double amount) => "$" + amount.ToString("n2");
        static string FormatPercentage(double percentage) => percentage.ToString("n2") + "%";

        static void DisplayBudgetLoop()
        {
            bool running = true;
            bool printOptions = true;

            while (running)
            {
                if (printOptions)
                {
                    _tts.SpeakAndDisplay("\nBudget report is ready, What would you like to do?");
                    _tts.SpeakAndDisplay("     M - Return to Main Menu");
                    _tts.SpeakAndDisplay("     X - Exit");
                    _tts.SpeakAndDisplay("     D - Display on Screen");
                    _tts.SpeakAndDisplay("     O - Open Report File");
                }

                SaveReport();
                string budgetContents = File.ReadAllText(BudgetReportPath);
                char choice = WaitForKeyPress();
                printOptions = true;
                _tts.Stop();

                switch (choice)
                {
                    case 'm':
                        return; // Return to main menu loop
                    case 'x':
                        Environment.Exit(0);
                        break;
                    case 'd':
                        DisplayOnScreen(budgetContents);
                        return;
                    case 'o':
                        OpenReportFile();
                        return;
                    default:
                        printOptions = false;
                        break;
                }
            }
        }

        static void DisplayOnScreen(string content)
        {
            Console.Clear();
            _tts.SpeakAndDisplay(content);
            WaitForKeyPress("\nPress a key to return to main menu...");
        }

        static void OpenReportFile()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = BudgetReportPath,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error opening report file: {ex.Message}");
                WaitForKeyPress("Press any key to continue...");
            }
        }

        static char WaitForKeyPress(string prompt = "")
        {
            if (!string.IsNullOrEmpty(prompt))
                Console.Write(prompt + "  ");
            return char.ToLower(Console.ReadKey(true).KeyChar);
        }

        static void ComputeTotals()
        {
            IncomeTotal = 0;
            ExpenseTotal = 0;

            foreach (var t in Income)
                IncomeTotal += t.Amount;

            foreach (var t in Expense)
                ExpenseTotal += t.Amount;

            // Calculate percentages (now simpler with class instead of struct)
            CalculatePercentages(Income, IncomeTotal);
            CalculatePercentages(Expense, ExpenseTotal);

            LeftOver = IncomeTotal - ExpenseTotal;
        }

        static void CalculatePercentages(List<Transaction> transactions, double total)
        {
            if (total > 0)
            {
                foreach (var t in transactions)
                {
                    t.Percentage = t.Amount / total * 100.0;
                }
            }
            else
            {
                foreach (var t in transactions)
                {
                    t.Percentage = 0;
                }
            }
        }

        static double GetNumber(string prompt, double low, double high)
        {
            while (true)
            {
                Console.Write(prompt);
                string line = Console.ReadLine();

                if (double.TryParse(line, NumberStyles.Number, CultureInfo.CurrentCulture, out double value))
                {
                    if (value >= low && value <= high)
                        return value;

                    SystemSounds.Asterisk.Play();
                    Console.WriteLine($"Value must be between {low} and {high}.");
                }
                else
                {
                    SystemSounds.Asterisk.Play();
                    Console.WriteLine("Invalid number format.");
                }
            }
        }

        static void LoadFiles()
        {
            Income.Clear();
            Expense.Clear();

            try
            {
                LoadTransactionsFromFile(IncomeListPath, Income);
                LoadTransactionsFromFile(ExpenseListPath, Expense);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading data files: {ex.Message}");
                WaitForKeyPress("\nPress a key to continue...");
            }
        }

        static void LoadTransactionsFromFile(string filePath, List<Transaction> transactions)
        {
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(new[] { ',' }, 2);
                if (parts.Length < 2)
                    continue;

                var name = parts[0].Trim();
                // Truncate name to 20 characters
                if (name.Length > 20)
                    name = name.Substring(0, 20);

                if (double.TryParse(parts[1].Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out double amount))
                {
                    transactions.Add(new Transaction(name, amount));
                }
            }

            // Sort by amount descending (highest to lowest)
            transactions.Sort((a, b) => b.Amount.CompareTo(a.Amount));
        }

        static void EditFile(string filename)
        {
            Console.Clear();
            _tts.SpeakAndDisplay("HOW TO EDIT THE INCOME & EXPENSE FILES:\n");
            _tts.SpeakAndDisplay("Columns: Name and Amount separated by a comma.");
            _tts.SpeakAndDisplay("Example:");
            _tts.SpeakAndDisplay("Social Security,900.00");
            _tts.SpeakAndDisplay("Work Income - Example Company,9598.00");
            _tts.SpeakAndDisplay("cell phone,266.86");
            WaitForKeyPress("\nPress a key to open the file...");
            _tts.Stop();

            try
            {
                using (var p = Process.Start(new ProcessStartInfo
                {
                    FileName = "notepad.exe",
                    Arguments = $"\"{filename}\"",
                    UseShellExecute = false
                }))
                {
                    p?.WaitForExit();
                }

                LoadFiles();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error editing file: {ex.Message}");
                WaitForKeyPress("Press any key to continue...");
            }
        }
    }
}
