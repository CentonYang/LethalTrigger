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
    [HideInInspector] public int direction, hitType, life;
    [HideInInspector] public bool hited, hurted, hurtCheck, downbreak, comboOver, death;
    [HideInInspector] public float stiff, pushReac, gravity, pushDis, dirDis, combo, downed, rootScale;
    [HideInInspector] public Vector2 velocity, hurtVel;
    public Vector2 hp, sta, btr, skill, fix;
    public GameObject hitboxes;
    public Transform hitTrans, root, local;
    public GameObject hitVFX;
    [HideInInspector] public CameraShake cameraShake;
    string animName, animCheck;

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
        direction = pc.direction;
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
                    actionMsg = cancelOtherList[0];
                    cancelOtherList.Clear();
                    return;
                }
            }
    }

    string SearchAction(string target, string mStr, string cStr, string aStr)
    {
        string inStateStr = inState == 0 ? "" : inState.ToString();
        for (int i = 0; i < cStr.Length; i++)
        {
            if (target == inStateStr + cStr.Substring(i) + aStr) return target;
            if (target == cStr.Substring(i) + aStr) return target;
        }
        if (target == inStateStr + mStr + aStr) return target;
        if (target == mStr + aStr) return target;
        if (target == inStateStr + aStr) return target;
        if (target == aStr) return target;
        if (mStr.Length > 0 && target == inStateStr + mStr[mStr.Length - 1].ToString()) return target;
        if (mStr.Length > 0 && target == mStr[mStr.Length - 1].ToString()) return target;
        for (int i = 0; i < cStr.Length; i++)
        {
            if (i == cStr.Length - 1)
                if (mStr[mStr.Length - 1] == '5' && cStr != "N") return null;
                else if ((mStr[mStr.Length - 1] == '1' || mStr[mStr.Length - 1] == '2' || mStr[mStr.Length - 1] == '3') && cStr != "D") return null;
                else if ((mStr[mStr.Length - 1] == '7' || mStr[mStr.Length - 1] == '8' || mStr[mStr.Length - 1] == '9') && cStr != "U") return null;
            if (target == inStateStr + cStr.Substring(i)) return target;
            if (target == cStr.Substring(i)) return target;
        }
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
            velocity.x += dirDis * Time.fixedDeltaTime;
        }
        else dirDis = 0;
        if (actionMsg != null && (cancel || (hitCancel && hited)) /*&& acceptMsg == null*/)
        {
            foreach (MoveList.Data item in moveList.data)
            {
                if (item.name == actionMsg && item.staLost > 0)
                    if (!staCD)
                        sta.x -= item.staLost;
                    else { actionMsg = item.subName == "" ? null : item.subName; pc.actionKey = '_'; }
                if (item.name == actionMsg && item.btrLost > 0)
                    if (btr.x - item.btrLost >= 0)
                        btr.x -= item.btrLost;
                    else { actionMsg = item.subName == "" ? null : item.subName; pc.actionKey = '_'; }
            }
            if (actionMsg.Contains(pc.actionKey))
                pc.actionKey = '_';
            NextState(actionMsg);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("HITF") && velocity.y < 0)
            NextStateSelf("HITD");
        if (stiff <= 0 && pushReac <= 0 && hurted)
            if (inState == InState.HU) { hurted = false; ; NextStateSelf("5"); }
            else if (inState == InState.G) { hurted = false; NextStateSelf("D"); }
            else if (inState == InState.AHU) { hurted = false; }
        if (stiff > 0)
            stiff--;
        if (pushReac > 0)
        {
            if (pushReac > 1)
                velocity *= 0;
            else
                velocity = hurtVel;
            pushReac--;
        }
        else
        {
            velocity.y -= gravity;
            velocity.x += velocity.x < 0 ? .05f : velocity.x > 0 ? -.05f : 0;
        }
        if (cc.isGrounded && velocity.y < 0 && pushReac <= 0)
        {
            if (inState == InState.A && (moveMode == MoveMode.move || moveMode == MoveMode.none))
                NextStateSelf("AL");
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("HITD") && !downbreak)
                if (velocity.y < -20)
                {
                    downbreak = true;
                    Time.timeScale = .5f; StartCoroutine(DownBreak(velocity, .2f));
                    pushReac = 6;
                    audioSource.clip = audios[5]; Sounder sd = Instantiate(sounder, audioSource.transform);
                }
                else
                {
                    if (downed > 0 && !death) NextStateSelf("WAKE");
                    else NextStateSelf("DOWN");
                    downed++;
                }
        }
        if (drtMode == DirectionMode.turn_ctrl) { root.localScale = new Vector3(rootScale * direction, rootScale, rootScale); local.localScale = new Vector3(direction, 1, 1); }
        switch (moveMode)
        {
            case MoveMode.move:
                velocity.x = moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : local.localScale.x);
                break;
            case MoveMode.soar:
                velocity.y = soarHeight;
                break;
            case MoveMode.all:
                velocity = new Vector2(moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : local.localScale.x), soarHeight);
                break;
        }
        if (hited && pushDis != 0)
            velocity.x = -pushDis;
        float dis = transform.position.x > opponent.transform.position.x ? ((transform.position.x + radius) - (opponent.transform.position.x - opponent.radius)) :
        ((transform.position.x - radius) - (opponent.transform.position.x + opponent.radius));
        if (dis >= 5 && velocity.x > 0)
            velocity.x = 5 - dis;
        if (dis <= -5 && velocity.x < 0)
            velocity.x = -5 - dis;
        float ras = Mathf.Abs(opponent.transform.position.x - transform.position.x);
        if (ras - radius - opponent.radius < 0 && !trigger && !opponent.trigger)
        {
            if (transform.position.x < opponent.transform.position.x && velocity.x > 0)
                velocity.x /= 8;
            if (transform.position.x > opponent.transform.position.x && velocity.x < 0)
                velocity.x /= 8;
        }
        cc.Move(new Vector2(velocity.x, velocity.y < -10 ? -10 : velocity.y) * Time.fixedDeltaTime);
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

    void NextStateSelf(string animState)
    {
        if (animState == "WAKE" && death) { StartCoroutine(Recovery()); return; }
        if (animator.HasState(0, Animator.StringToHash(animState)))
        {
            cancelList.Clear(); cancelOtherList.Clear(); actionMsg = null;
            animator.CrossFadeInFixedTime(animState, 0);
        }
    }

    public void NextState(string animState)
    {
        if (stiff <= 0 || pushReac <= 0) NextStateSelf(animState);
    }

    public void Hited(string oppoCol)
    {
        if (oppoCol == "HurtBox" && !hited && !follow && !opponent.death && opponent.inState != InState.WAKE && pc.isCtrl && !(opponent.cc.isGrounded && opponent.velocity.y < 0 && opponent.downed > 1))
        {
            hited = true;
            GameObject hitVFXObj = Instantiate(hitVFX, hitTrans.position, hitTrans.rotation);
            hitVFXObj.transform.localScale = local.localScale;
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

    float s_dmg, s_staDmg, s_hitSDmg, s_btrDmg, s_hitBDmg, s_fixRate, s_stiffDur, s_dStiffDur, s_hitH, s_hitD, s_aHitH, s_aHitD;
    bool s_hitHigh;

    public IEnumerator Hurted(float _dmg, float _staDmg, float _hitSDmg, float _btrDmg, float _hitBDmg, float _fixRate, float _stiffDur, float _dStiffDur, float _hitH, float _hitD, float _aHitH, float _aHitD, bool _hitHigh)
    {
        bool isAir = inState == InState.A || inState == InState.AHU || inState == InState.DOWN ? true : false;
        pushReac = 2;
        if ((inState == InState.D || inState == InState.G) && (opponent.transform.position.x - transform.position.x) * direction > 0)
        {
            root.localScale = new Vector3(rootScale * direction, rootScale, rootScale);
            local.localScale = new Vector3(direction, 1, 1);
            audioSource.clip = audios[4]; Sounder sd = Instantiate(sounder, audioSource.transform);
            //傷害計算
            btr.x += _btrDmg;
            opponent.skill.x += _btrDmg / 2;
            sta.x += _staDmg;
            float _gHitH = 0;
            if (sta.x > 0 && !staCD)
            { stiff = _dStiffDur; if (!hitRange) NextStateSelf("G"); }
            else if (_hitH != 0) //是打飛招式或在空中
            {
                if (_hitH < 0)
                    NextStateSelf("HITD");
                else
                    NextStateSelf("HITF");
                hp.x += _dmg / 2; _gHitH = _hitH;
            }
            else
            { stiff = _stiffDur; NextStateSelf("BK"); hp.x += _dmg / 2; }
            //////
            if (hp.x <= 0)
            { StartCoroutine(Death(_dmg, _dStiffDur, _aHitD, _aHitH, _hitH)); yield break; }
            //////
            hurtVel = new Vector2(opponent.transform.position.x > transform.position.x ? -_hitD * .75f : _hitD * .75f, _gHitH);
        }
        else
        {
            audioSource.clip = opponent.hitType == 1 || opponent.hitType == 3 ? opponent.audios[2] : opponent.hitType == 2 ? opponent.audios[3] : null;
            Sounder sd = Instantiate(sounder, audioSource.transform);
            opponent.combo++;
            //傷害計算
            hp.x += _dmg * fix.x;
            fix.x *= _fixRate;
            sta.x += _hitSDmg; btr.x += _hitBDmg;
            opponent.skill.x += _hitBDmg / 2;
            //////
            if (hp.x <= 0)
            { StartCoroutine(Death(_dmg, _dStiffDur, _aHitD, _aHitH, _hitH)); yield break; }
            //////
            if (isAir)
                hurtVel = new Vector2(opponent.transform.position.x > transform.position.x ? -_aHitD : _aHitD, _aHitH);
            else
                hurtVel = new Vector2(opponent.transform.position.x > transform.position.x ? -_hitD : _hitD, _hitH);
            if (_hitH == 0)
                stiff = _stiffDur;
            if (_hitH != 0 || isAir) //是打飛招式或在空中
                if (_hitH < 0)
                    NextStateSelf("HITD");
                else
                    NextStateSelf("HITF");
            else if (_hitHigh) //是打點高
                if ((opponent.transform.position.x - transform.position.x) * local.localScale.x > 0) //面對對手
                    NextStateSelf("HUB");
                else //背對對手
                    NextStateSelf("HUF");
            else //是打點低
            {
                if ((opponent.transform.position.x - transform.position.x) * local.localScale.x > 0) //面對對手
                    NextStateSelf("HUF");
                else //背對對手
                    NextStateSelf("HUB");
            }
        }
        hurted = true;
        cameraShake.SetShake(Mathf.Abs(_dStiffDur * .5f), Mathf.Abs(_dmg * .01f));
        Time.timeScale = .5f;
        yield return new WaitForSecondsRealtime(.1f);
        if (!death) Time.timeScale = 1;
    }

    public void CameraShake(string tick_power)
    {
        cameraShake.SetShake(float.Parse(tick_power.Split(',')[0]), float.Parse(tick_power.Split(',')[1]));
    }

    IEnumerator DownBreak(Vector2 vel, float timeLong)
    {
        yield return new WaitForSecondsRealtime(timeLong);
        if (!death) Time.timeScale = 1;
        velocity.x = vel.x;
        velocity.y = Mathf.Abs(vel.y) * .4f * (downed > 0 ? Mathf.Pow(.75f, downed) : 1); NextStateSelf("HITF");
        pushReac = 0;
        downed++; downbreak = false;
    }

    IEnumerator TimeWait(float timeLong)
    {
        yield return new WaitForSecondsRealtime(timeLong);
        Time.timeScale = 1;
    }

    IEnumerator Death(float _dmg, float _dStiffDur, float _aHitD, float _aHitH, float _hitH)
    {
        death = true; pc.isCtrl = false; life--;
        CinemachineTargetGroup cineTarget = FindObjectOfType<Cinemachine.CinemachineTargetGroup>();
        for (int i = 0; i < cineTarget.m_Targets.Length; i++)
            if (!cineTarget.m_Targets[i].target.GetComponent<ActionSystem>().death)
                cineTarget.m_Targets[i].weight = 0;
        hurtVel = new Vector2(opponent.transform.position.x > transform.position.x ? -_aHitD : _aHitD, _aHitH);
        if (_hitH < 0)
            NextStateSelf("HITD");
        else
            NextStateSelf("HITF");
        cameraShake.SetShake(Mathf.Abs(_dStiffDur * .5f), Mathf.Abs(_dmg * .01f));
        Time.timeScale = .5f;
        yield return new WaitForSecondsRealtime(.1f);
        if (!death) Time.timeScale = 1;
    }
    public IEnumerator Recovery()
    {
        Time.timeScale = 1;
        opponent.combo = 0;
        CinemachineTargetGroup cineTarget = FindObjectOfType<Cinemachine.CinemachineTargetGroup>();
        for (int i = 0; i < cineTarget.m_Targets.Length; i++)
            cineTarget.m_Targets[i].weight = 1;
        yield return new WaitForSecondsRealtime(.5f);
        if (life > 0)
        {
            hp.x = hp.y;
            death = false;
            NextStateSelf("WAKE");
            yield return new WaitForSecondsRealtime(.1f);
            pc.isCtrl = true;
        }
    }

    void Start()
    {
        gravity = .5f; rootScale = root.localScale.x;
        pc = transform.parent.GetComponent<PlayerController>();
        pc = transform.parent.GetComponent<PlayerController>();
        cc = GetComponentInChildren<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        moveList = GetComponent<MoveList>();
        audioSource = GetComponentInChildren<AudioSource>();
        cameraShake = FindAnyObjectByType<CameraShake>();
        if (GameObject.Find("TwoCharCam") != null)
            GameObject.Find("TwoCharCam").GetComponent<CinemachineTargetGroup>().AddMember(transform, 1, 0);
        life = GameSystem.life;
        if (pc != null)
        {
            foreach (CharacterColor item in GetComponentsInChildren<CharacterColor>())
                item.ColorChange(pc.pc);
            direction = pc.direction;
            if (opponent == null)
            {
                if (pc.name == "Player1")
                    opponent = GameObject.Find("Player2").GetComponentInChildren<ActionSystem>();
                else if (pc.name == "Player2")
                    opponent = GameObject.Find("Player1").GetComponentInChildren<ActionSystem>();
            }
        }
    }

    void FixedUpdate()
    {
        if (pc != null)
        {
            ActionEvent();
            ValueEvent();
        }
    }

    void Update()
    {
        if (pc == null) return;
        if ((inState == InState.WAKE || inState == InState.N) && pushReac <= 0)
        { downed = 0; opponent.combo = 0; fix.x = fix.y; }
        if ((pc.pc == 0 && GameSystem.p1Comp) || (pc.pc == 1 && GameSystem.p2Comp))
            ComMessage();
    }
}
