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

    public List<ApiInfo> LLMApiInfoList = new List<ApiInfo>
    {
        new ApiInfo { apiKey = "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",
            apiUrl = "https://openrouter.ai/api/v1/chat/completions",
            modelName = "deepseek/deepseek-r1:free" },
        new ApiInfo { apiKey = "sk-snmepepjmyvoajxbdxbwfgzlnqqcaysawibsomjvtrddzlpn",
            apiUrl = "https://api.siliconflow.cn/v1/chat/completions",
            modelName = "deepseek-ai/DeepSeek-V3" }
    };

    [Header("情绪识别模型配置（百度）")]
    [Tooltip("是否启用情绪识别")]
    public bool emoAnalaysisEnabled = true;

    [DisableIf("emoAnalaysisEnabled", false)]
    public string emoApiKey = "YAx5yQVSwJXaW8wbFgOvrFkz";
    [DisableIf("emoAnalaysisEnabled", false)]
    public string emoSercetKey = "SGb1ASyTXuP1iiJeqNyYuOlvBSX3LmWZ";

    [Header("语音生成模型配置（GPT-SoVITS）")]
    [Tooltip("是否启用语音生成")]
    public bool textToSoundEnabled = true;

    [Tooltip("本地服务地址")]
    [DisableIf("textToSoundEnabled", false)]
    public string serviceUrl = "http://localhost:9880";

    [Tooltip("本地GPT-SoVITS工程根目录")]
    [DisableIf("textToSoundEnabled", false)]
    public string ttsMainFilePath = "E:\\projects\\ai_voice\\GPT-SoVITS-v2-240821\\GPT-SoVITS-v2-240821";

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