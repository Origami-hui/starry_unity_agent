using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;

public class StarryAnimationController : MonoBehaviour
{
    private Animator starryAnimator;
    private SkinnedMeshRenderer skinnedMeshRenderer;

    private void Awake()
    {
        starryAnimator = GetComponent<Animator>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    //播放指定名称动画
    public void PlayAction(string action)
    {
        StartCoroutine(PlayActionIEnumerator(action, 0));
    }

    //重载，可指定动画所在层级
    public void PlayAction(string action, int layer)
    {
        StartCoroutine(PlayActionIEnumerator(action, layer));
    }

    public IEnumerator PlayActionIEnumerator(string action, int layer)
    {
        starryAnimator.CrossFade(action, 0.1f, layer);
        yield return new WaitForSeconds(0.1f);
    }

}
