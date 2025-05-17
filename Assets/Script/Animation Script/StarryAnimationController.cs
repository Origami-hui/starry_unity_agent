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

    public void PlayAction(string action)
    {
        StartCoroutine(PlayActionIEnumerator(action, 0));
    }

    public void PlayAction(string action, int layer)
    {
        StartCoroutine(PlayActionIEnumerator(action, layer));
    }

    public IEnumerator PlayActionIEnumerator(string action, int layer)
    {
        starryAnimator.CrossFade(action, 0.1f, layer);
        yield return new WaitForSeconds(0.1f);
    }

    public void StartBlendTransition(string emoName, float target)
    {
        int blendShapeIndex = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(emoName);
        if (blendShapeIndex == -1) return;
        StartCoroutine(TransitionBlendShape(skinnedMeshRenderer, blendShapeIndex, target, 1f));
    }

    private IEnumerator TransitionBlendShape(SkinnedMeshRenderer renderer, int index, float target, float duration)
    {
        float startWeight = renderer.GetBlendShapeWeight(index);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 按时间插值权重（线性过渡）
            float currentWeight = Mathf.Lerp(startWeight, target, elapsedTime / duration);
            renderer.SetBlendShapeWeight(index, currentWeight);

            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 确保最终权重为目标值（避免浮点误差）
        Debug.Log("表情变换完毕！" + index);
        renderer.SetBlendShapeWeight(index, target);
    }

}
