using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public interface IEventInfo
{
}

public class EventInfo<T> : IEventInfo
{
    //����ί�У�����һ���βεķ���
    public UnityAction<T> actions;

    //�Ǽ�action�����������Լ���ί�п�֮�У�����ί�е�ʱ��һ�������������еķ���
    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}
public class EventInfo : IEventInfo
{
    public UnityAction actions;
    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}

public class EventCenter : MonoBehaviour
{
    private static Dictionary<string, IEventInfo> eventDic
        = new Dictionary<string, IEventInfo>();

    public static void AddEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }

    public static void AddEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }

    public static void EventTrigger<T>(string name, T info)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions?.Invoke(info);
        }
    }
    public static void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions?.Invoke();
        }
    }
    public static void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            //�Ƴ����ί��
            (eventDic[name] as EventInfo<T>).actions -= action;
        }
    }
    public static void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            //�Ƴ����ί��
            (eventDic[name] as EventInfo).actions -= action;
        }
    }

    public static void ClearEventListener(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            eventDic.Remove(name);
        }
    }

    public static void Clear()
    {
        eventDic.Clear();
    }

}