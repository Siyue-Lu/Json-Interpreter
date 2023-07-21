using UnityEngine;
using TMPro;

public class VRConsole : MonoBehaviour
{
    public TextMeshProUGUI debugText;

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Update the debugText with the new log message
        debugText.text += logString + "\n";
    }
}