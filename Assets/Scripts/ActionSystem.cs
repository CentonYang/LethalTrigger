using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ActionSystem : MonoBehaviour
{
    [HideInInspector] public PlayerController controller;
    [HideInInspector] public ActionSystem opponent;
    [HideInInspector] public Rigidbody2D rgBody;
    [HideInInspector] public MoveList moveList;
    [HideInInspector] public Animator animator;
    [HideInInspector] public string actionMsg;
    [HideInInspector] public int direction;
    [HideInInspector] public bool hited, hurted, downed;
    [HideInInspector] public float stiff, offGround;
    [HideInInspector] public GameObject hitboxes;
    [HideInInspector] public Transform hitTrans;
    public GameObject hitVFX;

    public enum MoveMode { move, soar, all, none };
    [Tooltip("移動模式:移動/漂浮/全部/無視")] public MoveMode moveMode;
    public enum DirectionMode { noTurn_noCtrl, noTurn_ctrl, turn_ctrl }
    [Tooltip("轉身與方向控制:不轉身且不控制/不轉身但控制/轉身並控制")] public DirectionMode drtMode;
    [Tooltip("空中動作")] public bool airAction;
    [Tooltip("可取消動作")] public bool cancel;
    [Tooltip("可取消的動作")] public List<string> cancelList;
    [Tooltip("可取消但接續第一個動作")] public List<string> cancelOtherList;
    [Tooltip("浮空高度")] public float soarHeight;
    [Tooltip("移動速度")] public float moveSpeed;
    [Tooltip("使用時間縮放")] public bool timeScale;
    [Tooltip("時間縮放率")] public float timeScaleRate;
    [Tooltip("攻擊傷害")] public float dmg;
    [Tooltip("氣力值傷害")] public float vitDmg;
    [Tooltip("擊中氣力值傷害")] public float hitVitDmg;
    [Tooltip("自身氣力值損耗")] public float vitLost;
    [Tooltip("傷害修正")] public float fixRate;
    [Tooltip("僵直時間")] public float stiffDuration;
    [Tooltip("防禦僵直時間")] public float defStiffDuration;
    [Tooltip("擊飛高度")] public float hitHeight;
    [Tooltip("擊退距離")] public float hitDistance;
    [Tooltip("空中擊飛高度")] public float airHitHeight;
    [Tooltip("空中擊退距離")] public float airHitDistance;
    [Tooltip("攻擊倒地對手")] public bool hitGround;
    [Tooltip("固定/投擲招式")] public bool follow;
    public enum HitPoint { high, low };
    [Tooltip("攻擊打點高低:高/低")] public HitPoint hitPoint;
    [Tooltip("Counter時距")] public bool counterRange;
    [Tooltip("可否空中防禦")] public bool isADef;



    public void ActionMessage(string atName, int drt)
    {
        //foreach (var at in atName) print(at);
        direction = drt;
        if (cancelList != null && cancel)
        {
            for (int j = 0; j < cancelList.Count; j++)
                if (atName == cancelList[j] && CompareState(atName))
                {
                    cancelList.Clear(); cancelOtherList.Clear();
                    animator.CrossFadeInFixedTime(atName, 0.01f);
                    //return;
                }
        }
        if (cancelOtherList != null && cancel)
        {
            for (int j = 1; j < cancelOtherList.Count; j++)
                if (atName == cancelOtherList[j] && CompareState(atName))
                {
                    cancelList.Clear();
                    animator.CrossFadeInFixedTime(cancelOtherList[0], 0.01f);
                    cancelOtherList.Clear();
                    //return;
                }
        }
    }

    public void ActionEvent()
    {
        if (drtMode == DirectionMode.turn_ctrl) transform.localScale = new Vector3(1, 1, direction);
        switch (moveMode)
        {
            case MoveMode.move:
                rgBody.velocity = new Vector2(moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.z), rgBody.velocity.y);
                break;
            case MoveMode.soar:
                rgBody.velocity = new Vector2(rgBody.velocity.x, soarHeight);
                break;
            case MoveMode.all:
                rgBody.velocity = new Vector2(moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.z), soarHeight);
                break;
        }
        if ((opponent.transform.position.x - transform.position.x < -5 && rgBody.velocity.x > 0) || (opponent.transform.position.x - transform.position.x > 5 && rgBody.velocity.x < 0))
            rgBody.position -= Vector2.right * Time.deltaTime * rgBody.velocity.x;
        if (!animator.IsInTransition(0))
        {
            if (rgBody.velocity.y < -1f)
            {
                //    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") || animator.GetCurrentAnimatorStateInfo(0).IsName("FJump"))
                //        NextState("Fall");
                //    if (animator.GetCurrentAnimatorStateInfo(0).IsName("HitFly"))
                //        NextState("Drop");
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("HitFly"))
                    NextState("Drop");
            }
            //else
            if (rgBody.velocity.y == 0 && rgBody.position.y < 0.02f && offGround <= 0)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fall") || animator.GetCurrentAnimatorStateInfo(0).IsName("ADef"))
                    NextState("Land");
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
    }

    public void Canceler(string canceler)
    {
        cancelList.Clear();
        cancelList.AddRange(canceler.Split(','));
        controller.actionMsg.Add(controller.storageName);
        //controller.TransformOutput(controller.moveKey[0]);
        //controller.TransformOutput(controller.moveKey[1]);
    }

    public void CancelOther(string canceler)
    {
        cancelOtherList.Clear();
        cancelOtherList.AddRange(canceler.Split(','));
        controller.actionMsg.Add(controller.storageName);
        //controller.TransformOutput(controller.moveKey[0]);
        //controller.TransformOutput(controller.moveKey[1]);
    }

    public void NextState(string animState)
    {
        cancelList.Clear(); cancelOtherList.Clear();
        animator.CrossFadeInFixedTime(animState, 0.01f);
        //controller.actionMsg.Add(controller.storageName);
        //controller.TransformOutput(controller.moveKey[0]);
        //controller.TransformOutput(controller.moveKey[1]);
    }

    public void Jump(float force)
    {
        rgBody.AddForce(new Vector2(0, force), ForceMode2D.Impulse);
    }

    public void push(float force)
    {
        rgBody.AddForce(new Vector2(force * transform.localScale.z, 0), ForceMode2D.Impulse);
    }

    public void SetADef(int set)
    {
        isADef = set == 0 ? false : true;
    }

    public void SetDown(int set)
    {
        downed = set == 0 ? false : true;
    }

    bool CompareState(string state)
    {
        if (state == "ADef")
            if (isADef) return true;
            else return false;
        else
            return true;
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
        bool isAir = airAction;
        if (_hitHigh) //是打點高
            if (_hitH != 0 || airAction) //是打飛招式或在空中
                if (_hitH > 0) //往上打
                    NextState("HitFly");
                else //往下打
                    NextState("Drop");
            else //不是打飛招式也不是在空中
            if ((opponent.rgBody.position.x - rgBody.position.x) * transform.localScale.z > 0) //面對對手
                NextState("UStiff");
            else //背對對手
                NextState("DStiff");
        else //是打點低
        {
            if (_hitH != 0 || airAction) //是打飛招式或在空中
                NextState("LDrop");
            else //不是打飛招式
            if ((opponent.rgBody.position.x - rgBody.position.x) * transform.localScale.z > 0) //面對對手
                NextState("DStiff");
            else //背對對手
                NextState("UStiff");
        }
        rgBody.velocity = Vector2.zero;
        Time.timeScale = 0.01f;
        stiff = _stiffDur;
        offGround = 3;
        //print("hurt!");
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1;
        if (isAir)
            rgBody.AddForce(new Vector2(_aHitD * (opponent.rgBody.position.x - rgBody.position.x < 0 ? 1 : -1), _aHitH), ForceMode2D.Impulse);
        else
            rgBody.AddForce(new Vector2(_hitD * (opponent.rgBody.position.x - rgBody.position.x < 0 ? 1 : -1), _hitH), ForceMode2D.Impulse);
    }

    void Awake()
    {
        controller = transform.parent.GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
        rgBody = GetComponent<Rigidbody2D>();
        moveList = GetComponent<MoveList>();
        GameObject.Find("TwoCharCam").GetComponent<CinemachineTargetGroup>().AddMember(transform, 1, 0);
        if (controller.name == "Player1")
        { opponent = GameObject.Find("Player2").GetComponentInChildren<ActionSystem>(); direction = 1; }
        else if (controller.name == "Player2")
        { opponent = GameObject.Find("Player1").GetComponentInChildren<ActionSystem>(); direction = -1; }
        transform.localScale = new Vector3(1, 1, direction);
    }

    void Start()
    {

    }

    void Update()
    {
        if (!hitboxes.activeSelf)
            hited = false;
        ActionEvent();
    }
}
