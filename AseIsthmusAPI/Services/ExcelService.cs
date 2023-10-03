using AseIsthmusAPI.Data.AseIsthmusModels;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;
using System.Text;

namespace AseIsthmusAPI.Services
{
    public class ExcelService
    {
        public List<object> ReadContributionsExcelData(string filePath, out string errorFile)
        {
            errorFile = null;
            List<object> data = new List<object>();
            StringBuilder error = new StringBuilder();
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
            {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                SharedStringTablePart sharedStringTablePart = workbookPart.SharedStringTablePart;

                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                bool isFirstRow = true;
                var counter = 1;

                foreach (Row row in sheetData.Elements<Row>())
                {
                    if (isFirstRow)
                    {
                        isFirstRow = false;
                        continue;
                    }
                    //else if (last)
                     counter ++ ;

                    
                    string userId = null;
                    string employeeBalanceValue = null;
                    string employerBalanceValue = null;                  
                    DateTime? deductionDate = null;

                    foreach (Cell cell in row.Elements<Cell>())
                    {

                        string cellValue = GetCellValue(cell, workbookPart);

                        string columnName = GetColumnName(cell.CellReference);

                        switch (columnName)
                        {
                            case "A":
                                userId = cellValue;
                                break;
                            case "B":
                                deductionDate = ParseDateTime(cellValue);
                                break;
                            case "C":
                                employeeBalanceValue = cellValue;
                                break;
                            case "D":
                                employerBalanceValue = cellValue;
                                break;                           
                            default:

                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(userId))
                    {
                        if (!string.IsNullOrEmpty(employeeBalanceValue) && !string.IsNullOrEmpty(employerBalanceValue))
                        {
                            try {
                                ContributionBalance contributionBalance = new ContributionBalance
                                {
                                    PersonId = userId,
                                    EmployeeContribution = decimal.Parse(employeeBalanceValue),
                                    EmployerContribution = decimal.Parse(employerBalanceValue),
                                    DeductedDate = (DateTime)deductionDate
                                };

                                data.Add(contributionBalance);
                            
                            } catch  (Exception ex){
                                error.AppendLine($"No se pudo agregar la fila {counter}. Error de formato. {ex.Message} \r\n");
                            }                          
                        }
                        else {
                            var errorNull = "";

                            if (string.IsNullOrEmpty(employeeBalanceValue))
                                errorNull = "La columna de Ahorro es requerida";

                            if (!string.IsNullOrEmpty(errorNull) && string.IsNullOrEmpty(employerBalanceValue))
                            {
                                errorNull += " y la columna de Aporte es requerida";
                            }
                            else if (string.IsNullOrEmpty(errorNull) && !string.IsNullOrEmpty(employerBalanceValue))
                            {
                                errorNull = "La columna de Aporte es requerida";
                            }

                            error.AppendLine($"No se pudo agregar la fila {counter}. {errorNull}. \r\n");
                        }
                    }
                }
            }
            if (error.Length > 0) {
                 errorFile = @"C:\Users\karen.rudin\Documents\log.txt";
                using (StreamWriter file = new StreamWriter(errorFile))
                {
                    file.WriteLine(error.ToString()); 
                }
                return data;
            }
            return data;

        }

        public List<object> ReadLoansExcelData(string filePath, out string errorFile)
        {
                errorFile = null;
                List<object> data = new List<object>();
                StringBuilder error = new StringBuilder();
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    SharedStringTablePart sharedStringTablePart = workbookPart.SharedStringTablePart;

                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                    bool isFirstRow = true;
                    var counter = 1;

                    foreach (Row row in sheetData.Elements<Row>())
                    {
                        if (isFirstRow)
                        {
                            isFirstRow = false;
                            continue;
                        }
                        counter++;



                        string userId = null;
                    string loanRequestId = null;
                    string paymentNumber = null;
                    string beginningBalance = null;
                    string scheduledPayment = null;
                    string extraPayment = null;
                    string totalPayment = null;
                    string principal = null;
                    string interest = null;
                    string endingBalance = null;
                    string cumulativeInterest = null;
                    string interestRate = null;
                    string currency = null;
                    DateTime? deductionDate = null;

                        foreach (Cell cell in row.Elements<Cell>())
                        {

                            string cellValue = GetCellValue(cell, workbookPart);

                            string columnName = GetColumnName(cell.CellReference);

                            switch (columnName)
                            {
                                case "A":
                                    userId = cellValue;
                                    break;
                                case "B":
                                    deductionDate = ParseDateTime(cellValue);
                                    break;
                                case "C":
                                loanRequestId = cellValue;
                                break;
                            case "D":
                                paymentNumber = cellValue;
                                break;
                            case "E":
                                beginningBalance = cellValue;
                                break;
                            case "F":
                                scheduledPayment = cellValue;
                                break;
                            case "G":
                                extraPayment = cellValue;
                                break;
                            case "H":
                                totalPayment = cellValue;
                                break;
                            case "I":
                                principal = cellValue;
                                break;
                            case "J":
                                interest = cellValue;
                                break;
                            case "K":
                                endingBalance = cellValue;
                                break;
                            case "L":
                                cumulativeInterest = cellValue;
                                break;
                            case "M":
                                interestRate = cellValue;
                                break;
                            case "N":
                                currency = cellValue;
                                break;
                                default:
                                    break;
                            }
                        }

                        if (!string.IsNullOrEmpty(userId))
                        {
                            if (!string.IsNullOrEmpty(loanRequestId) && !string.IsNullOrEmpty(paymentNumber) && !string.IsNullOrEmpty(beginningBalance) && !string.IsNullOrEmpty(scheduledPayment) && !string.IsNullOrEmpty(extraPayment) && !string.IsNullOrEmpty(totalPayment) && !string.IsNullOrEmpty(principal) && !string.IsNullOrEmpty(interest) && !string.IsNullOrEmpty(endingBalance) && !string.IsNullOrEmpty(cumulativeInterest) && !string.IsNullOrEmpty(interestRate) && !string.IsNullOrEmpty(currency))

                            {
                                try
                                {
                                LoanBalance LoanBalance = new LoanBalance
                                {
                                        PersonId = userId,
                                        LoanRequestId = int.Parse(loanRequestId),
                                        PaymentNumber = int.Parse(paymentNumber),
                                        BeginningBalance = decimal.Parse(beginningBalance),
                                        ScheduledPayment = decimal.Parse(scheduledPayment),
                                        ExtraPayment = decimal.Parse(extraPayment),
                                        TotalPayment = decimal.Parse(totalPayment),
                                        Principal = decimal.Parse(principal),
                                        Interest = decimal.Parse(interest),
                                        EndingBalance = decimal.Parse(endingBalance),
                                        CumulativeInterest = decimal.Parse(cumulativeInterest),
                                        InterestRate = decimal.Parse(interestRate),
                                        Currency = currency,
                                        PaymentDate = (DateTime)deductionDate
                                    };

                                    data.Add(LoanBalance);

                                }
                                catch (Exception ex)
                                {
                                    error.AppendLine($"No se pudo agregar la fila {counter}. Error de formato. {ex.Message} \r\n");
                                }
                            }
                            else
                            {
                                var errorNull = "";

                                if (string.IsNullOrEmpty(loanRequestId))
                                    errorNull = "La columna de Loan Id es requerida";

                                if (!string.IsNullOrEmpty(errorNull) && string.IsNullOrEmpty(paymentNumber))
                                {
                                    errorNull += " y la columna de Pmt No. es requerida";
                                }
                                else if (string.IsNullOrEmpty(errorNull) && !string.IsNullOrEmpty(paymentNumber))
                                {
                                    errorNull = "La columna de Pmt No. es requerida";
                                }
                            if (!string.IsNullOrEmpty(errorNull) && string.IsNullOrEmpty(beginningBalance))
                            {
                                errorNull += " y la columna de Beginning Balance es requerida";
                            }
                            else if (string.IsNullOrEmpty(errorNull) && !string.IsNullOrEmpty(beginningBalance))
                            {
                                errorNull = "La columna deBeginning Balance es requerida";
                            }
                          
                            if (!string.IsNullOrEmpty(errorNull) && string.IsNullOrEmpty(scheduledPayment))
                            {
                                errorNull += " y la columna de Scheduled Payment es requerida";
                            }
                            else if (string.IsNullOrEmpty(errorNull) && !string.IsNullOrEmpty(scheduledPayment))
                            {
                                errorNull = "La columna de Scheduled Payment es requerida";
                            }
                         
                            if (!string.IsNullOrEmpty(errorNull) && string.IsNullOrEmpty(totalPayment))
                            {
                                errorNull += " y la columna de Total Payment es requerida";
                            }
                            else if (string.IsNullOrEmpty(errorNull) && !string.IsNullOrEmpty(totalPayment))
                            {
                                errorNull = "La columna de Total Payment es requerida";
                            }
                            if (!string.IsNullOrEmpty(errorNull) && string.IsNullOrEmpty(interest))
                            {
                                errorNull += " y la columna de Interest es requerida";
                            }
                            else if (string.IsNullOrEmpty(errorNull) && !string.IsNullOrEmpty(interest))
                            {
                                errorNull = "La columna de Interest es requerida";
                            }
                            if (!string.IsNullOrEmpty(errorNull) && string.IsNullOrEmpty(principal))
                            {
                                errorNull += " y la columna de Principal es requerida";
                            }
                            else if (string.IsNullOrEmpty(errorNull) && !string.IsNullOrEmpty(principal))
                            {
                                errorNull = "La columna de Principal es requerida";
                            }
                            if (!string.IsNullOrEmpty(errorNull) && string.IsNullOrEmpty(endingBalance))
                            {
                                errorNull += " y la columna de Ending Balance es requerida";
                            }
                            else if (string.IsNullOrEmpty(errorNull) && !string.IsNullOrEmpty(endingBalance))
                            {
                                errorNull = "La columna de Ending Balance es requerida";
                                if (!string.IsNullOrEmpty(errorNull) && string.IsNullOrEmpty(cumulativeInterest))
                                {
                                    errorNull += " y la columna de Cumulative Interest es requerida";
                                }
                                else if (string.IsNullOrEmpty(errorNull) && !string.IsNullOrEmpty(cumulativeInterest))
                                {
                                    errorNull = "La columna de Cumulative Interest es requerida";
                                }
                                ///////////////
                                if (!string.IsNullOrEmpty(errorNull) && string.IsNullOrEmpty(interestRate))
                                {
                                    errorNull += " y la columna de Interest Rate es requerida";
                                }
                                else if (string.IsNullOrEmpty(errorNull) && !string.IsNullOrEmpty(interestRate))
                                {
                                    errorNull = "La columna de Interest Rate es requerida";
                                }
                                ///////////////
                                if (!string.IsNullOrEmpty(errorNull) && string.IsNullOrEmpty(currency))
                                {
                                    errorNull += " y la columna de Currency es requerida";
                                }
                                else if (string.IsNullOrEmpty(errorNull) && !string.IsNullOrEmpty(currency))
                                {
                                    errorNull = "La columna de Currency es requerida";
                                }                              
                            }

                            error.AppendLine($"No se pudo agregar la fila {counter}. {errorNull}. \r\n");
                            }
                        }
                    }
                }
                if (error.Length > 0)
                {
                    errorFile = @"C:\Users\karen.rudin\Documents\log.txt";
                    using (StreamWriter file = new StreamWriter(errorFile))
                    {
                        file.WriteLine(error.ToString());
                    }
                    return data;
                }
                return data;

            }

        public List<object> ReadSavingsExcelData(string filePath, out string errorFile)
        {
            errorFile = null;
            List<object> data = new List<object>();
            StringBuilder error = new StringBuilder();
            using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filePath, false))
            {
                WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                SharedStringTablePart sharedStringTablePart = workbookPart.SharedStringTablePart;

                SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

                bool isFirstRow = true;
                var counter = 1;

                foreach (Row row in sheetData.Elements<Row>())
                {
                    if (isFirstRow)
                    {
                        isFirstRow = false;
                        continue;
                    }
                    //else if (last)
                    counter++;


                    string userId = null;
                    DateTime? deductionDate = null;
                    string savingsId = null;
                    string deductedAmount = null;
                    string installment = null;

                    foreach (Cell cell in row.Elements<Cell>())
                    {

                        string cellValue = GetCellValue(cell, workbookPart);

                        string columnName = GetColumnName(cell.CellReference);

                        switch (columnName)
                        {
                            case "A":
                                userId = cellValue;
                                break;
                            case "B":
                                deductionDate = ParseDateTime(cellValue);
                                break;
                            case "C":
                                savingsId = cellValue;
                                break;
                            case "D":
                                deductedAmount = cellValue;
                                break;
                            case "E":
                                installment = cellValue;
                                break;
                            default:

                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(userId))
                    {
                        if (!string.IsNullOrEmpty(savingsId) && !string.IsNullOrEmpty(deductedAmount) && !string.IsNullOrEmpty(installment))
                        {
                            try
                            {
                                SavingsBalance savingsBalance = new SavingsBalance
                                {
                                    PersonId = userId,
                                    SavingsRequestId = int.Parse(savingsId),
                                    LastAmountDeducted = decimal.Parse(deductedAmount),
                                    LastDeductedDate = (DateTime)deductionDate,
                                    Installment = int.Parse(installment),
                                };

                                data.Add(savingsBalance);

                            }
                            catch (Exception ex)
                            {
                                error.AppendLine($"No se pudo agregar la fila {counter}. Error de formato. {ex.Message} \r\n");
                            }
                        }
                        else
                        {
                            var errorNull = "";

                            if (string.IsNullOrEmpty(savingsId))
                                errorNull = "La columna de Installment es requerida";

                            if (!string.IsNullOrEmpty(errorNull) && string.IsNullOrEmpty(deductedAmount))
                            {
                                errorNull += " y la columna de Deducted Amount es requerida";
                            }
                            else if (string.IsNullOrEmpty(errorNull) && !string.IsNullOrEmpty(deductedAmount))
                            {
                                errorNull = "La columna de Deducted Amount es requerida";
                            }

                            if (!string.IsNullOrEmpty(errorNull) && string.IsNullOrEmpty(installment))
                            {
                                errorNull += " y la columna de Installment es requerida";
                            }
                            else if (string.IsNullOrEmpty(errorNull) && !string.IsNullOrEmpty(installment))
                            {
                                errorNull = "La columna de Installment es requerida";
                            }

                            error.AppendLine($"No se pudo agregar la fila {counter}. {errorNull}. \r\n");
                        }
                    }
                }
            }
            if (error.Length > 0)
            {
                errorFile = @"C:\Users\karen.rudin\Documents\log.txt";
                using (StreamWriter file = new StreamWriter(errorFile))
                {
                    file.WriteLine(error.ToString());
                }
                return data;
            }
            return data;

        }

        private string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            if (cell == null)
            {
                return string.Empty;
            }

            string cellValue = string.Empty;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                int sharedStringId;
                if (int.TryParse(cell.InnerText, out sharedStringId))
                {
                    SharedStringTablePart sharedStringPart = workbookPart.SharedStringTablePart;
                    var sharedStringItem = sharedStringPart.SharedStringTable.Elements<SharedStringItem>().ElementAtOrDefault(sharedStringId);
                    cellValue = sharedStringItem != null ? sharedStringItem.InnerText : string.Empty;
                }
            }
            else
            {
                cellValue = cell.InnerText;
            }

            return cellValue;
        }

        private string GetColumnName(string cellReference)
        {
            string columnName = string.Empty;

            foreach (char c in cellReference)
            {
                if (Char.IsLetter(c))
                {
                    columnName += c;
                }
                else
                {
                    break;
                }
            }

            return columnName;
        }

        private DateTime? ParseDateTime(string value)
        {
            DateTime dateTime;
            if (DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
            {
                return dateTime;
            }
            return null;
        }
    }
}
