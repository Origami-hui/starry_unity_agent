using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * ȫ�����ã�����͸�����û����ĵ�ѡ�������
 */
public class GlobalConfig : MonoBehaviour
{
    public static GlobalConfig instance;

    public List<Message> messagesList; //�洢�Ի���Ϣ

    public readonly string roleName = "��Ө";

    [Header("�����趨")]
    [Tooltip("���趨����Ļ�����Ϣ")]
    public string setUp = "��������ҵĵ���Ů�ѣ����ֽ���Ө������ÿ���ϲ�����������������ҵ�ÿһ�����ʣ���ֻ���Ե�һ�˳�ֱ�ӻظ�Ҫ˵�Ļ�����Ҫ����κ���̬������������������Լ����顣";

    [Header("�ı�����ģ������")]
    [SerializeField]
    [Tooltip("ģ��ѡ�񣬾���key��url��򿪴�������")]
    public ModelEnum modelEnum;

    public List<ApiInfo> LLMApiInfoList = new List<ApiInfo>
    {
        new ApiInfo { apiKey = "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",
            apiUrl = "https://openrouter.ai/api/v1/chat/completions",
            modelName = "deepseek/deepseek-r1:free" },
        new ApiInfo { apiKey = "sk-snmepepjmyvoajxbdxbwfgzlnqqcaysawibsomjvtrddzlpn",
            apiUrl = "https://api.siliconflow.cn/v1/chat/completions",
            modelName = "deepseek-ai/DeepSeek-V3" }
    };

    [Header("����ʶ��ģ�����ã��ٶȣ�")]
    [Tooltip("�Ƿ���������ʶ��")]
    public bool emoAnalaysisEnabled = true;

    [DisableIf("emoAnalaysisEnabled", false)]
    public string emoApiKey = "YAx5yQVSwJXaW8wbFgOvrFkz";
    [DisableIf("emoAnalaysisEnabled", false)]
    public string emoSercetKey = "SGb1ASyTXuP1iiJeqNyYuOlvBSX3LmWZ";

    [Header("��������ģ�����ã�GPT-SoVITS��")]
    [Tooltip("�Ƿ�������������")]
    public bool textToSoundEnabled = true;

    [Tooltip("���ط����ַ")]
    [DisableIf("textToSoundEnabled", false)]
    public string serviceUrl = "http://localhost:9880";

    [Tooltip("����GPT-SoVITS���̸�Ŀ¼")]
    [DisableIf("textToSoundEnabled", false)]
    public string ttsMainFilePath = "E:\\projects\\ai_voice\\GPT-SoVITS-v2-240821\\GPT-SoVITS-v2-240821";

    [Header("�Ի���������")]
    [Tooltip("��ʷ�Ի������ļ�����·��ΪApplication.persistentDataPath")]
    public readonly string HISTORY_CHAT_DATA_URL = "starry_chat_history.txt";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


public class ApiInfo
{
    public string apiKey;
    public string apiUrl;
    public string modelName;
}

[Serializable]
public class Message
{
    public string role;
    public string content;
}

[Serializable]
public class AIResponse
{
    public Choice[] choices;
}

[Serializable]
public class Choice
{
    public Message message;
}

[Serializable]
public class OllamaResponse
{
    public string model;
    public string created_at;
    public Message message;
    public bool done;
}

[Serializable]
public enum ModelEnum
{
    OpenRouter,
    SiliconFlow,
    LocalModel
}