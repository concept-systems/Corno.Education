using System;
using System.Globalization;
using System.Reflection;
using Telerik.Reporting;

namespace Corno.Reports;

public class TelerikReportHelper
{
    #region -- Data Members --

    //ILoginCredentials loginCredentials = (ILoginCredentials)HttpContext.Current.Session[StringConstants.loginCredetinals];

    #endregion

    #region -- Methods --

    /*/// <summary>
    ///     To Make Visible Parameters.
    /// </summary>
    /// <param name="itemCollection"></param>
    /// <param name="dataTable"></param>
    public static void MakefieldsInvisible(ReportItemBase.ItemCollection itemCollection, DataTable dataTable)
    {
        // Remove all unnecessary items
        foreach (var reportItem in itemCollection)
        {
            if (reportItem.GetType() == typeof(TextBox))
            {
                if (reportItem.Name.Substring(0, 3) == "txt" || reportItem.Name.Substring(0, 3) == "lbl")
                {
                    var fieldName = reportItem.Name.Remove(0, 3);
                    var contains = dataTable.Columns.Contains(fieldName);
                    if (false == contains)
                    {
                        reportItem.Visible = false;
                        ((TextBox) reportItem).Value = string.Empty;
                    }
                }
            }

            if (reportItem.Items.Count > 0)
                MakefieldsInvisible(reportItem.Items, dataTable);
        }
    }*/

    /*public static ArrayList GetSelectedValues(string columnName, DataTable dataTable)
    {
        var selectedValues = new ArrayList();
        foreach (DataRow filterRow in dataTable.Rows)
        {
            if (string.Empty == filterRow[columnName].ToString() ||
                "" == filterRow[columnName].ToString())
                continue;

            if (false == selectedValues.Contains(filterRow[columnName]))
            {
                selectedValues.Add(filterRow[columnName]);
            }
        }

        return selectedValues;
    }*/

    /*/// <summary>
    /// </summary>
    /// <returns></returns>
    public static string GetAmountInWords(double totalAmount)
    {
        var l_sInWords = "";
        try
        {
            var l_sResult = "";
            var strArray = totalAmount.ToString().Split('.');

            var l_dValue = Convert.ToDecimal(strArray[0]);
            //decimal decimalValue = 0;
            //if (strArray.Length > 1)
            //    decimalValue = Convert.ToDecimal(strArray[1]);

            if (l_dValue.ToString().Substring(0, 1) == "-")
                return "";
            var l_iValue = Convert.ToInt64(l_dValue);
            var l_sTotal = l_iValue.ToString();
            if (l_sTotal.Length >= 10)
                return l_sInWords;
            long l_iIndex = 0;
            while (l_sTotal.Length > l_iIndex)
            {
                switch (l_sTotal.Length)
                {
                    case 1:
                        l_sResult = ReturnOneToNinteen(l_iValue);
                        l_sInWords += " " + l_sResult;
                        l_iValue = l_iValue % 1;
                        break;
                    case 2:
                        if (l_iValue > 19)
                            l_sResult = ReturnZeroToNine(l_iValue / 10);
                        else
                        {
                            l_sResult = ReturnZeroToNine(l_iValue);
                            l_sInWords += " " + l_sResult + " Only";
                            return l_sInWords;
                        }
                        l_sInWords += " " + l_sResult;
                        l_iValue = l_iValue % 10;
                        break;
                    case 3:
                        l_sResult = ReturnOneToNinteen(l_iValue / 100);
                        l_sInWords += " " + l_sResult + " Hundred";
                        l_iValue = l_iValue % 100;
                        break;
                    case 4:
                        l_sResult = ReturnOneToNinteen(l_iValue / 1000);
                        l_sInWords += " " + l_sResult + " Thousand";
                        l_iValue = l_iValue % 1000;
                        break;
                    case 5:
                        var l_iResult = l_iValue / 1000;
                        l_sResult = ReturnZeroToNine(l_iResult);
                        if (l_iResult > 9 && l_iResult < 20)
                        {
                            l_sInWords += " " + l_sResult + " Thousand";
                            l_iValue = l_iValue % 1000;
                        }
                        else if (l_iResult > 19 && l_iResult.ToString().Contains("0"))
                        {
                            l_sInWords += " " + l_sResult + " Thousand";
                            l_iValue = l_iValue % 1000;
                        }
                        else
                        {
                            l_sInWords += " " + l_sResult;
                            l_iValue = l_iValue % 10000;
                        }
                        break;
                    case 6:
                        l_sResult = ReturnOneToNinteen(l_iValue / 100000);
                        l_sInWords += " " + l_sResult + " Lacs";
                        l_iValue = l_iValue % 100000;
                        break;
                    case 7:
                        l_iResult = l_iValue / 100000;
                        l_sResult = ReturnZeroToNine(l_iResult);
                        if (l_iResult > 9 && l_iResult < 20)
                        {
                            l_sInWords += " " + l_sResult + " Lacs";
                            l_iValue = l_iValue % 100000;
                        }
                        else
                        {
                            l_sInWords += " " + l_sResult;
                            l_iValue = l_iValue % 1000000;
                        }
                        break;
                    case 8:
                        l_sResult = ReturnOneToNinteen(l_iValue / 10000000);
                        l_sInWords += " " + l_sResult + " Crores";
                        l_iValue = l_iValue % 10000000;
                        break;
                    case 9:
                        l_iResult = l_iValue / 10000000;
                        l_sResult = ReturnZeroToNine(l_iResult);
                        if (l_iResult > 9 && l_iResult < 20)
                        {
                            l_sInWords += " " + l_sResult + "Crores";
                            l_iValue = l_iValue % 10000000;
                        }
                        else
                        {
                            l_sInWords += " " + l_sResult;
                            l_iValue = l_iValue % 100000000;
                        }
                        break;
                    default:
                        break;
                }
                l_sTotal = l_iValue.ToString();
                if (l_sTotal == "0")
                {
                    //if (decimalValue > 0)
                    //{
                    //    l_sTotal = decimalValue.ToString();
                    //    l_iValue = Convert.ToInt64(decimalValue);
                    //    l_sInWords += " and paise";
                    //    decimalValue = 0;
                    //}
                    //else
                    break;
                }
            }
            l_sInWords += " Only";
        }
        catch
        {
            l_sInWords += "Zero Only";
        }

        return l_sInWords;
    }*/

    public static string GetAmountInWords(double totalAmount)
    {
        return GetAmountInWords(totalAmount.ToString(CultureInfo.InvariantCulture));
    }


    public static string GetAmountInWords(string totalAmount)
    {
        var inWords = string.Empty;
        try
        {
            var strArray = totalAmount.Split('.');

            var l_dValue = Convert.ToDecimal(strArray[0]);
            //decimal decimalValue = 0;
            //if (strArray.Length > 1)
            //    decimalValue = Convert.ToDecimal(strArray[1]);

            if (l_dValue.ToString(CultureInfo.InvariantCulture).Substring(0, 1) == "-")
                return "";
            var l_iValue = Convert.ToInt64(l_dValue);
            var l_sTotal = l_iValue.ToString();
            if (l_sTotal.Length >= 10)
                return inWords;
            long l_iIndex = 0;
            while (l_sTotal.Length > l_iIndex)
            {
                var result = "";
                switch (l_sTotal.Length)
                {
                    case 1:
                        result = ReturnOneToNineteen(l_iValue);
                        inWords += " " + result;
                        l_iValue = l_iValue % 1;
                        break;
                    case 2:
                        if (l_iValue > 19)
                            result = ReturnZeroToNine(l_iValue / 10);
                        else
                        {
                            result = ReturnZeroToNine(l_iValue);
                            inWords += " " + result + " Only";
                            return inWords;
                        }
                        inWords += " " + result;
                        l_iValue = l_iValue % 10;
                        break;
                    case 3:
                        result = ReturnOneToNineteen(l_iValue / 100);
                        inWords += " " + result + " Hundred";
                        l_iValue = l_iValue % 100;
                        break;
                    case 4:
                        result = ReturnOneToNineteen(l_iValue / 1000);
                        inWords += " " + result + " Thousand";
                        l_iValue = l_iValue % 1000;
                        break;
                    case 5:
                        var l_iResult = l_iValue / 1000;
                        result = ReturnZeroToNine(l_iResult);
                        if (l_iResult > 9 && l_iResult < 20)
                        {
                            inWords += " " + result + " Thousand";
                            l_iValue = l_iValue % 1000;
                        }
                        else if (l_iResult > 19 && l_iResult.ToString().Contains("0"))
                        {
                            inWords += " " + result + " Thousand";
                            l_iValue = l_iValue % 1000;
                        }
                        else
                        {
                            inWords += " " + result;
                            l_iValue = l_iValue % 10000;
                        }
                        break;
                    case 6:
                        result = ReturnOneToNineteen(l_iValue / 100000);
                        inWords += " " + result + " Lacs";
                        l_iValue = l_iValue % 100000;
                        break;
                    case 7:
                        l_iResult = l_iValue / 100000;
                        result = ReturnZeroToNine(l_iResult);
                        if (l_iResult > 9 && l_iResult < 20)
                        {
                            inWords += " " + result + " Lacs";
                            l_iValue = l_iValue % 100000;
                        }
                        else
                        {
                            inWords += " " + result;
                            l_iValue = l_iValue % 1000000;
                        }
                        break;
                    case 8:
                        result = ReturnOneToNineteen(l_iValue / 10000000);
                        inWords += " " + result + " Crores";
                        l_iValue = l_iValue % 10000000;
                        break;
                    case 9:
                        l_iResult = l_iValue / 10000000;
                        result = ReturnZeroToNine(l_iResult);
                        if (l_iResult > 9 && l_iResult < 20)
                        {
                            inWords += " " + result + "Crores";
                            l_iValue = l_iValue % 10000000;
                        }
                        else
                        {
                            inWords += " " + result;
                            l_iValue = l_iValue % 100000000;
                        }
                        break;
                    default:
                        break;
                }
                l_sTotal = l_iValue.ToString();
                if (l_sTotal == "0")
                {
                    //if (decimalValue > 0)
                    //{
                    //    l_sTotal = decimalValue.ToString();
                    //    l_iValue = Convert.ToInt64(decimalValue);
                    //    l_sInWords += " and paise";
                    //    decimalValue = 0;
                    //}
                    //else
                    break;
                }
            }
            inWords += " Only";
        }
        catch
        {
            inWords += "Zero Only";
        }

        return inWords;
    }

    /// <summary>
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ReturnZeroToNine(long value)
    {
        var inWords = "";

        if (value > 19)
            value = value / 10;
        switch (value)
        {
            case 0:
                inWords = "Zero";
                break;
            case 1:
                inWords = "Ten";
                break;
            case 2:
                inWords = "Twenty";
                break;
            case 3:
                inWords = "Thirty";
                break;
            case 4:
                inWords = "Fourty";
                break;
            case 5:
                inWords = "Fifty";
                break;
            case 6:
                inWords = "Sixty";
                break;
            case 7:
                inWords = "Seventy";
                break;
            case 8:
                inWords = "Eighty";
                break;
            case 9:
                inWords = "Ninty";
                break;
            case 10:
                inWords = "Ten";
                break;
            case 11:
                inWords = "Elevin";
                break;
            case 12:
                inWords = "Twelve";
                break;
            case 13:
                inWords = "Thirteen";
                break;
            case 14:
                inWords = "Fourteen";
                break;
            case 15:
                inWords = "Fifteen";
                break;
            case 16:
                inWords = "Sixteen";
                break;
            case 17:
                inWords = "Seventeen";
                break;
            case 18:
                inWords = "Eighteen";
                break;
            case 19:
                inWords = "Nineteen";
                break;
        }
        return inWords;
    }

    public static string ReturnOneToNineteen(long value)
    {
        var inWords = "";
        switch (value)
        {
            case 1:
                inWords = "One";
                break;
            case 2:
                inWords = "Two";
                break;
            case 3:
                inWords = "Three";
                break;
            case 4:
                inWords = "Four";
                break;
            case 5:
                inWords = "Five";
                break;
            case 6:
                inWords = "Six";
                break;
            case 7:
                inWords = "Seven";
                break;
            case 8:
                inWords = "Eight";
                break;
            case 9:
                inWords = "Nine";
                break;
        }
        return inWords;
    }

    /*/// <summary>
    ///     this method return financial year
    /// </summary>
    /// <param name="sdate"></param>
    /// <returns></returns>
    public static string GetFinYear(string sdate)
    {
        var finyear = "";
        var s = Convert.ToDateTime(sdate);
        var m = s.Month;
        var y = s.Year;
        if (m > 3)
        {
            finyear = y + "-" + Convert.ToString(y + 1);
        }
        else
        {
            finyear = Convert.ToString(y - 1) + "-" + y;
        }
        return finyear.Trim();
    }*/

    public static Report GetTelerikReport(string reportName)
    {
        Report report = null;
        var assembly = Assembly.Load("TelerikReports");
        foreach (var type in assembly.GetTypes())
        {
            if (type.Name != reportName) continue;
            if (type.FullName != null) report = (Report)assembly.CreateInstance(type.FullName);
        }
        return report;
    }

    public static Report GetTelerikReport(string reportName, int id)
    {
        Report report = null;
        var assembly = Assembly.Load("Corno.Reports");
        foreach (var type in assembly.GetTypes())
        {
            if (type.Name != reportName) continue;

            object[] args = { id };
            if (type.FullName != null)
                report =
                    (Report)
                    assembly.CreateInstance(type.FullName, true, BindingFlags.CreateInstance, null, args,
                        CultureInfo.CurrentCulture, null);
            break;
        }
        return report;
    }

    /*public static string ReplaceAmpersandForHtml(string value)
    {
        return value.Replace("&", "&amp;");
    }

    public static bool HasPaper(string subject)
    {
        if (subject.IndexOf("PAPER", StringComparison.OrdinalIgnoreCase) >= 0)
            return true;
        return false;
    }*/

    /*public static string GetSeatNosRange(string allSeatNos)
    {
        if (string.IsNullOrEmpty(allSeatNos)) return allSeatNos;

        allSeatNos = allSeatNos.Trim(',');
        allSeatNos = allSeatNos.Trim();
        var numbers = allSeatNos.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

        var ranges = numbers
            .Select((n, i) => new { number = n, group = n - i })
            .GroupBy(n => n.group)
            .Select(
                g =>
                    g.Count() >= 3
                        ? g.First().number + "-" + g.Last().number
                        : string.Join(", ", g.Select(x => x.number)))
            .ToList();

        var finalRange = string.Join(", ", ranges);

        return finalRange;
    }*/

    public static object GetMonthName(int value)
    {
        return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(value);
    }

    #endregion
}