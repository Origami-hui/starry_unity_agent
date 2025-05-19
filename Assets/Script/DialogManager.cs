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
        // ע��س���������Ϣ
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

        HistoryManager.AddDialogue("��", msg);

        inputField.interactable = false;

        responseText.text = "˼����...";
        starryController.SetThinkingState();

        // �۽������
        inputField.ActivateInputField();

        string response = await llmAPI.GetAIResponse(message);

        //������з���
        if (GlobalConfig.instance.emoAnalaysisEnabled)
        {
            StartCoroutine(emotioManagerAPI.AnalyzeSentimentCoroutine(response));
        }

        if (!string.IsNullOrEmpty(response))
        {
            //��������
            Coroutine coroutine1 = StartCoroutine(soVITSAPI.SendTTSRequest(response));
            
            StartCoroutine(WaitAndStartType(coroutine1, response));
        }
    }

    private IEnumerator WaitAndStartType(Coroutine previousCoroutine, string response)
    {
        yield return previousCoroutine; // �ȴ�ǰһ��Э�����
        StartCoroutine(AnimateTextTyping(response)); // �����ڶ���Э��

        starryController.SetTalkingState(emotioManagerAPI.emoIndex);
    }

    public void RetrySendMessage()
    {
        Debug.Log("�ٴη��ͣ�");
        //��ϴ�ظ����ݣ���ֹ�û�����ط�
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

        // ��������
        inputField.text = string.Empty;
        inputField.interactable = true;

        if (!GlobalConfig.instance.textToSoundEnabled)
        {
            HistoryManager.AddDialogue(GlobalConfig.instance.roleName, fullText);
            starryController.SetIdleState();
        }
    }

}
