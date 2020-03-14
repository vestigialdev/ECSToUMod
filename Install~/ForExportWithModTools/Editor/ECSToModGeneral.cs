using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ECSToModGeneral {

    static string PrintMessagesKey => $"ECSToUModEditor_DebugMode_{Application.productName}";


    [MenuItem("Tools/ECSToUMod/Toggle debug messages")]
    public static void ToggleMessages() {
        EditorPrefs.SetBool(PrintMessagesKey, !EditorPrefs.GetBool(PrintMessagesKey, true));
        Debug.Log($"ECSToUMod print debug messages: {EditorPrefs.GetBool(PrintMessagesKey)}");
    }

    public static void Print(string message, LogType logType = LogType.Log) {

        switch(logType) {

            case LogType.Error:
                Debug.LogError(message);
                break;

            case LogType.Assert:
                Debug.LogAssertion(message);
                break;

            case LogType.Warning:
                Debug.LogWarning(message);
                break;

            case LogType.Log:
                if(EditorPrefs.GetBool(PrintMessagesKey)) {
                    Debug.Log(message);
                }
                break;

            default:
                break;
        }
    }
}
