namespace RegBlaze.Domain;

public interface ITaskTracker
{
    event EventHandler TaskCompleted;
    public void CompleteTask();
}