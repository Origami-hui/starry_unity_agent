using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryItemPlaneMgr : MonoBehaviour
{
    [Header("配置参数")]
    [SerializeField] private ScrollRect historyScrollView;  // 绑定ScrollView
    [SerializeField] private Transform contentTransform;    // 绑定Content
    [SerializeField] private GameObject historyItemPrefab;   // 历史对话条目预制体

    void OnEnable()
    {
        RefreshHistory(); // 页面显示时刷新内容
    }

    /// <summary>
    /// 刷新历史对话列表
    /// </summary>
    public void RefreshHistory()
    {
        // 清空旧条目
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
            //TestSet(child.gameObject);
        }

        //Debug.Log("dialogueHistory: " + JsonConvert.SerializeObject(HistoryManager.dialogueHistory));
        // 动态生成新条目
        foreach (DialogueData data in HistoryManager.dialogueHistory)
        {
            GameObject newItem = Instantiate(historyItemPrefab, contentTransform);
            // 设置内容
            SetItemContent(newItem, data);
        }

        // 滚动到最底部（最新消息）
        Canvas.ForceUpdateCanvases(); // 强制更新Canvas布局
        historyScrollView.verticalNormalizedPosition = 0;
    }

    /// <summary>
    /// 设置条目内容（头像、文本、时间）
    /// </summary>
    private void SetItemContent(GameObject item, DialogueData data)
    {
        // 获取子组件（根据预制体结构调整路径）
        TMP_Text contentText = item.transform.Find("TextContent").GetComponent<TMP_Text>();
        TMP_Text nameText = item.transform.Find("Name").GetComponent<TMP_Text>();
        GameObject playSound = item.transform.Find("PlayButton").gameObject;

        LayoutElement layoutElement = item.GetComponent<LayoutElement>();

        // 设置文本
        contentText.text = data.content;
        nameText.text = data.role;
        layoutElement.preferredHeight = contentText.preferredHeight;
        playSound.SetActive(data.role.Equals(GlobalConfig.instance.roleName) && data.clip != null);
        if(data.clip != null)
        {
            item.GetComponent<HistoryItemController>().clip = data.clip;
        }
    }

    private void TestSet(GameObject item)
    {
        LayoutElement layoutElement = item.GetComponent<LayoutElement>();
        TMP_Text contentText = item.transform.Find("TextContent").GetComponent<TMP_Text>();
        layoutElement.preferredHeight = contentText.preferredHeight;
        Debug.Log(contentText.preferredHeight);
    }
}
