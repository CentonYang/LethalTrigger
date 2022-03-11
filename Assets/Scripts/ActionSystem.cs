using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ActionSystem : MonoBehaviour
{
    [HideInInspector] public GameObject controller, opponent;
    [HideInInspector] public Rigidbody2D rgBody;
    [HideInInspector] public MoveList moveList;
    [HideInInspector] public Animator animator;
    [HideInInspector] public string actionMsg;
    [HideInInspector] public int direction;

    public enum MoveMode { set, multi, add };
    //[Tooltip("���ʼҦ�:����/����/�W�[")] public MoveMode moveMode;
    public enum DirectionMode { noTurn_noCtrl, noTurn_ctrl, turn_ctrl }
    [Tooltip("�ਭ�P��V����:���ਭ�B������/���ਭ������/�ਭ�ñ���")] public DirectionMode drtMode;
    [Tooltip("�i�����ʧ@")] public bool cancel;
    [Tooltip("�i�������ʧ@")] public List<string> cancelList;
    [Tooltip("���D����")] public float jumpHeight;
    [Tooltip("���ʳt��")] public float moveSpeed;
    [Tooltip("�ϥήɶ��Y��")] public bool timeScale;
    [Tooltip("�ɶ��Y��v")] public float timeScaleRate;
    [Tooltip("�����ˮ`")] public float dmg;
    [Tooltip("��O�ȶˮ`")] public float vitDmg;
    [Tooltip("������O�ȶˮ`")] public float hitVitDmg;
    [Tooltip("�ۨ���O�ȷl��")] public float vitLost;
    [Tooltip("�ˮ`�ץ�")] public float fixRate;
    [Tooltip("�����ɶ�")] public float stiffDuration;
    [Tooltip("���m�����ɶ�")] public float defStiffDuration;
    [Tooltip("��������")] public float hitHeight;
    [Tooltip("���h�Z��")] public float hitDistance;
    [Tooltip("�Ť���������")] public float airHitHeight;
    [Tooltip("�Ť����h�Z��")] public float airHitDistance;
    [Tooltip("�����˦a���")] public bool hitGround;
    [Tooltip("�T�w/���Y�ۦ�")] public bool follow;
    [Tooltip("�i��z���")] public bool triggerOppo;
    public enum HitPoint { high, low };
    [Tooltip("�������I���C:��/�C")] public HitPoint hitPoint;
    [Tooltip("Counter�ɶZ")] public bool counterRange;

    public void ActionMessage(string atName, int drt)
    {
        print(atName);
        actionMsg = atName;
        direction = drt;
        if (cancelList != null && cancel)
        {
            for (int i = 0; i < cancelList.Count; i++)
                if (atName == cancelList[i])
                {
                    animator.CrossFadeInFixedTime(atName, 0.05f);
                    return;
                }
        }
    }

    public void ActionEvent()
    {
        Physics2D.IgnoreLayerCollision(3, 3, triggerOppo);
        if (drtMode == DirectionMode.turn_ctrl) transform.localScale = new Vector3(1, 1, direction);
        //switch (moveMode)
        //{
        //    case MoveMode.set:
        rgBody.velocity = (Vector2.right * moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.z) + Vector2.up * jumpHeight) * timeScaleRate;
        //        break;
        //    case MoveMode.multi:
        //        rgBody.velocity *= (Vector2.right * moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.z) + Vector2.up * jumpHeight) * timeScaleRate;
        //        break;
        //    case MoveMode.add:
        //        rgBody.velocity += (Vector2.right * moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.z) + Vector2.up * jumpHeight) * timeScaleRate;
        //        break;
        //}
        if (timeScale)
            foreach (Animator anims in FindObjectsOfType<Animator>())
                anims.SetFloat("scale", timeScaleRate);
    }

    public void Canceler(string canceler)
    {
        cancelList.Clear();
        cancelList.AddRange(canceler.Split(','));
    }

    public void NextState(string animState)
    {
        animator.CrossFadeInFixedTime(animState, 0.05f);
    }

    void Awake()
    {
        controller = transform.parent.gameObject;
        animator = GetComponentInChildren<Animator>();
        rgBody = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(3, 3, false);
        moveList = GetComponent<MoveList>();
        GameObject.Find("TwoCharCam").GetComponent<CinemachineTargetGroup>().AddMember(transform, 1, 0);
        if (controller.name == "Player1")
        { opponent = GameObject.Find("Player2").transform.GetChild(0).gameObject; direction = 1; }
        else if (controller.name == "Player2")
        { opponent = GameObject.Find("Player1").transform.GetChild(0).gameObject; direction = -1; }
        transform.localScale = new Vector3(1, 1, direction);
    }

    void Start()
    {

    }

    void Update()
    {
        if (rgBody.velocity.y < 0) ActionMessage("Fall", '5');
        if (opponent.transform.position.x - transform.position.x < -5)
            if (rgBody.velocity.x > 0)
                rgBody.velocity = -Vector2.right;
        if (opponent.transform.position.x - transform.position.x > 5)
            if (rgBody.velocity.x < 0)
                rgBody.velocity = Vector2.right;
        timeScaleRate = animator.GetFloat("scale");
        ActionEvent();
    }
}
