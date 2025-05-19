using System.Collections;
using TMPro;
using UnityEngine;
using static LLMAPI;

public class DialogManager : MonoBehaviour
{
    public TMP_Text responseText;
    public TMP_InputField inputField;
    public LLMAPI llmAPI;
    public TTSAPI soVITSAPI;
    public EmotioManagerAPI emotioManagerAPI;
    public StarryController starryController;

    private string message;

    void Start()
    {
        // 注册回车键发送消息
        inputField.onEndEdit.AddListener(OnEndEdit);

        responseText.text = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RetrySendMessage();
        }
    }


    void OnEndEdit(string text)
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            SendMyMessage(inputField.text.Trim());
        }
    }

    async void SendMyMessage(string msg)
    {

        if (string.IsNullOrEmpty(msg))
        {
            return;
        }
        message = msg;

        HistoryManager.AddDialogue("你", msg);

        inputField.interactable = false;

        responseText.text = "思考中...";
        starryController.SetThinkingState();

        // 聚焦输入框
        inputField.ActivateInputField();

        string response = await llmAPI.GetAIResponse(message);

        //进行情感分析
        if (GlobalConfig.instance.emoAnalaysisEnabled)
        {
            StartCoroutine(emotioManagerAPI.AnalyzeSentimentCoroutine(response));
        }

        if (!string.IsNullOrEmpty(response))
        {
            //加载语音
            Coroutine coroutine1 = StartCoroutine(soVITSAPI.SendTTSRequest(response));
            
            StartCoroutine(WaitAndStartType(coroutine1, response));
        }
    }

    private IEnumerator WaitAndStartType(Coroutine previousCoroutine, string response)
    {
        yield return previousCoroutine; // 等待前一个协程完成
        StartCoroutine(AnimateTextTyping(response)); // 启动第二个协程

        starryController.SetTalkingState(emotioManagerAPI.emoIndex);
    }

    public void RetrySendMessage()
    {
        Debug.Log("再次发送！");
        //清洗重复数据，防止用户多次重发
        llmAPI.WashMessagesList(message);
        SendMyMessage(message);
    }

    IEnumerator AnimateTextTyping(string fullText)
    {
        responseText.text = "";
        foreach (char c in fullText)
        {
            responseText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        // 清空输入框
        inputField.text = string.Empty;
        inputField.interactable = true;

        if (!GlobalConfig.instance.textToSoundEnabled)
        {
            HistoryManager.AddDialogue(GlobalConfig.instance.roleName, fullText);
            starryController.SetIdleState();
        }
    }

}
