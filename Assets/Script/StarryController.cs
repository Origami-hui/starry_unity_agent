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
        Toughing = 3 //���ڽ���״̬��������
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
    private Vector2 horizontalAngleLimit = new Vector2(-50f, 50f);//ˮƽ�����ϵĽǶ�����
    private Vector2 verticalAngleLimit = new Vector2(-40f, 40f);//��ֱ�����ϵĽǶ�����
    private float lerpSpeed = 5f;

    [SerializeField]
    private float currentIdleTime = 0f;      // ��ǰ����ʱ��
    private Coroutine idleCoroutine;         // ������ʱЭ������

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

        // ��ʼ������ʱ
        StartIdleTimer();
    }

    void Update()
    {
        // ������������
        if (Input.GetMouseButtonDown(0))
        {
            DetectMeshCollision();
        }
    }

    void LateUpdate()
    {
        //����ʱ�������
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

    //�����������Ų�ͬ�Ķ���
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

    //����ĳ��
    public void LookAtPosition(Vector3 position)
    {
        //ͷ��λ��
        //Vector3 headPosition = transform.position + transform.up * headHeight;
        //����
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

    //�Ƕȱ�׼��
    private float NormalizeAngle(float angle)
    {
        if (angle > 180) angle -= 360f;
        else if (angle < -180) angle += 360f;
        return angle;
    }

    //��ȡ�������������ϵ�е�λ�ã���������ͬһƽ�棩
    private Vector3 GetMouseWorldPosition()
    {
        // �����ͨ�����λ�÷���һ������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 point = ray.GetPoint(0);
        return point;
    }

    void StartIdleTimer()
    {
        // �������Э�������У���ֹͣ��
        if (idleCoroutine != null)
            StopCoroutine(idleCoroutine);

        // ��ʼ�µĴ�����ʱ
        idleCoroutine = StartCoroutine(IdleTimerCoroutine());
    }

    IEnumerator IdleTimerCoroutine()
    {
        currentIdleTime = 0f;

        while (currentIdleTime < 10f)
        {
            // �����ɫ�����ƶ���ִ�ж����������ü�ʱ��
            if (starryState != StarryState.Idle)
            {
                currentIdleTime = 0f;
                yield return new WaitForSeconds(0.1f); // ���ټ��Ƶ�����Ż�����
                continue;
            }

            // ���Ӵ���ʱ��
            currentIdleTime += Time.deltaTime;
            yield return null;
        }

        // ����ʱ��ﵽ��ֵ��ִ���������
        PerformRandomAction();
    }

    void PerformRandomAction()
    {
        // ���ѡ��һ������
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
        // ��������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // �������߲������ײ
        if (Physics.Raycast(ray, out hit))
        {
            // ��ȡ��һ���Ӵ���MeshRenderer���
            GameObject gameObject = hit.collider.gameObject;

            if (gameObject.name.Equals("Head") && starryState == StarryState.Idle)
            {
                starryState = StarryState.Toughing;
                starryAnimationController.PlayAction("Tough");
                starryAnimationController.PlayAction("Tough", 2);

                // ������Ƶ�������׺����
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
