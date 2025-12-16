using Corno.Data.Helpers;
using System;
using System.IO;
using System.Threading;

namespace Corno.Data.Dtos.Import;

public class ProgressDto
{
    #region -- Constructors --
    public ProgressDto()
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }
    #endregion
    #region -- Properties --
    public double Progress { get; set; } = 0.0;
    public double Total { get; set; }
    public int Minimum { get; set; } = 0;
    public int Maximum { get; set; } = 0;
    public int Step { get; set; } = 1;
    public double Percent { get; set; } = 0.0;
    public string Message { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime? StartTime { get; set; } = DateTime.Now;
    public DateTime? EndTime { get; set; } = DateTime.Now;
    public string FileName { get; set; } = string.Empty;

    public int ImportedCount { get; set; } = 0;
    public int ExistingCount { get; set; } = 0;
    public int IgnoredCount { get; set; } = 0;
    #endregion

    #region -- Data Members --
    private CancellationTokenSource _cancellationTokenSource;
    #endregion

    #region -- Event Handlers --
    public event EventHandler<ProgressDto> OnProgressChanged;
    #endregion

    #region -- Private Methods --
    private void CalculateProgress()
    {
        Percent = Progress * 100 / (Maximum == 0 ? 1 : Maximum);
    }

    private void CreateMessage()
    {
        var timeElapsed = DateTime.Now - (StartTime ?? DateTime.Now);
        var tickTime = timeElapsed.Ticks / (Progress == 0 ? 1 : Progress);
        var remainingProgress = Maximum - Progress;
        var timeRemaining = TimeSpan.FromTicks((tickTime * remainingProgress).ToLong());
        Message = $"File: {Path.GetFileName(FilePath)}, {Environment.NewLine}" +
                      $"Total : {Maximum}, " +
                      $"Completed : {Progress}, " +
                      $"Percent : {Percent}%, " +
                      $"Imported: {IgnoredCount}, " +
                      $"Existing: {ExistingCount}, " +
                      $"Ignored: {IgnoredCount}, {Environment.NewLine}, " +
                      $"Time Elapsed : {timeElapsed:d'.'hh':'mm':'ss}, " +
                      $"Time Remaining : {timeRemaining:d'.'hh':'mm':'ss}";
    }

    private void ReportProgress(int value)
    {
        Progress += value;
        CalculateProgress();
        CreateMessage();

        OnProgressChanged?.Invoke(this, this);
    }
    #endregion

    #region -- Public Methods --

    public virtual void Initialize(string filePath, int minimum, int maximum,
        int step)
    {
        StartTime = DateTime.Now;
        FilePath = filePath;
        Minimum = minimum;
        Maximum = maximum;
        Step = step;

        _cancellationTokenSource = new CancellationTokenSource();
    }
    
    public void ReportMessage(string message)
    {
        Message = message;
        OnProgressChanged?.Invoke(this, this);
    }

    public void Imported(int value)
    {
        ImportedCount += value;
    }

    public void Existing(int value)
    {
        ExistingCount += value;
    }

    public void Ignored(int value)
    {
        IgnoredCount += value;
    }

    public void CancelRequested()
    {
        _cancellationTokenSource.Cancel();
    }
    #endregion
}