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
    [HideInInspector] public float stiff, offGround, gravity, pushDis, dirDis, combo;
    public Vector2 velocity, hp, sta, btr, skill;
    public GameObject hitboxes;
    public Transform hitTrans;
    public GameObject hitVFX;

    public enum MoveMode { move, soar, all, none };
    [Tooltip("���ʼҦ�:����/�}�B/����/�L��")] public MoveMode moveMode;
    public enum DirectionMode { noTurn_noCtrl, noTurn_ctrl, turn_ctrl }
    [Tooltip("�ਭ�P��V����:���ਭ�B������/���ਭ������/�ਭ�ñ���")] public DirectionMode drtMode;
    [Tooltip("�i�����ʧ@")] public bool cancel;
    [Tooltip("��������")] public bool hitRange;
    [Tooltip("������i�����ʧ@")] public bool hitCancel;
    [Tooltip("�������ʧ@")] public List<string> cancelList;
    [Tooltip("�������H�Ĥ@�Ӱʧ@")] public List<string> cancelOtherList;
    [Tooltip("�B�Ű���")] public float soarHeight;
    [Tooltip("���ʳt��")] public float moveSpeed;
    [Tooltip("����b�|")] public float radius;
    [Tooltip("�����L")] public bool trigger;
    [Tooltip("��O�ȧN�o��")] public bool staCD;
    [Tooltip("�ϥήɶ��Y��")] public bool timeScale;
    [Tooltip("�ɶ��Y��v")] public float timeScaleRate;
    [Tooltip("�����ˮ`")] public float dmg;
    [Tooltip("�����O�ȶˮ`")] public float staDmg;
    [Tooltip("������O�ȶˮ`")] public float hitStaDmg;
    [Tooltip("�ۨ���O�ȷl��")] public float staLost;
    [Tooltip("����U���Ѷˮ`")] public float btrDmg;
    [Tooltip("�����U���Ѷˮ`")] public float hitBtrDmg;
    [Tooltip("�ۨ��U���ѷl��")] public float btrLost;
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
        if (cancelList != null)
            foreach (string item in cancelList)
            {
                getName = SearchAction(item, mStr, cStr, aStr);
                if (pc.comString.Length <= 1)
                    pc.comString = pc.ConvertMoves(pc.movesNum.ToString(), pc.comString);
                if (getName != null)
                {
                    cancelList.Clear(); cancelOtherList.Clear();
                    pc.actionKey = '\0';
                    actionMsg = getName;
                    return;
                }
            }
        if (cancelOtherList != null)
            for (int i = 1; i < cancelOtherList.Count; i++)
            {
                getName = SearchAction(cancelOtherList[i], mStr, cStr, aStr);
                if (pc.comString.Length <= 1)
                    pc.comString = pc.ConvertMoves(pc.movesNum.ToString(), pc.comString);
                if (getName != null)
                {
                    cancelList.Clear();
                    pc.actionKey = '\0';
                    actionMsg = cancelOtherList[0];
                    cancelOtherList.Clear();
                    return;
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
        if (!hitRange)
        { hited = false; pushDis = 0; }
        if (!hurted)
        {
            if ((moveMode == MoveMode.soar || moveMode == MoveMode.none) && Mathf.Abs(dirDis) < 2)
            {
                if (pc.movesNum == 1 || pc.movesNum == 4 || pc.movesNum == 7) dirDis -= .2f;
                if (pc.movesNum == 3 || pc.movesNum == 6 || pc.movesNum == 9) dirDis += .2f;
            }
            else dirDis = 0;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("HITF") && velocity.y < 0 && offGround <= 0)
            NextState("HITD");
        if (cc.isGrounded && velocity.y < 0)
        {
            if (inState != InState.A && inState != InState.AHU) velocity.y = 0;
            if (inState == InState.A && (moveMode == MoveMode.move || moveMode == MoveMode.none))
                NextState("AL");
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("HITD") && offGround <= 0)
                if (!downed)
                {
                    downed = true;
                    if (velocity.y < -20)
                    {
                        Time.timeScale = .01f; StartCoroutine(TimeStop(.1f));
                        velocity.y *= -.4f; NextState("HITF");
                    }
                    else
                        NextState("DOWN");
                }
                else NextState("WAKE");
        }
        if (inState == InState.WAKE || inState == InState.N)
        { downed = false; opponent.combo = 0; }
        if (inState == InState.HU || inState == InState.AHU || inState == InState.G)
            hurted = true;
        else hurted = false;
        velocity.y -= gravity;
        velocity.x += velocity.x < 0 ? .05f : velocity.x > 0 ? -.05f : 0;
        if (actionMsg != null && (cancel || (hitCancel && hited)) && inState != InState.HU && inState != InState.AHU && inState != InState.G)
        {
            foreach (MoveList.Data item in moveList.data)
            {
                if (item.name == actionMsg && item.staLost > 0)
                    if (!staCD)
                        sta.x -= item.staLost;
                    else actionMsg = null;
                if (item.name == actionMsg && item.btrLost > 0)
                    if (btr.x - item.btrLost >= 0)
                        btr.x -= item.btrLost;
                    else actionMsg = actionMsg + "_NB";
            }
            if (actionMsg != null)
            {
                NextState(actionMsg);
                if (pc.comString.Length <= 1 || (actionMsg != null && actionMsg.Length > 1))
                    pc.comString = pc.ConvertMoves(pc.movesNum.ToString(), pc.comString);
                actionMsg = null;
            }
        }
        if (drtMode == DirectionMode.turn_ctrl) transform.localScale = new Vector3(direction, 1, 1);
        switch (moveMode)
        {
            case MoveMode.move:
                velocity.x = moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.x);
                break;
            case MoveMode.soar:
                velocity.y = soarHeight;
                break;
            case MoveMode.all:
                velocity = new Vector2(moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.x), soarHeight);
                break;
        }
        if (hited && pushDis != 0)
            velocity.x = -pushDis;
        float dis = Mathf.Abs(opponent.transform.position.x - transform.position.x) - 5;
        if (dis > 0)
        {
            if (transform.position.x > opponent.transform.position.x && velocity.x > 0)
                velocity.x = -dis / Time.deltaTime;
            if (transform.position.x < opponent.transform.position.x && velocity.x < 0)
                velocity.x = dis / Time.deltaTime;
            cc.Move(velocity * Time.deltaTime);
            velocity.x = 0;
        }
        float ras = Mathf.Abs(opponent.transform.position.x - transform.position.x) - radius - opponent.radius;
        if (ras < 0 && !trigger && !opponent.trigger && inState != InState.HU && inState != InState.AHU)
        {
            if (transform.position.x < opponent.transform.position.x)
                velocity.x = ras / Time.deltaTime * .75f;
            if (transform.position.x > opponent.transform.position.x)
                velocity.x = -ras / Time.deltaTime * .75f;
            cc.Move(velocity * Time.deltaTime);
            velocity.x = 0;
        }
        if (stiff <= 0 && offGround <= 0)
            if (inState == InState.HU) NextState("5");
            else if (inState == InState.G) NextState("D");
        if (stiff > 0)
            stiff -= 60 * Time.deltaTime;
        if (offGround > 0)
            offGround -= 60 * Time.deltaTime;
        cc.Move((velocity + new Vector2(dirDis, 0)) * Time.deltaTime);
        transform.position = cc.transform.position;
        cc.transform.localPosition *= 0;
    }

    public void ValueEvent()
    {
        if (hp.x <= 0) hp.x = 0;
        if (sta.x <= 0) { staCD = true; sta.x = 0; }
        if (sta.x >= sta.y) { staCD = false; sta.x = sta.y; }
        if (sta.x < sta.y && inState != InState.D && inState != InState.G) sta.x += Time.deltaTime * 50;
    }

    public void Canceler(string canceler)
    {
        cancelList.Clear(); actionMsg = null;
        cancelList.AddRange(canceler.Split(','));
    }

    public void CancelOther(string canceler)
    {
        cancelOtherList.Clear(); actionMsg = null;
        cancelOtherList.AddRange(canceler.Split(','));
    }

    public void NextState(string animState)
    {
        cancelList.Clear(); cancelOtherList.Clear(); actionMsg = null;
        animator.CrossFadeInFixedTime(animState, 0);
    }

    public void Hited(string oppoCol)
    {
        if (oppoCol == "HurtBox" && !hited && !follow)
        {
            hited = true;
            GameObject hitVFXObj = Instantiate(hitVFX, hitTrans.position, hitTrans.rotation);
            hitVFXObj.transform.localScale = transform.localScale;
            StartCoroutine(opponent.Hurted(dmg, staDmg, hitStaDmg, fixRate, stiffDuration, defStiffDuration, hitHeight, hitDistance, airHitHeight, airHitDistance, hitPoint == 0 ? true : false));
        }
    }

    public IEnumerator Hurted(float _dmg, float _staDmg, float _hitSDmg, float _fixRate, float _stiffDur, float _dStiffDur, float _hitH, float _hitD, float _aHitH, float _aHitD, bool _hitHigh)
    {
        bool isAir = inState == InState.A || inState == InState.AHU || inState == InState.DOWN ? true : false;
        offGround = 2;
        if ((inState == InState.D || inState == InState.G) && (opponent.transform.position.x - transform.position.x) * direction > 0)
        {
            transform.localScale = new Vector3(direction, 1, 1);
            stiff = _dStiffDur; NextState("G");
        }
        else
        {
            if (_hitH == 0)
                stiff = _stiffDur;
            if (_hitH != 0 || isAir) //�O�����ۦ��Φb�Ť�
                if (_hitH < 0)
                    NextState("HITD");
                else
                    NextState("HITF");
            else if (_hitHigh) //�O���I��
                if ((opponent.transform.position.x - transform.position.x) * transform.localScale.x > 0) //������
                    NextState("HUB");
                else //�I����
                    NextState("HUF");
            else //�O���I�C
            {
                if ((opponent.transform.position.x - transform.position.x) * transform.localScale.x > 0) //������
                    NextState("HUF");
                else //�I����
                    NextState("HUB");
            }
            opponent.combo++;
        }
        Time.timeScale = .01f;
        yield return new WaitForSecondsRealtime(.1f);
        if (isAir)
            velocity = new Vector2(opponent.transform.position.x > transform.position.x ? -_aHitD : _aHitD, _aHitH);
        else
            velocity = new Vector2(opponent.transform.position.x > transform.position.x ? -_hitD * (inState == InState.G ? .75f : 1) : _hitD * (inState == InState.G ? .75f : 1), _hitH);
        Time.timeScale = 1;
    }

    public IEnumerator TimeStop(float timeLong)
    {
        yield return new WaitForSecondsRealtime(timeLong);
        Time.timeScale = 1;
    }

    void Start()
    {
        gravity = .5f;
        pc = transform.parent.GetComponent<PlayerController>();
        pc = transform.parent.GetComponent<PlayerController>();
        cc = GetComponentInChildren<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        moveList = GetComponent<MoveList>();
        GameObject.Find("TwoCharCam").GetComponent<CinemachineTargetGroup>().AddMember(transform, 1, 0);
        if (pc.name == "Player1")
            opponent = GameObject.Find("Player2").GetComponentInChildren<ActionSystem>();
        else if (pc.name == "Player2")
            opponent = GameObject.Find("Player1").GetComponentInChildren<ActionSystem>();
        transform.localScale = new Vector3(direction, 1, 1);
    }

    void FixedUpdate()
    {
        ComMessage();
        ActionEvent();
        ValueEvent();
    }
}
