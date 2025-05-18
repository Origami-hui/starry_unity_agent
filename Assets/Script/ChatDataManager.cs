using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class ChatDataManager : MonoBehaviour
{
    private void Start()
    {
        GlobalConfig.instance.messagesList = LoadHistoryChat();
    }

    public List<Message> LoadHistoryChat()
    {
        List<Message> messages = null;
        if (SaveSystem.FileExists(GlobalConfig.instance.historyChatDataFileName))
        {
            messages = SaveSystem.DeserializeObjectFromJson<List<Message>>(GlobalConfig.instance.historyChatDataFileName);
        }

        if (messages == null || messages.Count == 0)
        {
            messages = new List<Message>
            {
                new Message { role = "user", content = GlobalConfig.instance.setUp }
            };
            SaveSystem.SavePersistentDataByJson(GlobalConfig.instance.historyChatDataFileName, messages);
        }

        return messages;
    }

    public void SaveHistoryChat()
    {
        SaveSystem.SavePersistentDataByJson(GlobalConfig.instance.historyChatDataFileName, GlobalConfig.instance.messagesList);
    }

    public void CleanHistoryChat()
    {
        List<Message> messages = new List<Message>
        {
            new Message { role = "user", content = GlobalConfig.instance.setUp }
        };

        if (SaveSystem.FileExists(GlobalConfig.instance.historyChatDataFileName))
        {
            
            SaveSystem.SavePersistentDataByJson(GlobalConfig.instance.historyChatDataFileName, messages);
        }

        GlobalConfig.instance.messagesList = messages;
    }

    // ¿ÉÑ¡£º±à¼­Æ÷²âÊÔ°´Å¥
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Tools/Clean History Chat")]
    private static void TestCleanHistoryChat()
    {
        ChatDataManager chatDataManager = FindObjectOfType<ChatDataManager>();
        chatDataManager?.CleanHistoryChat();
    }
#endif
}
