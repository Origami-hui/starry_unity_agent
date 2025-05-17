using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{
    public GameObject historyPlane;
    public GameObject dialogUI;

    [SerializeField]
    public static List<DialogueData> dialogueHistory = new List<DialogueData>();

    private bool historyActive = false;

    private void Update()
    {
        if (Input.mouseScrollDelta.y > 0.2f && !historyActive)
        {
            OpenHistoryPlane();
        }

        if(Input.GetKeyDown(KeyCode.Escape) && historyActive)
        {
            ExitButton();
        }
    }

    public void OpenHistoryPlane()
    {
        historyPlane.SetActive(true);
        dialogUI.SetActive(false);
        historyActive = true;
    }

    public void ExitButton()
    {
        historyPlane.SetActive(false);
        dialogUI.SetActive(true);
        historyActive = false;
    }

    public static void AddDialogue(string role, string content, AudioClip clip)
    {
        DialogueData newDialogue = new DialogueData
        {
            role = role,
            content = content,
            clip = clip
        };
        dialogueHistory.Add(newDialogue);
    }

    public static void AddDialogue(string role, string content)
    {
        AddDialogue(role, content, null);
    }

    public void CleanDialogue()
    {
        dialogueHistory.Clear();
    }

}

[System.Serializable]
public class DialogueData
{
    public string role;       // 角色（"Player"或"AI"）
    public string content;    // 对话内容
    public AudioClip clip;    // 对话语音
}
