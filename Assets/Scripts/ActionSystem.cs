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
    [HideInInspector] public float stiff, offGround, gravity, pushDis, dirDis;
    public Vector2 velocity, hp, sta, btr, skill;
    public GameObject hitboxes;
    public Transform hitTrans;
    public GameObject hitVFX;

    public enum MoveMode { move, soar, all, none };
    [Tooltip("仮以社Α:仮以/�}�B/��魁/�L鋸")] public MoveMode moveMode;
    public enum DirectionMode { noTurn_noCtrl, noTurn_ctrl, turn_ctrl }
    [Tooltip("退┃�Pよ�V臼��:ぃ退┃�Bぃ臼��/ぃ退┃��臼��/退┃�単烏�")] public DirectionMode drtMode;
    [Tooltip("�i����以�@")] public bool cancel;
    [Tooltip("ゴ聖����")] public bool hitRange;
    [Tooltip("聖い�瓮i����以�@")] public bool hitCancel;
    [Tooltip("�����紺払@")] public List<string> cancelList;
    [Tooltip("�������H可�@�唹払@")] public List<string> cancelOtherList;
    [Tooltip("�B�徹���")] public float soarHeight;
    [Tooltip("仮以�t��")] public float moveSpeed;
    [Tooltip("┐�皀b�|")] public float radius;
    [Tooltip("┐�皙鏐L")] public bool trigger;
    [Tooltip("�陲O�櫨N�oい")] public bool staCD;
    [Tooltip("�魯流俵〜Y��")] public bool timeScale;
    [Tooltip("�俵〜Y�餡v")] public float timeScaleRate;
    [Tooltip("ю聖極�`")] public float dmg;
    [Tooltip("┥�蹼陲O�閥邦`")] public float staDmg;
    [Tooltip("聖い�陲O�閥邦`")] public float hitStaDmg;
    [Tooltip("�曩��陲O�鳩l��")] public float staLost;
    [Tooltip("┥�躾U換煮極�`")] public float btrDmg;
    [Tooltip("聖い�U換煮極�`")] public float hitBtrDmg;
    [Tooltip("�曩��U換煮�l��")] public float btrLost;
    [Tooltip("極�`�廛�")] public float fixRate;
    [Tooltip("侍�舟俵�")] public float stiffDuration;
    [Tooltip("┥�m侍�舟俵�")] public float defStiffDuration;
    [Tooltip("聖�鍵���")] public float hitHeight;
    [Tooltip("聖�h�Z託")] public float hitDistance;
    [Tooltip("�鼎だ鮫鍵���")] public float airHitHeight;
    [Tooltip("�鼎だ三h�Z託")] public float airHitDistance;
    [Tooltip("�N�J�昿＾糞e")] public bool follow;
    public enum HitPoint { high, low };
    [Tooltip("ю聖ゴ�I葵�C:葵/�C")] public HitPoint hitPoint;
    public enum InState { N, FF, A, AL, D, G, HU, AHU, DOWN, WAKE };
    [Tooltip("���A: �a��/ 縦��/ �鼎�/ 絹�a/ ┥�m/ ┥��/ ��極/ �鼎え�極/ �胞a/ �_┃")] public InState inState;


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
        if (inState == InState.WAKE)
            downed = false;
        if (inState == InState.HU || inState == InState.AHU)
            hurted = true;
        else hurted = false;
        velocity.y -= gravity;
        velocity.x += velocity.x < 0 ? .05f : velocity.x > 0 ? -.05f : 0;
        if (actionMsg != null && (cancel || (hitCancel && hited)))
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
                animator.CrossFadeInFixedTime(actionMsg, 0);
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
        if (stiff <= 0 && offGround <= 0 && inState == InState.HU)
            NextState("5");
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
        if (_hitH == 0)
            stiff = _stiffDur;
        if (_hitH != 0 || isAir) //�Oゴ�県昿？陸b�鼎�
            if (_hitH < 0)
                NextState("HITD");
            else
                NextState("HITF");
        else if (_hitHigh) //�Oゴ�I葵
            if ((opponent.transform.position.x - transform.position.x) * transform.localScale.x > 0) //�厩鏐錣�
                NextState("HUB");
            else //�I刻刻も
                NextState("HUF");
        else //�Oゴ�I�C
        {
            if ((opponent.transform.position.x - transform.position.x) * transform.localScale.x > 0) //�厩鏐錣�
                NextState("HUF");
            else //�I刻刻も
                NextState("HUB");
        }
        Time.timeScale = .01f;
        yield return new WaitForSecondsRealtime(.1f);
        if (isAir)
            velocity = new Vector2(opponent.transform.position.x > transform.position.x ? -_aHitD : _aHitD, _aHitH);
        else
            velocity = new Vector2(opponent.transform.position.x > transform.position.x ? -_hitD : _hitD, _hitH);
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

    void FixedUpdate()
    {
        ComMessage();
        ActionEvent();
        ValueEvent();
    }
}
