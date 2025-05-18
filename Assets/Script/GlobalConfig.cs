using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 全局配置，所有透出给用户更改的选项都在这里
 */
public class GlobalConfig : MonoBehaviour
{
    public static GlobalConfig instance;

    public List<Message> messagesList; //存储对话信息

    public readonly string roleName = "星莹";

    [Header("人物设定")]
    [Tooltip("可设定人物的基本信息")]
    public string setUp = "请你扮演我的电子女友，名字叫星莹。你活泼开朗喜欢弹吉他。接下来我的每一条提问，你只需以第一人称直接回复要说的话，不要添加任何神态、动作或解释性描述以及表情。";

    [Header("文本生成模型配置")]
    [SerializeField]
    [Tooltip("模型选择，具体key与url请打开代码设置")]
    public ModelEnum modelEnum;

    //对话大模型配置，可以配置多个模型进行切换
    [SerializeField]
    public List<ApiInfo> LLMApiInfoList = new List<ApiInfo>
    {
        new ApiInfo { apiKey = "",
            apiUrl = "https://openrouter.ai/api/v1/chat/completions",
            modelName = "deepseek/deepseek-r1:free" },
        new ApiInfo { apiKey = "",
            apiUrl = "https://api.siliconflow.cn/v1/chat/completions",
            modelName = "deepseek-ai/DeepSeek-V3" },
        //本地模型，添加模型时请确保本地模型处于最后一位！
        new ApiInfo {
            apiUrl = "http://localhost:11434/api/chat",
            modelName = "deepseek-r1:7b" }
    };

    [Header("情绪识别模型配置（百度）")]
    [Tooltip("是否启用情绪识别")]
    public bool emoAnalaysisEnabled = true;

    [DisableIf("emoAnalaysisEnabled", false)]
    [Password]
    public string emoApiKey = "";
    [DisableIf("emoAnalaysisEnabled", false)]
    [Password]
    public string emoSercetKey = "";

    [Header("语音生成模型配置（GPT-SoVITS）")]
    [Tooltip("是否启用语音生成")]
    public bool textToSoundEnabled = true;

    [Tooltip("本地服务地址")]
    [DisableIf("textToSoundEnabled", false)]
    public string serviceUrl = "http://localhost:9880";

    [Tooltip("本地GPT-SoVITS工程根目录")]
    [DisableIf("textToSoundEnabled", false)]
    [Password]
    public string ttsMainFilePath = "";

    [Header("对话管理配置")]
    [Tooltip("历史对话保存文件名，路径为Application.persistentDataPath")]
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