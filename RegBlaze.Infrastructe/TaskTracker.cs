using RegBlaze.Domain;

namespace RegBlaze.Infrastructe;

public class TaskTracker : ITaskTracker
{
    public event EventHandler? TaskCompleted;

    public void CompleteTask()
    {
        TaskCompleted?.Invoke(this, EventArgs.Empty);
    }
}