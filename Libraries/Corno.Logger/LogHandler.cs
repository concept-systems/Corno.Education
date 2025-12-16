using Corno.Logger.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Windows.Forms;


namespace Corno.Logger;

public class LogHandler
{
    #region -- Data Members --

    private static readonly ILoggingService Logger = LoggingService.GetLoggingService();
    #endregion

    #region -- Properties --
    public static bool ShowMessageBox { get; set; }
    #endregion

    #region -- Methods --

    //private static string GetCallerInfo()
    //{
    //    var stackTrace = new StackTrace();
    //    var methodNames = "";

    //    // Skip the first few frames (0 = GetCallerInfo, 1 = LogError/LogInfo)
    //    for (int i = 2; i <= 4 && i < stackTrace.FrameCount; i++)
    //    {
    //        var method = stackTrace.GetFrame(i).GetMethod();
    //        methodNames += $"{method.DeclaringType.FullName}.{method.Name} -> ";
    //    }

    //    return methodNames.TrimEnd(' ', '-', '>');
    //}
    
    public static void LogError(Exception exception)
    {
        var detailException = GetDetailException(exception);
        /*var callerInfo = GetCallerInfo();
        Logger.Error($"{callerInfo} - {detailException}");*/
        Logger.Error(detailException);
    }

    public static void LogInfo(Exception exception)
    {
        /*var callerInfo = GetCallerInfo();
        Logger.Info($"{callerInfo} - {exception.Message}");*/
        Logger.Info(GetDetailException(exception));
    }

    public static void LogInfo(string message)
    {
        /*var callerInfo = GetCallerInfo();
        Logger.Info($"{callerInfo} - {message}");*/
        Logger.Info(new Exception(message));
    }
    #endregion

    public static Exception GetDetailException(Exception exception)
    {
        var message = exception.Message + "\n";
        if (exception.GetType() == typeof(DbEntityValidationException))
        {
            if (exception is DbEntityValidationException dbEntityValidationException)
            {
                foreach (var eve in dbEntityValidationException.EntityValidationErrors)
                {
                    message =
                        $"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors: \n";
                    foreach (var ve in eve.ValidationErrors)
                        message += $"\n  - Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"";
                }
            }
        }

        if (exception.InnerException?.GetType() == typeof(System.Data.SqlClient.SqlException))
        {
            if (exception.InnerException is System.Data.SqlClient.SqlException sqlException)
            {
                foreach (var error in sqlException.Errors)
                {
                    //message =
                    //    $"Entity of type \"{error.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors: \n";
                    //foreach (var ve in eve.ValidationErrors)
                    //    message += $"\n  - Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"";
                }
            }
        }

        if (exception.InnerException != null)
        {
            message += exception.InnerException.Message;
            exception = exception.InnerException;

            if (exception.InnerException != null)
            {
                message += exception.InnerException.Message;
                exception = exception.InnerException;
            }
        }

        if (exception.Data.Count > 0)
        {
            foreach (DictionaryEntry de in exception.Data)
            {
                if (de.Value is List<Exception> exceptions)
                {
                    message += "\n";
                    foreach (var value in exceptions)
                        message += $"\nError: \"{value.Message}\"";
                }
                else
                {
                    message += $"\n{de.Key} : {de.Value}";
                }
            }
        }

        // Log the message
        LogInfo(message);

        return new Exception(message);
    }
}