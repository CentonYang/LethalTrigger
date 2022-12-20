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
    [HideInInspector] public AudioSource audioSource;
    public Sounder sounder;
    [HideInInspector] public string actionMsg, acceptMsg;
    [HideInInspector] public int direction, hitType;
    [HideInInspector] public bool hited, hurted, downbreak;
    [HideInInspector] public float stiff, pushReac, gravity, pushDis, dirDis, combo, downed;
    [HideInInspector] public Vector2 velocity, hurtVel;
    public Vector2 hp, sta, btr, skill, fix;
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
    [Tooltip("防住燃晶槽傷害")] public float btrDmg;
    [Tooltip("擊中燃晶槽傷害")] public float hitBtrDmg;
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
    [Tooltip("音效列表")] public List<AudioClip> audios;

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
        if ((moveMode == MoveMode.soar || moveMode == MoveMode.none) && inState == InState.A && Mathf.Abs(dirDis) < 2)
        {
            if (pc.movesNum == 1 || pc.movesNum == 4 || pc.movesNum == 7) dirDis -= .2f;
            if (pc.movesNum == 3 || pc.movesNum == 6 || pc.movesNum == 9) dirDis += .2f;
        }
        else dirDis = 0;
        if (actionMsg != null && (cancel || (hitCancel && hited)) && acceptMsg == null)
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
                    else actionMsg = actionMsg + "NB";
            }
            if (actionMsg != null)
            {
                NextState(actionMsg);
                if (pc.comString.Length <= 1 || (actionMsg != null && actionMsg.Length > 1))
                    pc.comString = pc.ConvertMoves(pc.movesNum.ToString(), pc.comString);
                actionMsg = null;
            }
        }
        if (acceptMsg != null)
        {
            if (inState != InState.WAKE)
                NextState(acceptMsg);
            acceptMsg = null;
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("HITF") && velocity.y < 0)
            NextState("HITD");
        if (cc.isGrounded && velocity.y < 0)
        {
            if (inState == InState.A && (moveMode == MoveMode.move || moveMode == MoveMode.none))
                NextState("AL");
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("HITD") && !downbreak)
                if (velocity.y < -20)
                {
                    downbreak = true;
                    Time.timeScale = .5f; StartCoroutine(DownBreak(velocity.y, .1f));
                    velocity.y = 0;
                    audioSource.clip = audios[5]; Sounder sd = Instantiate(sounder, audioSource.transform);
                }
                else
                {
                    if (downed > 0) NextState("WAKE");
                    else NextState("DOWN");
                    downed++;
                }
        }
        if (inState == InState.HU || inState == InState.AHU || inState == InState.G)
            hurted = true;
        else hurted = false;
        velocity.y -= gravity;
        velocity.x += velocity.x < 0 ? .05f : velocity.x > 0 ? -.05f : 0;
        if (stiff <= 0 && pushReac <= 0)
            if (inState == InState.HU) NextState("5");
            else if (inState == InState.G) NextState("D");
        if (stiff > 0)
            stiff--;
        if (pushReac > 0)
        {
            velocity = hurtVel;
            pushReac--;
        }
        if ((inState == InState.WAKE || inState == InState.N) && pushReac <= 0)
        { downed = 0; opponent.combo = 0; fix.x = fix.y; }
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
                velocity.x = -dis / Time.fixedDeltaTime;
            if (transform.position.x < opponent.transform.position.x && velocity.x < 0)
                velocity.x = dis / Time.fixedDeltaTime;
            cc.Move(velocity * Time.fixedDeltaTime);
            velocity.x = 0;
        }
        float ras = Mathf.Abs(opponent.transform.position.x - transform.position.x) - radius - opponent.radius;
        if (ras < 0 && !trigger && !opponent.trigger && inState != InState.HU && inState != InState.AHU)
        {
            if (transform.position.x < opponent.transform.position.x)
                velocity.x = ras / Time.fixedDeltaTime * .75f;
            if (transform.position.x > opponent.transform.position.x)
                velocity.x = -ras / Time.fixedDeltaTime * .75f;
            cc.Move(velocity * Time.fixedDeltaTime);
            velocity.x = 0;
        }
        cc.Move((velocity + new Vector2(dirDis, 0)) * Time.fixedDeltaTime);
        transform.position = cc.transform.position;
        cc.transform.localPosition *= 0;
    }

    void ValueEvent()
    {
        if (hp.x < 0) hp.x = 0; if (hp.x > hp.y) hp.x = hp.y;
        if (sta.x <= 0) { staCD = true; sta.x = 0; }
        if (sta.x >= sta.y) { staCD = false; sta.x = sta.y; }
        if (sta.x < sta.y && inState != InState.D && inState != InState.G) sta.x += Time.fixedDeltaTime * 50;
        if (btr.x < 0) btr.x = 0; if (btr.x > btr.y) btr.x = btr.y;
        if (skill.x < 0) skill.x = 0; if (skill.x > skill.y) skill.x = skill.y;
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
        if (animator.HasState(0, Animator.StringToHash(animState)))
            animator.Play(animState, -1, 0);
    }

    public void Hited(string oppoCol)
    {
        if (oppoCol == "HurtBox" && !hited && !follow && !(opponent.cc.isGrounded && opponent.velocity.y < 0 && opponent.downed > 1))
        {
            hited = true;
            GameObject hitVFXObj = Instantiate(hitVFX, hitTrans.position, hitTrans.rotation);
            hitVFXObj.transform.localScale = transform.localScale;
            StartCoroutine(opponent.Hurted(dmg, staDmg, hitStaDmg, btrDmg, hitBtrDmg, fixRate, stiffDuration, defStiffDuration, hitHeight, hitDistance, airHitHeight, airHitDistance, hitPoint == 0 ? true : false));
        }
    }

    public void AudioPlay(string type) //打:h/砍:s/銃:g/只有擊中音效:+n
    {
        Sounder sd;
        switch (type)
        {
            case "h":
                hitType = 1; audioSource.clip = audios[0];
                sd = Instantiate(sounder, audioSource.transform); break;
            case "hn":
                hitType = 1; audioSource.clip = audios[0]; break;
            case "s":
                hitType = 2; audioSource.clip = audios[1];
                sd = Instantiate(sounder, audioSource.transform); break;
            case "sn":
                hitType = 2; audioSource.clip = audios[1]; break;
            case "g":
                hitType = 3; audioSource.clip = audios[6];
                sd = Instantiate(sounder, audioSource.transform); break;
        }
    }

    public IEnumerator Hurted(float _dmg, float _staDmg, float _hitSDmg, float _btrDmg, float _hitBDmg, float _fixRate, float _stiffDur, float _dStiffDur, float _hitH, float _hitD, float _aHitH, float _aHitD, bool _hitHigh)
    {
        bool isAir = inState == InState.A || inState == InState.AHU || inState == InState.DOWN ? true : false;
        pushReac = 2;
        if ((inState == InState.D || inState == InState.G) && (opponent.transform.position.x - transform.position.x) * direction > 0)
        {
            transform.localScale = new Vector3(direction, 1, 1);
            audioSource.clip = audios[4]; Sounder sd = Instantiate(sounder, audioSource.transform);
            float _gHitH = 0;
            //傷害計算
            btr.x += _btrDmg;
            opponent.skill.x += _btrDmg / 2;
            sta.x += _staDmg;
            if (sta.x > 0 && !staCD)
            { stiff = _dStiffDur; if (!hitRange) NextState("G"); }
            else if (_hitH != 0) //是打飛招式或在空中
            {
                if (_hitH < 0)
                    NextState("HITD");
                else
                    NextState("HITF");
                hp.x += _dmg / 2; _gHitH = _hitH;
            }
            else
            { stiff = _stiffDur; NextState("BK"); hp.x += _dmg / 2; }
            //////
            hurtVel = new Vector2(opponent.transform.position.x > transform.position.x ? -_hitD * .75f : _hitD * .75f, _gHitH);
        }
        else
        {
            audioSource.clip = opponent.hitType == 1 || opponent.hitType == 3 ? audios[2] : opponent.hitType == 2 ? audios[3] : null;
            Sounder sd = Instantiate(sounder, audioSource.transform);
            opponent.combo++;
            //傷害計算
            hp.x += _dmg * fix.x;
            fix.x *= _fixRate;
            sta.x += _hitSDmg; btr.x += _hitBDmg;
            opponent.skill.x += _hitBDmg / 2;
            //////
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
            if (isAir)
                hurtVel = new Vector2(opponent.transform.position.x > transform.position.x ? -_aHitD : _aHitD, _aHitH);
            else
                hurtVel = new Vector2(opponent.transform.position.x > transform.position.x ? -_hitD : _hitD, _hitH);
        }
        Time.timeScale = .5f;
        yield return new WaitForSecondsRealtime(.1f);
        Time.timeScale = 1;
    }

    IEnumerator DownBreak(float vely, float timeLong)
    {
        yield return new WaitForSecondsRealtime(timeLong);
        Time.timeScale = 1;
        velocity.y = Mathf.Abs(vely) * .4f * (downed > 0 ? Mathf.Pow(.9f, downed) : 1); NextState("HITF");
        downed++; downbreak = false;
    }

    IEnumerator TimeStop(float timeLong)
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
        audioSource = GetComponentInChildren<AudioSource>();
        GameObject.Find("TwoCharCam").GetComponent<CinemachineTargetGroup>().AddMember(transform, 1, 0);
        //transform.localScale = new Vector3(direction, 1, 1);
    }

    void FixedUpdate()
    {
        if (opponent == null)
        {
            if (pc.name == "Player1")
                opponent = GameObject.Find("Player2").GetComponentInChildren<ActionSystem>();
            else if (pc.name == "Player2")
                opponent = GameObject.Find("Player1").GetComponentInChildren<ActionSystem>();
        }
        else
        {
            ActionEvent();
            ValueEvent();
        }
    }

    void Update()
    {
        ComMessage();
    }
}
