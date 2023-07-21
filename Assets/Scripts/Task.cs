using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskData
{
    public string action;
    public string[] objects;
    public Vector3 position;
}

[System.Serializable]
public class TaskList
{
    public List<TaskData> tasks;
}

public enum TaskAction
{
    Hold,
    Place,
    Idle
}

public class Task
{
    public TaskAction act;
    public GameObject[] obj;
    public Vector3 pos;
}