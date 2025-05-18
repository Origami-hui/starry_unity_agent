using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StarryController;

public class StarryController : MonoBehaviour
{
    public StarryState starryState;
    public AudioSource audioSource;

    public enum StarryState
    {
        Idle = 0,
        Talking = 1,
        Thinking = 2,
        Toughing = 3 //处于交互状态（触摸）
    }

    public enum StarryEmoAnimation
    {
        Shout,
        TalkingNoEmo,
        Laugh
    }

    private Transform starryHead;
    private Animator starryAnimator;
    private StarryAnimationController starryAnimationController;
    private float angleX;
    private float angleY;
    private Vector2 horizontalAngleLimit = new Vector2(-50f, 50f);//水平方向上的角度限制
    private Vector2 verticalAngleLimit = new Vector2(-40f, 40f);//垂直方向上的角度限制
    private float lerpSpeed = 5f;

    [SerializeField]
    private float currentIdleTime = 0f;      // 当前待机时间
    private Coroutine idleCoroutine;         // 待机计时协程引用

    private void Awake()
    {
        starryAnimator = GetComponent<Animator>();
        starryAnimationController = GetComponent<StarryAnimationController>();
        starryHead = starryAnimator.GetBoneTransform(HumanBodyBones.Head);
    }

    private void Start()
    {
        starryState = StarryState.Idle;
        EventCenter.AddEventListener("SetIdleState", SetIdleState);

        // 开始待机计时
        StartIdleTimer();
    }

    void Update()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            DetectMeshCollision();
        }
    }

    void LateUpdate()
    {
        //闲置时看向鼠标
        if(starryState == StarryState.Idle)
        {
            LookAtPosition(GetMouseWorldPosition());
        }
    }

    public void SetThinkingState()
    {
        //EventCenter.EventTrigger("PlayAction", "Thinking");
        starryAnimationController.PlayAction("Thinking");
        starryState = StarryState.Thinking;
    }

    //根据情绪播放不同的动画
    public void SetTalkingState(int emoIndex)
    {
        StarryEmoAnimation emoAnimation = (StarryEmoAnimation)emoIndex;
        starryAnimationController.PlayAction(emoAnimation.ToString());
        starryAnimationController.PlayAction(emoAnimation.ToString(), 2);
        //starryAnimationController.StartBlendTransition("Fcl_ALL_Angry", 100);
        starryState = StarryState.Talking;

        StartCoroutine(SetLayerWeightUpdate(1, 0.5f, 1));
    }

    public void SetIdleState()
    {
        starryAnimationController.PlayAction("Idle");
        starryAnimationController.PlayAction("IdleEmo", 2);
        starryState = StarryState.Idle;

        StartCoroutine(SetLayerWeightUpdate(1, 0.5f, 0));
    }

    //看向某点
    public void LookAtPosition(Vector3 position)
    {
        //头部位置
        //Vector3 headPosition = transform.position + transform.up * headHeight;
        //朝向
        Quaternion lookRotation = Quaternion.LookRotation(position - starryHead.position);
        Vector3 eulerAngles = lookRotation.eulerAngles - starryHead.rotation.eulerAngles;
        float x = NormalizeAngle(eulerAngles.x);
        float y = NormalizeAngle(eulerAngles.y);
        x = Mathf.Clamp(x, verticalAngleLimit.x, verticalAngleLimit.y);
        y = Mathf.Clamp(y, horizontalAngleLimit.x, horizontalAngleLimit.y);
        angleX = Mathf.Clamp(Mathf.Lerp(angleX, x, Time.deltaTime * lerpSpeed), verticalAngleLimit.x, verticalAngleLimit.y);
        angleY = Mathf.Clamp(Mathf.Lerp(angleY, y, Time.deltaTime * lerpSpeed), horizontalAngleLimit.x, horizontalAngleLimit.y);

        Quaternion rotY = Quaternion.AngleAxis(angleY, starryHead.InverseTransformDirection(transform.up));
        starryHead.rotation *= rotY;
        Quaternion rotX = Quaternion.AngleAxis(angleX, starryHead.InverseTransformDirection(transform.TransformDirection(Vector3.right)));
        starryHead.rotation *= rotX;

    }

    //角度标准化
    private float NormalizeAngle(float angle)
    {
        if (angle > 180) angle -= 360f;
        else if (angle < -180) angle += 360f;
        return angle;
    }

    //获取鼠标在世界坐标系中的位置（与物体在同一平面）
    private Vector3 GetMouseWorldPosition()
    {
        // 从相机通过鼠标位置发出一条射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 point = ray.GetPoint(0);
        return point;
    }

    void StartIdleTimer()
    {
        // 如果已有协程在运行，则停止它
        if (idleCoroutine != null)
            StopCoroutine(idleCoroutine);

        // 开始新的待机计时
        idleCoroutine = StartCoroutine(IdleTimerCoroutine());
    }

    IEnumerator IdleTimerCoroutine()
    {
        currentIdleTime = 0f;

        while (currentIdleTime < 10f)
        {
            // 如果角色正在移动或执行动作，则重置计时器
            if (starryState != StarryState.Idle)
            {
                currentIdleTime = 0f;
                yield return new WaitForSeconds(0.1f); // 减少检查频率以优化性能
                continue;
            }

            // 增加待机时间
            currentIdleTime += Time.deltaTime;
            yield return null;
        }

        // 待机时间达到阈值，执行随机动作
        PerformRandomAction();
    }

    void PerformRandomAction()
    {
        // 随机选择一个动作
        string actionName = "IdleMove" + Random.Range(1, 4);
        starryAnimationController.PlayAction(actionName);
    }

    public void OnIdleMoveExit()
    {
        StartIdleTimer();
    }

    public void OnToughingExit()
    {
        StartIdleTimer();
        starryState = StarryState.Idle;
    }

    /*public void OnByeExit()
    {
        float currentWeight = starryAnimator.GetLayerWeight(starryAnimator.GetLayerIndex("Talk"));
        //NPCAnimator.SetLayerWeight(NPCAnimator.GetLayerIndex("Talk"), 0);
        StartCoroutine(SetLayerWeightUpdate(starryAnimator.GetLayerIndex("Talk"), 1f, currentWeight, 0));
    }*/

    IEnumerator SetLayerWeightUpdate(int layerIndex, float waitTime, float targetWeight)
    {
        float timeOrigin = 0f;
        float currentWeight = starryAnimator.GetLayerWeight(layerIndex);
        while (true)
        {
            if (timeOrigin >= waitTime) { break; }
            timeOrigin += Time.deltaTime;
            currentWeight = Mathf.Lerp(currentWeight, targetWeight, 0.05f);
            starryAnimator.SetLayerWeight(layerIndex, currentWeight);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield break;
    }

    private void DetectMeshCollision()
    {
        // 创建射线
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 发射射线并检测碰撞
        if (Physics.Raycast(ray, out hit))
        {
            // 获取第一个接触的MeshRenderer组件
            GameObject gameObject = hit.collider.gameObject;

            if (gameObject.name.Equals("Head") && starryState == StarryState.Idle)
            {
                starryState = StarryState.Toughing;
                starryAnimationController.PlayAction("Tough");
                starryAnimationController.PlayAction("Tough", 2);

                // 加载音频（无需后缀名）
                AudioClip clip = Resources.Load<AudioClip>("tough_voice" + Random.Range(1, 3));
                if (clip != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                }
            }
        }
    }
}
