using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class LLMAPI : MonoBehaviour
{

    private readonly HttpClient httpClient = new HttpClient();
    private ChatDataManager chatDataManager;
    

    private void Start()
    {
        chatDataManager = GetComponent<ChatDataManager>();
    }

    public async Task<string> GetAIResponse(string userMessage)
    {
        if((int)GlobalConfig.instance.modelEnum == GlobalConfig.instance.LLMApiInfoList.Count)
        {
            return await GetLocalAIResponse(userMessage);
        }

        try
        {
            GlobalConfig.instance.messagesList.Add(new Message { role = "user", content = userMessage });
            var requestData = new
            {
                model = GlobalConfig.instance.LLMApiInfoList[(int)GlobalConfig.instance.modelEnum].modelName,
                messages = GlobalConfig.instance.messagesList
            };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.
                Add("Authorization", $"Bearer {GlobalConfig.instance.LLMApiInfoList[(int)GlobalConfig.instance.modelEnum].apiKey}");

            var response = await httpClient.PostAsync(GlobalConfig.instance.LLMApiInfoList[(int)GlobalConfig.instance.modelEnum].apiUrl, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            //Debug.Log(responseJson);
            var responseData = JsonConvert.DeserializeObject<AIResponse>(responseJson);

            string assistantReply = responseData.choices[0].message.content;
            GlobalConfig.instance.messagesList.Add(new Message { role = "assistant", content = assistantReply });

            //保存历史对话
            chatDataManager.SaveHistoryChat();

            return assistantReply;
        }
        catch (Exception ex)
        {
            Debug.LogError($"LLM API调用失败: {ex.Message} ，messagesList: {JsonConvert.SerializeObject(GlobalConfig.instance.messagesList)}");
            return "抱歉，好像遇到了一些问题。";
        }
    }

    //可调用基于Ollama框架的本地LLM模型
    public async Task<string> GetLocalAIResponse(string userMessage)
    {
        try
        {
            GlobalConfig.instance.messagesList.Add(new Message { role = "user", content = userMessage });
            var requestData = new
            {
                model = GlobalConfig.instance.LLMApiInfoList[-1].modelName,
                messages = GlobalConfig.instance.messagesList
            };

            var json = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(GlobalConfig.instance.LLMApiInfoList[-1].apiUrl, content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            //Debug.Log(responseJson);
            StringReader reader = new StringReader(responseJson);
            string fullResponse = "";
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var lineData = JsonConvert.DeserializeObject<OllamaResponse>(line);
                Debug.Log(lineData);
                if (lineData.message != null)
                {
                    fullResponse += lineData.message.content;
                }

                if (lineData.done) break;
            }

            GlobalConfig.instance.messagesList.Add(new Message { role = "assistant", content = fullResponse });

            //保存历史对话
            chatDataManager.SaveHistoryChat();

            return RemoveThoughtMarkers(fullResponse);
        }
        catch (Exception ex)
        {
            Debug.LogError($"AI API调用失败: {ex.Message} ，messagesList: {JsonConvert.SerializeObject(GlobalConfig.instance.messagesList)}");
            return "抱歉，好像遇到了一些问题。";
        }
    }

    private string RemoveThoughtMarkers(string content)
    {
        // 使用正则表达式匹配<think>和</think>标签及其内容
        string pattern = @"<think>[\s\S]*?</think>" + "\n";
        return Regex.Replace(content, pattern, "", RegexOptions.IgnoreCase);
    }

    public void WashMessagesList(string message)
    {
        int size = GlobalConfig.instance.messagesList.Count;
        if (size > 1 && GlobalConfig.instance.messagesList[size - 2].content.Equals(message))
        {
            GlobalConfig.instance.messagesList.RemoveAt(size - 1);
            GlobalConfig.instance.messagesList.RemoveAt(size - 2);
            Debug.Log("移除重复数据");
        }
    }
}
