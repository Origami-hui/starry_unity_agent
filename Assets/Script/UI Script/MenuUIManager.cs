using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    public HistoryManager historyManager;
    public ChatDataManager chatDataManager;

    private DialogManager dialogManager;

    private void Start()
    {
        dialogManager = GetComponent<DialogManager>();
    }

    public void ShowHistory()
    {
        historyManager.OpenHistoryPlane();
    }

    public void Retry()
    {
        dialogManager.RetrySendMessage();
    }

    public void CleanHistoryChat()
    {
        chatDataManager.CleanHistoryChat();
        historyManager.CleanDialogue();
        dialogManager.responseText.text = "��ʷ�Ի�ȫ��ɾ������Ҫ������...";
        //TODO ȷ�Ϸ���
    }
}
