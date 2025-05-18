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
        // 获取Token并分析情感（异步任务转协程）
        var task = AnalyzeSentimentAsync(text);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError("任务异常：" + task.Exception?.Message);
        }

        //处理结果
        emoIndex = task.Result;
        string emotion = emoIndex switch
        {
            2 => "正向",
            0 => "负向",
            _ => "中性"
        };
        Debug.Log($"最终情绪：{emotion}, emoIndex: {emoIndex}");
    }

    //获取Token并分析情感
    private async Task<int> AnalyzeSentimentAsync(string text)
    {
        try
        {
            // 步骤2：调用情感分析接口
            var response = await SendSentimentRequestAsync(text);
            //Debug.Log("情感分析结果：" + response);

            // 解析结果（示例：提取情感标签）
            var sentimentResult = JsonConvert.DeserializeObject<BaiduSentimentResponse>(response);
            int emoIndex = sentimentResult.items[0].sentiment;
            
            return emoIndex;
        }
        catch (Exception ex)
        {
            Debug.LogError("分析过程异常：" + ex.Message);
            return -1;
        }
    }

    private IEnumerator SetAccessToken()
    {
        // 步骤1：获取Access Token
        var task = GetAccessTokenAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        accessToken = task.Result;

        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("获取Token失败");
            yield return GetAccessTokenAsync();
        }
    }

    //获取百度AI Access Token
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
            response.EnsureSuccessStatusCode(); // 非200状态码抛异常

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TokenResponse>(responseBody);
            return result.access_token.ToString();
        }
    }

    //异步发送情感分析请求
    private async Task<string> SendSentimentRequestAsync(string _text)
    {
        var url = $"https://aip.baidubce.com/rpc/2.0/nlp/v1/sentiment_classify?charset=UTF-8&access_token={accessToken}";
        var requestBody = new { text = _text };
        var json = JsonConvert.SerializeObject(requestBody);

        using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
        {
            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode(); // 非200状态码抛异常

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
        public int sentiment;   // 0:负向，1:中性，2:正向
        public float confidence;// 置信度
        public float positive_prob; // 正向概率
        public float negative_prob; // 负向概率
    }

    [Serializable]
    private class TokenResponse
    {
        public string access_token;
        public int expires_in;
    }
}
