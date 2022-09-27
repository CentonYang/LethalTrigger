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
    [Tooltip("移動模式:移動/漂浮/全部/無視")] public MoveMode moveMode;
    public enum DirectionMode { noTurn_noCtrl, noTurn_ctrl, turn_ctrl }
    [Tooltip("轉身與方向控制:不轉身且不控制/不轉身但控制/轉身並控制")] public DirectionMode drtMode;
    [Tooltip("可取消動作")] public bool cancel;
    [Tooltip("打擊長度")] public bool hitRange;
    [Tooltip("擊中後可取消動作")] public bool hitCancel;
    [Tooltip("取消的動作")] public List<string> cancelList;
    [Tooltip("取消但以第一個動作")] public List<string> cancelOtherList;
    [Tooltip("浮空高度")] public float soarHeight;
    [Tooltip("移動速度")] public float moveSpeed;
    [Tooltip("角色半徑")] public float radius;
    [Tooltip("角色穿過")] public bool trigger;
    [Tooltip("氣力值冷卻中")] public bool staCD;
    [Tooltip("使用時間縮放")] public bool timeScale;
    [Tooltip("時間縮放率")] public float timeScaleRate;
    [Tooltip("攻擊傷害")] public float dmg;
    [Tooltip("防住氣力值傷害")] public float staDmg;
    [Tooltip("擊中氣力值傷害")] public float hitStaDmg;
    [Tooltip("自身氣力值損耗")] public float staLost;
    [Tooltip("防住燃晶槽傷害")] public float blzDmg;
    [Tooltip("擊中燃晶槽傷害")] public float hitBlzDmg;
    [Tooltip("自身燃晶槽損耗")] public float blzLost;
    [Tooltip("傷害修正")] public float fixRate;
    [Tooltip("僵直時間")] public float stiffDuration;
    [Tooltip("防禦僵直時間")] public float defStiffDuration;
    [Tooltip("擊飛高度")] public float hitHeight;
    [Tooltip("擊退距離")] public float hitDistance;
    [Tooltip("空中擊飛高度")] public float airHitHeight;
    [Tooltip("空中擊退距離")] public float airHitDistance;
    [Tooltip("代入招式動畫")] public bool follow;
    public enum HitPoint { high, low };
    [Tooltip("攻擊打點高低:高/低")] public HitPoint hitPoint;
    public enum InState { N, FF, A, AL, D, G, HU, AHU, DOWN, WAKE };
    [Tooltip("狀態: 地面/ 衝刺/ 空中/ 落地/ 防禦/ 防住/ 受傷/ 空中受傷/ 倒地/ 起身")] public InState inState;


    public void ComMessage()
    {
        direction = pc.diraction;
        string mStr = pc.moveString, cStr = pc.comString, aStr = pc.actionKey.ToString();
        string getName;
        if (cancelList != null && (cancel || (hitCancel && hited)))
            foreach (string item in cancelList)
            {
                getName = SearchAction(item, mStr, cStr, aStr);
                if (pc.comString.Length > 1)
                    pc.comString = pc.comString.Substring(1);
                else
                    pc.comString = pc.ConvertMoves(pc.movesNum.ToString(), pc.comString);
                if (getName != null)
                {
                    cancelList.Clear(); cancelOtherList.Clear();
                    pc.actionKey = '\0';
                    animator.CrossFadeInFixedTime(getName, 0);
                    return;
                }
            }
        if (cancelOtherList != null && (cancel || (hitCancel && hited)))
            for (int i = 1; i < cancelOtherList.Count; i++)
            {
                getName = SearchAction(cancelOtherList[i], mStr, cStr, aStr);
                if (pc.comString.Length > 1)
                    pc.comString = pc.comString.Substring(1);
                else
                    pc.comString = pc.ConvertMoves(pc.movesNum.ToString(), pc.comString);
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

    public void Canceler(string canceler)
    {
        cancelList.Clear();
        cancelList.AddRange(canceler.Split(','));
    }

    public void CancelOther(string canceler)
    {
        cancelOtherList.Clear();
        cancelOtherList.AddRange(canceler.Split(','));
    }

    public void NextState(string animState)
    {
        cancelList.Clear(); cancelOtherList.Clear();
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
        if (_hitH != 0 || isAir) //是打飛招式或在空中
            if (_hitH < 0)
                NextState("HITD");
            else
                NextState("HITF");
        else if (_hitHigh) //是打點高
            if ((opponent.transform.position.x - transform.position.x) * transform.localScale.x > 0) //面對對手
                NextState("HUB");
            else //背對對手
                NextState("HUF");
        else //是打點低
        {
            if ((opponent.transform.position.x - transform.position.x) * transform.localScale.x > 0) //面對對手
                NextState("HUF");
            else //背對對手
                NextState("HUB");
        }
        Time.timeScale = .01f;
        yield return new WaitForSecondsRealtime(.1f);
        if (isAir)
        {
            velocity = new Vector2(opponent.transform.position.x > transform.position.x ? -_aHitD : _aHitD, _aHitH);
        }
        else
        {
            velocity = new Vector2(opponent.transform.position.x > transform.position.x ? -_hitD : _hitD, _hitH);
        }
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
    }
}
