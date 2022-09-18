using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ActionSystem : MonoBehaviour
{
    [HideInInspector] public PlayerController pc;
    [HideInInspector] public CharacterController cc;
    [HideInInspector] public ActionSystem opponent;
    [HideInInspector] public MoveList moveList;
    [HideInInspector] public Animator animator;
    [HideInInspector] public string actionMsg;
    [HideInInspector] public int direction;
    [HideInInspector] public bool hited, hurted, downed;
    [HideInInspector] public float stiff, offGround, gravity;
    public Vector2 velocity;
    public GameObject hitboxes;
    public Transform hitTrans;
    public GameObject hitVFX;

    public enum MoveMode { move, soar, all, none };
    [Tooltip("���ʼҦ�:����/�}�B/����/�L��")] public MoveMode moveMode;
    public enum DirectionMode { noTurn_noCtrl, noTurn_ctrl, turn_ctrl }
    [Tooltip("�ਭ�P��V����:���ਭ�B������/���ਭ������/�ਭ�ñ���")] public DirectionMode drtMode;
    [Tooltip("�i�����ʧ@")] public bool cancel;
    [Tooltip("������i�����ʧ@")] public bool hitCancel;
    [Tooltip("�������ʧ@")] public List<string> cancelList;
    [Tooltip("�������H�Ĥ@�Ӱʧ@")] public List<string> cancelOtherList;
    [Tooltip("�B�Ű���")] public float soarHeight;
    [Tooltip("���ʳt��")] public float moveSpeed;
    [Tooltip("�ϥήɶ��Y��")] public bool timeScale;
    [Tooltip("�ɶ��Y��v")] public float timeScaleRate;
    [Tooltip("�����ˮ`")] public float dmg;
    [Tooltip("�����O�ȶˮ`")] public float vitDmg;
    [Tooltip("������O�ȶˮ`")] public float hitVitDmg;
    [Tooltip("�ۨ���O�ȷl��")] public float vitLost;
    [Tooltip("����U���Ѷˮ`")] public float blzDmg;
    [Tooltip("�����U���Ѷˮ`")] public float hitBlzDmg;
    [Tooltip("�ۨ��U���ѷl��")] public float blzLost;
    [Tooltip("�ˮ`�ץ�")] public float fixRate;
    [Tooltip("�����ɶ�")] public float stiffDuration;
    [Tooltip("���m�����ɶ�")] public float defStiffDuration;
    [Tooltip("��������")] public float hitHeight;
    [Tooltip("���h�Z��")] public float hitDistance;
    [Tooltip("�Ť���������")] public float airHitHeight;
    [Tooltip("�Ť����h�Z��")] public float airHitDistance;
    [Tooltip("�N�J�ۦ��ʵe")] public bool follow;
    public enum HitPoint { high, low };
    [Tooltip("�������I���C:��/�C")] public HitPoint hitPoint;
    public enum InState { N, FF, A, AL, D, G, HU, AHU, DOWN, WAKE };
    [Tooltip("���A: �a��/ �Ĩ�/ �Ť�/ ���a/ ���m/ ����/ ����/ �Ť�����/ �˦a/ �_��")] public InState inState;


    public void ComMessage()
    {
        direction = pc.diraction;
        string mStr = pc.moveString, cStr = pc.comString, aStr = pc.actionKey.ToString();
        string getName;
        if (cancelList != null && cancel)
            foreach (string item in cancelList)
            {
                getName = SearchAction(item, mStr, cStr, aStr);
                if (getName != null)
                {
                    cancelList.Clear(); cancelOtherList.Clear();
                    pc.actionKey = '\0';
                    animator.CrossFadeInFixedTime(getName, 0);
                    return;
                }

            }
        if (cancelOtherList != null && cancel)
            for (int i = 1; i < cancelOtherList.Count; i++)
            {
                getName = SearchAction(cancelOtherList[i], mStr, cStr, aStr);
                if (getName != null)
                {
                    cancelList.Clear();
                    animator.CrossFadeInFixedTime(cancelOtherList[0], 0);
                    pc.actionKey = '\0';
                    cancelOtherList.Clear(); return;
                }
            }
    }

    string SearchAction(string target, string mStr, string cStr, string aStr)
    {
        string inStateStr = inState == 0 ? "" : inState.ToString();
        if (target == inStateStr + cStr + aStr) return target;
        if (target == cStr + aStr) return target;
        if (cStr.Length > 1 && target == inStateStr + cStr.Substring(1) + aStr) return target;
        if (cStr.Length > 1 && target == cStr.Substring(1) + aStr) return target;
        if (target == inStateStr + mStr + aStr) return target;
        if (target == mStr + aStr) return target;
        if (target == inStateStr + aStr) return target;
        if (target == aStr) return target;
        if (target == inStateStr + cStr) return target;
        if (target == cStr) return target;
        if (cStr.Length > 1 && target == inStateStr + cStr.Substring(1)) return target;
        if (cStr.Length > 1 && target == cStr.Substring(1)) return target;
        if (mStr.Length > 0 && target == inStateStr + mStr[mStr.Length - 1].ToString()) return target;
        if (mStr.Length > 0 && target == mStr[mStr.Length - 1].ToString()) return target;
        return null;
    }

    public void ActionEvent()
    {
        velocity.y -= gravity * Time.deltaTime;
        if (cc.isGrounded)
            velocity.y = 0;
        if (drtMode == DirectionMode.turn_ctrl) transform.localScale = new Vector3(direction, 1, 1);
        switch (moveMode)
        {
            case MoveMode.move:
                velocity.x = moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.x) * Time.deltaTime;
                break;
            case MoveMode.soar:
                velocity.y = soarHeight * Time.deltaTime;
                break;
            case MoveMode.all:
                velocity = new Vector2(moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.x), soarHeight) * Time.deltaTime;
                break;
        }
        float dis = Mathf.Abs(opponent.transform.position.x - transform.position.x) - 5;
        if (dis > 0)
        {
            if (opponent.transform.position.x - transform.position.x < 0 && velocity.x > 0)
                velocity.x -= dis;
            if (opponent.transform.position.x - transform.position.x > 0 && velocity.x < 0)
                velocity.x += dis;
        }
        if (!animator.IsInTransition(0))
        {
            if (velocity.y < -1f)
            {
                //    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") || animator.GetCurrentAnimatorStateInfo(0).IsName("FJump"))
                //        NextState("Fall");
                //    if (animator.GetCurrentAnimatorStateInfo(0).IsName("HitFly"))
                //        NextState("Drop");
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("HitFly"))
                    NextState("Drop");
            }
            //else
            if (cc.isGrounded)
            {
                if (inState == InState.A && (moveMode == MoveMode.move || moveMode == MoveMode.none))
                    NextState("AL");
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Drop") || animator.GetCurrentAnimatorStateInfo(0).IsName("HitFly"))
                    if (!downed)
                    { downed = true; NextState("Down"); }
                    else NextState("Stand");
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("LDrop"))
                    if (!downed)
                    { downed = true; NextState("LDown"); }
                    else NextState("LStand");
            }
        }
        if (stiff <= 0 && !animator.IsInTransition(0) && (animator.GetCurrentAnimatorStateInfo(0).IsName("UStiff") || animator.GetCurrentAnimatorStateInfo(0).IsName("DStiff")))
            NextState("Idle");
        if (stiff > 0)
            stiff -= 60 * Time.deltaTime;
        if (offGround > 0)
            offGround -= 60 * Time.deltaTime;
        cc.Move(velocity);
        transform.position = cc.transform.position;
        cc.transform.localPosition *= 0;
    }

    public void Canceler(string canceler)
    {
        cancelList.Clear();
        cancelList.AddRange(canceler.Split(','));
        //controller.TransformOutput(controller.moveKey[0]);
        //controller.TransformOutput(controller.moveKey[1]);
    }

    public void CancelOther(string canceler)
    {
        cancelOtherList.Clear();
        cancelOtherList.AddRange(canceler.Split(','));
        //controller.TransformOutput(controller.moveKey[0]);
        //controller.TransformOutput(controller.moveKey[1]);
    }

    public void NextState(string animState)
    {
        cancelList.Clear(); cancelOtherList.Clear();
        animator.CrossFadeInFixedTime(animState, 0);
        //controller.actionMsg.Add(controller.storageName);
        //controller.TransformOutput(controller.moveKey[0]);
        //controller.TransformOutput(controller.moveKey[1]);
    }

    public void Jump(float force)
    {
        velocity.y = force;
    }

    public void push(float force)
    {
        velocity.x = force;
    }

    public void SetADef(int set)
    {
        //isADef = set == 0 ? false : true;
    }

    public void SetDown(int set)
    {
        downed = set == 0 ? false : true;
    }

    public void Hited(string oppoCol)
    {
        if (oppoCol == "HurtBox" && !hited && !follow)
        {
            hited = true;
            GameObject hitVFXObj = Instantiate(hitVFX, hitTrans.position, hitTrans.rotation);
            hitVFXObj.transform.localScale = transform.localScale;
            StartCoroutine(opponent.Hurted(dmg, vitDmg, hitVitDmg, fixRate, stiffDuration, defStiffDuration, hitHeight, hitDistance, airHitHeight, airHitDistance, hitPoint == 0 ? true : false));
        }
    }

    public IEnumerator Hurted(float _dmg, float _vitDmg, float _hitVDmg, float _fixRate, float _stiffDur, float _dStiffDur, float _hitH, float _hitD, float _aHitH, float _aHitD, bool _hitHigh)
    {
        hurted = true;
        bool isAir = inState == InState.A || inState == InState.AHU ? true : false;
        if (_hitHigh) //�O���I��
            if (_hitH != 0 || isAir) //�O�����ۦ��Φb�Ť�
                if (_hitH > 0) //���W��
                    NextState("HitFly");
                else //���U��
                    NextState("Drop");
            else //���O�����ۦ��]���O�b�Ť�
            if ((opponent.transform.position.x - transform.position.x) * transform.localScale.z > 0) //������
                NextState("UStiff");
            else //�I����
                NextState("DStiff");
        else //�O���I�C
        {
            if (_hitH != 0 || isAir) //�O�����ۦ��Φb�Ť�
                NextState("LDrop");
            else //���O�����ۦ�
            if ((opponent.transform.position.x - transform.position.x) * transform.localScale.z > 0) //������
                NextState("DStiff");
            else //�I����
                NextState("UStiff");
        }
        Time.timeScale = 0.01f;
        stiff = _stiffDur;
        offGround = 3;
        //print("hurt!");
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1;
        if (isAir)
            velocity = new Vector2(_aHitD * (opponent.transform.position.x - transform.position.x < 0 ? 1 : -1), _aHitH);
        else
            velocity = new Vector2(_hitD * (opponent.transform.position.x - transform.position.x < 0 ? 1 : -1), _hitH);
    }

    void Awake()
    {
        gravity = 5;
        pc = transform.parent.GetComponent<PlayerController>();
        cc = GetComponentInChildren<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        moveList = GetComponent<MoveList>();
        GameObject.Find("TwoCharCam").GetComponent<CinemachineTargetGroup>().AddMember(transform, 1, 0);
        if (pc.name == "Player1")
        { opponent = GameObject.Find("Player2").GetComponentInChildren<ActionSystem>(); direction = 1; }
        else if (pc.name == "Player2")
        { opponent = GameObject.Find("Player1").GetComponentInChildren<ActionSystem>(); direction = -1; }
        transform.localScale = new Vector3(direction, 1, 1);
    }

    void Start()
    {

    }

    void Update()
    {
        if (!hitboxes.activeSelf)
            hited = false;
        ActionEvent();
        ComMessage();
    }
}