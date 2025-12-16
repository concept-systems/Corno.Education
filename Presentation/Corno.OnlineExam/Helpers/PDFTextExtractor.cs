using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.Content.Objects;
using PdfSharp.Pdf.IO;
using System;
using System.Linq;
using System.Text;

namespace Corno.OnlineExam.Helpers;

public static class PdfTextExtractor
{
    public static string GetText(string pdfFileName)
    {
        using (var document = PdfReader.Open(pdfFileName, PdfDocumentOpenMode.ReadOnly))
        {
            var result = new StringBuilder();
            foreach (var page in document.Pages.OfType<PdfPage>())
            {
                ExtractText(ContentReader.ReadContent(page), result);
                result.AppendLine();
            }
            return result.ToString();
        }
    }

    public static string GetPageText(PdfPage page)
    {
        var result = new StringBuilder();
        ExtractText(ContentReader.ReadContent(page), result);
        result.AppendLine();
        return result.ToString();
    }

    #region CObject Visitor

    private static void ExtractText(CObject obj, StringBuilder target)
    {
        if (obj is CArray)
            ExtractText((CArray)obj, target);
        else if (obj is CComment)
            ExtractText((CComment)obj, target);
        else if (obj is CInteger)
            ExtractText((CInteger)obj, target);
        else if (obj is CName)
            ExtractText((CName)obj, target);
        else if (obj is CNumber)
            ExtractText((CNumber)obj, target);
        else if (obj is COperator)
            ExtractText((COperator)obj, target);
        else if (obj is CReal)
            ExtractText((CReal)obj, target);
        else if (obj is CSequence)
            ExtractText((CSequence)obj, target);
        else if (obj is CString)
            ExtractText((CString)obj, target);
        else
            throw new NotImplementedException(obj.GetType().AssemblyQualifiedName);
    }

    private static void ExtractText(CArray obj, StringBuilder target)
    {
        foreach (var element in obj)
        {
            ExtractText(element, target);
        }
    }

    private static void ExtractText(CComment obj, StringBuilder target)
    {
        /* nothing */
    }

    private static void ExtractText(CInteger obj, StringBuilder target)
    {
        /* nothing */
    }

    private static void ExtractText(CName obj, StringBuilder target)
    {
        /* nothing */
    }

    private static void ExtractText(CNumber obj, StringBuilder target)
    {
        /* nothing */
    }

    private static void ExtractText(COperator obj, StringBuilder target)
    {
        if (obj.OpCode.OpCodeName == OpCodeName.Tj || obj.OpCode.OpCodeName == OpCodeName.TJ)
        {
            foreach (var element in obj.Operands)
            {
                ExtractText(element, target);
            }
            target.Append(" ");
        }
    }

    private static void ExtractText(CReal obj, StringBuilder target)
    {
        /* nothing */
    }

    private static void ExtractText(CSequence obj, StringBuilder target)
    {
        foreach (var element in obj)
        {
            ExtractText(element, target);
        }
    }

    private static void ExtractText(CString obj, StringBuilder target)
    {
        target.Append(obj.Value);
    }

    #endregion
}

//public class PdfTextExtractor
//{
//    /// BT = Beginning of a text object operator 
//    /// ET = End of a text object operator
//    /// Td move to the start of next line
//    ///  5 Ts = superscript
//    /// -5 Ts = subscript

//    #region Fields

//    #region _numberOfCharsToKeep
//    /// <summary>
//    /// The number of characters to keep, when extracting text.
//    /// </summary>
//    private static int _numberOfCharsToKeep = 15;
//    #endregion

//    #endregion

//    #region ExtractTextFromPDFBytes
//    /// <summary>
//    /// This method processes an uncompressed Adobe (text) object 
//    /// and extracts text.
//    /// </summary>
//    /// <param name="input">uncompressed</param>
//    /// <returns></returns>
//    public string ExtractTextFromPdfBytes(byte[] input)
//    {
//        if (input == null || input.Length == 0) return "";

//        try
//        {
//            string resultString = "";

//            // Flag showing if we are we currently inside a text object
//            bool inTextObject = false;

//            // Flag showing if the next character is literal 
//            // e.g. '\\' to get a '\' character or '\(' to get '('
//            bool nextLiteral = false;

//            // () Bracket nesting level. Text appears inside ()
//            int bracketDepth = 0;

//            // Keep previous chars to get extract numbers etc.:
//            char[] previousCharacters = new char[_numberOfCharsToKeep];
//            for (int j = 0; j < _numberOfCharsToKeep; j++) previousCharacters[j] = ' ';


//            for (int i = 0; i < input.Length; i++)
//            {
//                char c = (char)i;

//                if (inTextObject)
//                {
//                    // Position the text
//                    if (bracketDepth == 0)
//                    {
//                        if (CheckToken(new[] { "TD", "Td" }, previousCharacters))
//                        {
//                            resultString += "\n\r";
//                        }
//                        else
//                        {
//                            if (CheckToken(new[] { "'", "T*", "\"" }, previousCharacters))
//                            {
//                                resultString += "\n";
//                            }
//                            else
//                            {
//                                if (CheckToken(new[] { "Tj" }, previousCharacters))
//                                {
//                                    resultString += " ";
//                                }
//                            }
//                        }
//                    }

//                    // End of a text object, also go to a new line.
//                    if (bracketDepth == 0 &&
//                        CheckToken(new[] { "ET" }, previousCharacters))
//                    {

//                        inTextObject = false;
//                        resultString += " ";
//                    }
//                    else
//                    {
//                        // Start outputting text
//                        if ((c == '(') && (bracketDepth == 0) && (!nextLiteral))
//                        {
//                            bracketDepth = 1;
//                        }
//                        else
//                        {
//                            // Stop outputting text
//                            if ((c == ')') && (bracketDepth == 1) && (!nextLiteral))
//                            {
//                                bracketDepth = 0;
//                            }
//                            else
//                            {
//                                // Just a normal text character:
//                                if (bracketDepth == 1)
//                                {
//                                    // Only print out next character no matter what. 
//                                    // Do not interpret.
//                                    if (c == '\\' && !nextLiteral)
//                                    {
//                                        nextLiteral = true;
//                                    }
//                                    else
//                                    {
//                                        if (((c >= ' ') && (c <= '~')) ||
//                                            ((c >= 128) && (c < 255)))
//                                        {
//                                            resultString += c.ToString();
//                                        }

//                                        nextLiteral = false;
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }

//                // Store the recent characters for 
//                // when we have to go back for a checking
//                for (int j = 0; j < _numberOfCharsToKeep - 1; j++)
//                {
//                    previousCharacters[j] = previousCharacters[j + 1];
//                }
//                previousCharacters[_numberOfCharsToKeep - 1] = c;

//                // Start of a text object
//                if (!inTextObject && CheckToken(new[] { "BT" }, previousCharacters))
//                {
//                    inTextObject = true;
//                }
//            }
//            return resultString;
//        }
//        catch
//        {
//            return "";
//        }
//    }
//    #endregion

//    #region CheckToken

//    /// <summary>
//    /// Check if a certain 2 character token just came along (e.g. BT)
//    /// </summary>
//    /// <param name="tokens"></param>
//    /// <param name="recent">the recent character array</param>
//    /// <returns></returns>
//    private bool CheckToken(string[] tokens, char[] recent)
//    {
//        foreach (var token in tokens)
//        {
//            if (token.Length > 1)
//            {
//                if ((recent[_numberOfCharsToKeep - 3] == token[0]) &&
//                    (recent[_numberOfCharsToKeep - 2] == token[1]) &&
//                    ((recent[_numberOfCharsToKeep - 1] == ' ') ||
//                    (recent[_numberOfCharsToKeep - 1] == 0x0d) ||
//                    (recent[_numberOfCharsToKeep - 1] == 0x0a)) &&
//                    ((recent[_numberOfCharsToKeep - 4] == ' ') ||
//                    (recent[_numberOfCharsToKeep - 4] == 0x0d) ||
//                    (recent[_numberOfCharsToKeep - 4] == 0x0a))
//                    )
//                {
//                    return true;
//                }
//            }
//            else
//            {
//                return false;
//            }

//        }
//        return false;
//    }
//    #endregion
//}