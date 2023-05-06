namespace RegBlaze.Core.Services;

public class TaskTracker : ITaskTracker
{
    public event EventHandler? TaskCompleted;

    public void CompleteTask()
    {
        TaskCompleted?.Invoke(this, EventArgs.Empty);
    }
}