namespace RegBlaze.Core.Services;

public interface ITaskTracker
{
    event EventHandler TaskCompleted;
    public void CompleteTask();
}