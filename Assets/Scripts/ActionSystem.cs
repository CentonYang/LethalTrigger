using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ActionSystem : MonoBehaviour
{
    [HideInInspector] public PlayerController controller;
    public ActionSystem opponent;
    [HideInInspector] public Rigidbody2D rgBody;
    [HideInInspector] public MoveList moveList;
    [HideInInspector] public Animator animator;
    [HideInInspector] public string actionMsg;
    [HideInInspector] public int direction;

    public enum MoveMode { move, jump, all };
    [Tooltip("移動模式:移動/跳躍/全部")] public MoveMode moveMode;
    public enum DirectionMode { noTurn_noCtrl, noTurn_ctrl, turn_ctrl }
    [Tooltip("轉身與方向控制:不轉身且不控制/不轉身但控制/轉身並控制")] public DirectionMode drtMode;
    [Tooltip("空中動作")] public bool airAction;
    [Tooltip("可取消動作")] public bool cancel;
    [Tooltip("可取消的動作")] public List<string> cancelList;
    [Tooltip("可取消但接續第一個動作")] public List<string> cancelOtherList;
    [Tooltip("跳躍高度")] public float jumpHeight;
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

    public void ActionMessage(List<string> atName, int drt)
    {
        //print(atName);
        direction = drt;
        for (int i = 0; i < atName.Count; i++)
        {
            if (cancelList != null && cancel)
            {
                for (int j = 0; j < cancelList.Count; j++)
                    if (atName[i] == cancelList[j])
                    {
                        cancelList.Clear();
                        animator.CrossFadeInFixedTime(atName[i], 0.05f);
                        return;
                    }
            }
            if (cancelOtherList != null && cancel)
            {
                for (int j = 1; j < cancelOtherList.Count; j++)
                    if (atName[i] == cancelOtherList[j])
                    {
                        animator.CrossFadeInFixedTime(cancelOtherList[0], 0.05f);
                        cancelOtherList.Clear();
                        return;
                    }
            }
        }
    }

    public void ActionEvent()
    {
        if (drtMode == DirectionMode.turn_ctrl) transform.localScale = new Vector3(1, 1, direction);
        switch (moveMode)
        {
            case MoveMode.all:
                rgBody.velocity = (Vector2.right * moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.z) + Vector2.up * jumpHeight) * timeScaleRate;
                break;
            case MoveMode.move:
                rgBody.velocity = (Vector2.right * moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.z) + Vector2.up * rgBody.velocity.y) * timeScaleRate;
                break;
            case MoveMode.jump:
                rgBody.velocity = (Vector2.right * rgBody.velocity.x * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.z) + Vector2.up * jumpHeight) * timeScaleRate;
                break;
        }
        if ((opponent.transform.position.x - transform.position.x < -5 && rgBody.velocity.x > 0) || (opponent.transform.position.x - transform.position.x > 5 && rgBody.velocity.x < 0))
            rgBody.position -= Vector2.right * Time.deltaTime * rgBody.velocity.x;
        if (timeScale)
            foreach (Animator anims in FindObjectsOfType<Animator>())
                anims.SetFloat("scale", timeScaleRate);
    }

    public void Canceler(string canceler)
    {
        cancelList.Clear();
        cancelList.AddRange(canceler.Split(','));
        controller.TransformOutput(controller.moveKey[0]);
        controller.TransformOutput(controller.moveKey[1]);
    }

    public void CancelOther(string canceler)
    {
        cancelOtherList.Clear();
        cancelOtherList.AddRange(canceler.Split(','));
        controller.TransformOutput(controller.moveKey[0]);
        controller.TransformOutput(controller.moveKey[1]);
    }

    public void NextState(string animState)
    {
        cancelOtherList.Clear();
        animator.CrossFadeInFixedTime(animState, 0.05f);
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
        if (!timeScale)
            timeScaleRate = animator.GetFloat("scale");
        ActionEvent();
    }
}
