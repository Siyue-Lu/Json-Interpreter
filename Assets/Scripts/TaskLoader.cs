using UnityEngine;
using System.Collections.Generic;

public static class TaskLoader
{
    private static TaskAction MapAction(string act)
    {
        switch (act)
        {
            case "take":
                return TaskAction.Hold;
            case "put":
                return TaskAction.Place;
            default:
                return TaskAction.Idle;
        }
    }

    private static GameObject[] FindObjects(string[] objectNames)
    {
        List<GameObject> objects = new List<GameObject>();
        foreach (string objName in objectNames)
        {
            GameObject obj = GameObject.Find(objName);
            if (obj != null)
            {
                objects.Add(obj);
            }
            else
            {
                Debug.LogWarning("Object " + objName + " not found!");
            }
        }
        return objects.ToArray();
    }

    public static Task[] LoadTasks()
    {
        // Load the JSON file from the Resources folder
        // in a built apk this won't work, need the functionality
        // of loading a json by selection/external link
        TextAsset jsonFile = Resources.Load<TextAsset>("tasks");
        if (jsonFile == null || jsonFile.text == null)
        {
            Debug.Log("you fucked up!");
        }
        
        // Parse the JSON data into a List<TaskData>
        TaskList taskList = JsonUtility.FromJson<TaskList>(jsonFile.text);

        List<Task> tasks = new List<Task>();

        if (taskList != null && taskList.tasks != null)
        {
            foreach (TaskData data in taskList.tasks)
            {
                Task task = new Task();
                task.act = MapAction(data.action);
                task.obj = FindObjects(data.objects);
                task.pos = data.position;
                tasks.Add(task);
            }
        }

        return tasks.ToArray();
    }
}