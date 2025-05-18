using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EmotioManagerAPI : MonoBehaviour
{
    public int emoIndex = 0;

    private readonly HttpClient httpClient = new HttpClient();
    private string accessToken;

    void Start()
    {
        if (GlobalConfig.instance.emoAnalaysisEnabled)
        {
            StartCoroutine(SetAccessToken());
        }

    }

    public IEnumerator AnalyzeSentimentCoroutine(string text)
    {
        // ��ȡToken��������У��첽����תЭ�̣�
        var task = AnalyzeSentimentAsync(text);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError("�����쳣��" + task.Exception?.Message);
        }

        //������
        emoIndex = task.Result;
        string emotion = emoIndex switch
        {
            2 => "����",
            0 => "����",
            _ => "����"
        };
        Debug.Log($"����������{emotion}, emoIndex: {emoIndex}");
    }

    //��ȡToken���������
    private async Task<int> AnalyzeSentimentAsync(string text)
    {
        try
        {
            // ����2��������з����ӿ�
            var response = await SendSentimentRequestAsync(text);
            //Debug.Log("��з��������" + response);

            // ���������ʾ������ȡ��б�ǩ��
            var sentimentResult = JsonConvert.DeserializeObject<BaiduSentimentResponse>(response);
            int emoIndex = sentimentResult.items[0].sentiment;
            
            return emoIndex;
        }
        catch (Exception ex)
        {
            Debug.LogError("���������쳣��" + ex.Message);
            return -1;
        }
    }

    private IEnumerator SetAccessToken()
    {
        // ����1����ȡAccess Token
        var task = GetAccessTokenAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        accessToken = task.Result;

        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("��ȡTokenʧ��");
            yield return GetAccessTokenAsync();
        }
    }

    //��ȡ�ٶ�AI Access Token
    private async Task<string> GetAccessTokenAsync()
    {
        var url = "https://aip.baidubce.com/oauth/2.0/token";
        var parameters = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", GlobalConfig.instance.emoApiKey },
            { "client_secret", GlobalConfig.instance.emoSercetKey }
        };

        using (var content = new FormUrlEncodedContent(parameters))
        {
            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode(); // ��200״̬�����쳣

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TokenResponse>(responseBody);
            return result.access_token.ToString();
        }
    }

    //�첽������з�������
    private async Task<string> SendSentimentRequestAsync(string _text)
    {
        var url = $"https://aip.baidubce.com/rpc/2.0/nlp/v1/sentiment_classify?charset=UTF-8&access_token={accessToken}";
        var requestBody = new { text = _text };
        var json = JsonConvert.SerializeObject(requestBody);

        using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
        {
            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode(); // ��200״̬�����쳣

            return await response.Content.ReadAsStringAsync();
        }
    }

    [Serializable]
    private class BaiduSentimentResponse
    {
        public SentimentItem[] items;
    }

    [Serializable]
    private class SentimentItem
    {
        public int sentiment;   // 0:����1:���ԣ�2:����
        public float confidence;// ���Ŷ�
        public float positive_prob; // �������
        public float negative_prob; // �������
    }

    [Serializable]
    private class TokenResponse
    {
        public string access_token;
        public int expires_in;
    }
}
