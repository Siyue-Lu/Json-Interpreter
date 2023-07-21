using UnityEngine;

public class TaskSubscriber : MonoBehaviour
{
    private TaskManager taskManager;

    private void OnEnable()
    {
        // Get the TaskManager instance when the script is enabled
        taskManager = TaskManager.Manager;

        if (taskManager != null)
        {
            // Subscribe to the event
            taskManager.OnTaskCompleted += HandleTaskCompleted;
        }
        else
        {
            Debug.LogWarning("TaskManager instance not found. Ensure that TaskManager is present in the scene.");
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from the event when the script is disabled
        if (taskManager != null)
        {
            taskManager.OnTaskCompleted -= HandleTaskCompleted;
        }
    }

    private void HandleTaskCompleted(Task completedTask)
    {
        // Perform actions when a task is completed
        Debug.Log("Task completed: " + completedTask.act);
    }
}