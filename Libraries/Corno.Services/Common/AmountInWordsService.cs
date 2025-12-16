using System;
using System.Globalization;
using Corno.Services.Common.Interfaces;
using Corno.Services.Corno;

namespace Corno.Services.Common;

public class AmountInWordsService : BaseService, IAmountInWordsService
{
    public string GetAmountInWords(string amount)
    {
        var inWords = "";
        try
        {
            var strArray = amount.Split('.');

            var dValue = Convert.ToDecimal(strArray[0]);

            if (dValue.ToString(CultureInfo.InvariantCulture).Substring(0, 1) == "-")
                return "";
            var iValue = Convert.ToInt64(dValue);
            var sTotal = iValue.ToString();
            if (sTotal.Length >= 10)
                return inWords;

            long l_iIndex = 0;
            while (sTotal.Length > l_iIndex)
            {
                string sResult;
                switch (sTotal.Length)
                {
                    case 1:
                        sResult = ReturnOneToNinteen(iValue);
                        inWords += " " + sResult;
                        iValue = iValue%1;
                        break;
                    case 2:
                        if (iValue > 19)
                            sResult = ReturnZeroToNine(iValue/10);
                        else
                        {
                            sResult = ReturnZeroToNine(iValue);
                            inWords += " " + sResult + " Only";
                            return inWords;
                        }
                        inWords += " " + sResult;
                        iValue = iValue%10;
                        break;
                    case 3:
                        sResult = ReturnOneToNinteen(iValue/100);
                        inWords += " " + sResult + " Hundred";
                        iValue = iValue%100;
                        break;
                    case 4:
                        sResult = ReturnOneToNinteen(iValue/1000);
                        inWords += " " + sResult + " Thousand";
                        iValue = iValue%1000;
                        break;
                    case 5:
                        var iResult = iValue/1000;
                        sResult = ReturnZeroToNine(iResult);
                        if (iResult > 9 && iResult < 20)
                        {
                            inWords += " " + sResult + " Thousand";
                            iValue = iValue%1000;
                        }
                        else if (iResult > 19 && iResult.ToString().Contains("0"))
                        {
                            inWords += " " + sResult + " Thousand";
                            iValue = iValue%1000;
                        }
                        else
                        {
                            inWords += " " + sResult;
                            iValue = iValue%10000;
                        }
                        break;
                    case 6:
                        sResult = ReturnOneToNinteen(iValue/100000);
                        inWords += " " + sResult + " Lacs";
                        iValue = iValue%100000;
                        break;
                    case 7:
                        iResult = iValue/100000;
                        sResult = ReturnZeroToNine(iResult);
                        if (iResult > 9 && iResult < 20)
                        {
                            inWords += " " + sResult + " Lacs";
                            iValue = iValue%100000;
                        }
                        else
                        {
                            inWords += " " + sResult;
                            iValue = iValue%1000000;
                        }
                        break;
                    case 8:
                        sResult = ReturnOneToNinteen(iValue/10000000);
                        inWords += " " + sResult + " Crores";
                        iValue = iValue%10000000;
                        break;
                    case 9:
                        iResult = iValue/10000000;
                        sResult = ReturnZeroToNine(iResult);
                        if (iResult > 9 && iResult < 20)
                        {
                            inWords += " " + sResult + "Crores";
                            iValue = iValue%10000000;
                        }
                        else
                        {
                            inWords += " " + sResult;
                            iValue = iValue%100000000;
                        }
                        break;
                }
                sTotal = iValue.ToString();
                if (sTotal == "0")
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

    private string ReturnZeroToNine(long aIValue)
    {
        var inWords = "";

        if (aIValue > 19)
            aIValue = aIValue/10;
        switch (aIValue)
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

    private string ReturnOneToNinteen(long aIValue)
    {
        var inWords = "";
        switch (aIValue)
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
}