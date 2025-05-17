using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryItemPlaneMgr : MonoBehaviour
{
    [Header("���ò���")]
    [SerializeField] private ScrollRect historyScrollView;  // ��ScrollView
    [SerializeField] private Transform contentTransform;    // ��Content
    [SerializeField] private GameObject historyItemPrefab;   // ��ʷ�Ի���ĿԤ����

    void OnEnable()
    {
        RefreshHistory(); // ҳ����ʾʱˢ������
    }

    /// <summary>
    /// ˢ����ʷ�Ի��б�
    /// </summary>
    public void RefreshHistory()
    {
        // ��վ���Ŀ
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
            //TestSet(child.gameObject);
        }

        //Debug.Log("dialogueHistory: " + JsonConvert.SerializeObject(HistoryManager.dialogueHistory));
        // ��̬��������Ŀ
        foreach (DialogueData data in HistoryManager.dialogueHistory)
        {
            GameObject newItem = Instantiate(historyItemPrefab, contentTransform);
            // ��������
            SetItemContent(newItem, data);
        }

        // ��������ײ���������Ϣ��
        Canvas.ForceUpdateCanvases(); // ǿ�Ƹ���Canvas����
        historyScrollView.verticalNormalizedPosition = 0;
    }

    /// <summary>
    /// ������Ŀ���ݣ�ͷ���ı���ʱ�䣩
    /// </summary>
    private void SetItemContent(GameObject item, DialogueData data)
    {
        // ��ȡ�����������Ԥ����ṹ����·����
        TMP_Text contentText = item.transform.Find("TextContent").GetComponent<TMP_Text>();
        TMP_Text nameText = item.transform.Find("Name").GetComponent<TMP_Text>();
        GameObject playSound = item.transform.Find("PlayButton").gameObject;

        LayoutElement layoutElement = item.GetComponent<LayoutElement>();

        // �����ı�
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
