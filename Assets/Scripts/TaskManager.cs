using System.Collections;
using UnityEngine;
using VRBuilder.XRInteraction.Properties;
using System.Linq;
using VRBuilder.XRInteraction;
using VRBuilder.Core.SceneObjects;


// please be noted that snap task related functions are not implemented yet
public class TaskManager : MonoBehaviour
{
    private Task[] tasks;
    private Task currentTask;
    private int currentTaskIndex = 0;

    private static TaskManager manager;

    public static TaskManager Manager
    {
        get
        {
            if (manager == null)
            {
                manager = FindObjectOfType<TaskManager>();
            }
            return manager;
        }
    }

    // Event to notify task completion
    public event System.Action<Task> OnTaskCompleted;

    private void Start()
    {
        // Ensure there's only one instance of TaskManager in the scene
        if (manager != null && manager != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            manager = this;
            DontDestroyOnLoad(this.gameObject);
            tasks = TaskLoader.LoadTasks();
            StartTask();
        }
    }

    private bool AreObjectsPlaced(GameObject[] objects)
    {
        // Implement your logic here to check if the objects are placed correctly
        // Return true if the objects are placed correctly, otherwise false
        return false;
    }

    private void StartTask()
    {
        if (currentTaskIndex >= tasks.Length)
        {
            Debug.Log("All tasks completed!");
            return;
        }

        currentTask = tasks[currentTaskIndex];
        StartCoroutine(PerformAction(currentTask.act, currentTask.obj, currentTask.pos));
    }

    // it is harder to add the right collider for an object in runtime
    // so we're assuming the handed over models already have colliders with them
    private IEnumerator PerformAction(TaskAction action, GameObject[] objects, Vector3 position)
    {
        if (action == TaskAction.Hold)
        {
            foreach (GameObject obj in objects)
            {
                // Add needed components if they don't exist
                // in editor, adding just GrabbableProperty adds all needed components
                // not tested in built app, so adding components manually here just in case

                Rigidbody body = obj.GetComponent<Rigidbody>();

                if (!body)
                {
                    obj.AddComponent<Rigidbody>();
                    body = obj.GetComponent<Rigidbody>();
                }
                if (!body.isKinematic)
                {
                    body.isKinematic = true;
                }
                if (body.useGravity)
                {
                    body.useGravity = false;
                }
                
                if (!obj.GetComponent<InteractableObject>())
                {
                    obj.AddComponent<InteractableObject>();
                }

                if (!obj.GetComponent<ProcessSceneObject>())
                {
                    obj.AddComponent<ProcessSceneObject>();
                }

                if (!obj.GetComponent<TouchableProperty>())
                {
                    obj.AddComponent<TouchableProperty>();
                }

                if (!obj.GetComponent<GrabbableProperty>())
                {
                    obj.AddComponent<GrabbableProperty>();
                }

                // if the object really doesn't have a collider, add a mesh collider
                // highly not recommended tho, c'mon asset team
                if (!obj.GetComponent<Collider>())
                {
                    obj.AddComponent<MeshCollider>();
                }
            }

            Debug.Log("Take " + objects[0]);

            // Wait until the required objects are being held
            yield return new WaitUntil(() => objects.All(obj => obj.GetComponent<GrabbableProperty>() && obj.GetComponent<GrabbableProperty>().IsGrabbed));
        }
        else if (action == TaskAction.Place)
        {
            foreach (GameObject obj in objects)
            {
                // Add 'snappable' component if it doesn't exist
                if (!obj.GetComponent<SnappableProperty>())
                {
                    obj.AddComponent<SnappableProperty>();
                }
            }

            // Wait until the objects are placed correctly or one of the objects is released
            yield return new WaitUntil(() => AreObjectsPlaced(objects) || objects.Any(obj => obj.GetComponent<GrabbableProperty>() && !obj.GetComponent<GrabbableProperty>().IsGrabbed));

            // Check if the objects are placed correctly
            if (!AreObjectsPlaced(objects))
            {
                // Objects were released before being placed correctly
                Debug.Log("Put " + objects[0] + " at " + position);
                yield break;
            }
        }

        // Notify task completion using the event
        OnTaskCompleted?.Invoke(currentTask);

        // Move to the next task
        currentTaskIndex++;
        StartTask();
    }
}