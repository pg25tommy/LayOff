using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] private Canvas consoleCanvas; // Reference to the canvas
    public TextMeshProUGUI debugText;
    private List<LogMessage> logMessages = new List<LogMessage>();
    private int maxLogCount = 3;
    private float logDuration = 5.0f; // Duration in seconds to keep messages

    void Awake()
    {
        // Register the HandleLog method to listen to log messages
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy()
    {
        // Unregister the HandleLog method when the object is destroyed
        Application.logMessageReceived -= HandleLog;
    }

    void Update()
    {
        // Toggle the console canvas on or off with the Tab key
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            consoleCanvas.enabled = !consoleCanvas.enabled;
        }

        // Remove messages that are older than the log duration
        logMessages.RemoveAll(log => Time.time - log.timestamp > logDuration);

        // Update the debug text with the remaining log messages
        UpdateDebugText();
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Ignore warnings and errors
        if (type != LogType.Log) return;

        // Add the new log message with a timestamp
        logMessages.Add(new LogMessage { message = logString, timestamp = Time.time });

        // Remove the oldest log message if we exceed the max log count
        if (logMessages.Count > maxLogCount)
        {
            logMessages.RemoveAt(0);
        }

        UpdateDebugText(); // Update text immediately after adding a new log
    }

    void UpdateDebugText()
    {
        if (debugText != null)
        {
            debugText.text = string.Join("\n", logMessages.ConvertAll(log => log.message).ToArray());
        }
    }

    private class LogMessage
    {
        public string message;
        public float timestamp;
    }
}
