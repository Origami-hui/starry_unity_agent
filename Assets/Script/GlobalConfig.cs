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

    //�Ի���ģ�����ã��������ö��ģ�ͽ����л�
    [SerializeField]
    public List<ApiInfo> LLMApiInfoList = new List<ApiInfo>
    {
        new ApiInfo { apiKey = "",
            apiUrl = "https://openrouter.ai/api/v1/chat/completions",
            modelName = "deepseek/deepseek-r1:free" },
        new ApiInfo { apiKey = "",
            apiUrl = "https://api.siliconflow.cn/v1/chat/completions",
            modelName = "deepseek-ai/DeepSeek-V3" },
        //����ģ�ͣ����ģ��ʱ��ȷ������ģ�ʹ������һλ��
        new ApiInfo {
            apiUrl = "http://localhost:11434/api/chat",
            modelName = "deepseek-r1:7b" }
    };

    [Header("����ʶ��ģ�����ã��ٶȣ�")]
    [Tooltip("�Ƿ���������ʶ��")]
    public bool emoAnalaysisEnabled = true;

    [DisableIf("emoAnalaysisEnabled", false)]
    [Password]
    public string emoApiKey = "";
    [DisableIf("emoAnalaysisEnabled", false)]
    [Password]
    public string emoSercetKey = "";

    [Header("��������ģ�����ã�GPT-SoVITS��")]
    [Tooltip("�Ƿ�������������")]
    public bool textToSoundEnabled = true;

    [Tooltip("���ط����ַ")]
    [DisableIf("textToSoundEnabled", false)]
    public string serviceUrl = "http://localhost:9880";

    [Tooltip("����GPT-SoVITS���̸�Ŀ¼")]
    [DisableIf("textToSoundEnabled", false)]
    [Password]
    public string ttsMainFilePath = "";

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

[Serializable]
public class ApiInfo
{
    public string apiUrl;
    [Password]
    public string apiKey;
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

public class PasswordAttribute : PropertyAttribute { }

[Serializable]
public enum ModelEnum
{
    OpenRouter,
    SiliconFlow,
    LocalModel
}