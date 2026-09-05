# Console Budget Report

A simple and intuitive console-based budgeting application for Windows that helps you track income and expenses, calculate percentages, and generate monthly budget reports.

## Features

- 📊 **Budget Tracking**: Manage income sources and expenses in separate CSV files
- 💰 **Automatic Calculations**: Computes totals, percentages, and leftover money
- 📄 **Report Generation**: Creates formatted monthly budget reports
- 🔊 **Text-to-Speech**: Speaks all program text and budget reports
- 📝 **Easy Editing**: Edit income and expense files directly in Notepad
- 💾 **Data Persistence**: Stores data in CSV format in your Documents folder

## Requirements

- .NET 10 SDK (required to build and run)
- Windows 10/11 (recommended). The application is a console app and is tested on Windows.
- Notepad or any text editor (for CSV file editing)

## Installation

1. Clone the repository:
   ```
   git clone https://github.com/HiTechCharles/Console-Budget-Report.git
   ```

2. Using the dotnet CLI (recommended):
   ```powershell
   cd Console-Budget-Report
   dotnet restore
   dotnet build
   dotnet run --project "Console Budget Report.csproj"
   

   Or open the solution in Visual Studio 2022/2024/2026 and Build -> Run.

## Usage

### First Launch

On first launch, the application will:
- Create a folder in your OneDrive Documents or My Documents: `Console Budget Report`
- Generate example `income.csv` and `expenses.csv` files
- Create a `Monthly Budget.txt` report file
- Prompt you to enable text-to-speech functionality

### Main Menu Options

- **D** - Display Budget: View, exit, or return to the main menu
- **I** - Edit Income List: Modify your income sources
- **E** - Edit Expense List: Modify your expenses
- **X** - Exit: Close the application

### Budget Display Options

When viewing your budget, you can:
- **D** - Display on Screen: View the budget report in the console
- **O** - Open Report File: Open the budget report in your default text editor
- **M** - Return to Main Menu
- **X** - Exit the application

### CSV File Format

Both income and expense files use a simple CSV format:

```
Name,Amount
Social Security,900.00
Work Income - Example Company,9598.00
cell phone,266.86
```

**Format Rules:**
- Two columns: Name and Amount
- Separated by a comma (,)
- Amount should be a decimal number
- Each entry on a new line

### Data Storage Location

Files are stored in:
```
%OneDriveConsumer%\Documents\Console Budget Report\
```

Or if OneDrive is not available:
```
%UserProfile%\Documents\Console Budget Report\
```

Files created:
- `income.csv` - Your income sources
- `expenses.csv` - Your expenses
- `Monthly Budget.txt` - Generated budget report

## Budget Report Format

The generated report includes:

```
INCOME SOURCES▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
	Income Name          $1,000.00      100.00%
	INCOME TOTAL         $1,000.00

EXPENSES▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
	Expense Name           $100.00       10.00%
	EXPENSES TOTAL         $100.00

	LEFTOVER MONEY         $900.00
```

Each entry shows:
- Name of income/expense
- Dollar amount
- Percentage of total income/expenses

## Technical Details

- **Language**: C# (.NET 10)
- **Architecture**: Console Application
- **Key Features**:
  - CSV parsing with error handling
  - Text-to-speech synthesis (Windows SAPI)
  - File I/O with exception handling
  - Process management for opening external editors

## Code Structure

- `Transaction` class: Represents income/expense entries
- CSV file loading and parsing
- Budget calculation engine
- Report generation and formatting
- Interactive menu system
- Text-to-speech integration

## Contributing

Contributions are welcome! Feel free to:
- Report bugs
- Suggest new features
- Submit pull requests

## License

This project is open source and available for personal and educational use.

## Author

Charles (HiTechCharles)

## Acknowledgments

Built with simplicity in mind for everyday budget management needs.
