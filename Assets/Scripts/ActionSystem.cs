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
    [HideInInspector] public bool hited;
    [HideInInspector] public GameObject hitboxes;

    public enum MoveMode { move, soar, all, none };
    [Tooltip("仮以社Α:仮以/�}�B/��魁/�L鋸")] public MoveMode moveMode;
    public enum DirectionMode { noTurn_noCtrl, noTurn_ctrl, turn_ctrl }
    [Tooltip("退┃�Pよ�V臼��:ぃ退┃�Bぃ臼��/ぃ退┃��臼��/退┃�単烏�")] public DirectionMode drtMode;
    [Tooltip("�鼎ぐ払@")] public bool airAction;
    [Tooltip("�i����以�@")] public bool cancel;
    [Tooltip("�i�����紺払@")] public List<string> cancelList;
    [Tooltip("�i������欝挺可�@�唹払@")] public List<string> cancelOtherList;
    [Tooltip("�B�徹���")] public float soarHeight;
    [Tooltip("仮以�t��")] public float moveSpeed;
    [Tooltip("�魯流俵〜Y��")] public bool timeScale;
    [Tooltip("�俵〜Y�餡v")] public float timeScaleRate;
    [Tooltip("ю聖極�`")] public float dmg;
    [Tooltip("�陲O�閥邦`")] public float vitDmg;
    [Tooltip("聖い�陲O�閥邦`")] public float hitVitDmg;
    [Tooltip("�曩��陲O�鳩l��")] public float vitLost;
    [Tooltip("極�`�廛�")] public float fixRate;
    [Tooltip("侍�舟俵�")] public float stiffDuration;
    [Tooltip("┥�m侍�舟俵�")] public float defStiffDuration;
    [Tooltip("聖�鍵���")] public float hitHeight;
    [Tooltip("聖�h�Z託")] public float hitDistance;
    [Tooltip("�鼎だ鮫鍵���")] public float airHitHeight;
    [Tooltip("�鼎だ三h�Z託")] public float airHitDistance;
    [Tooltip("ю聖�胞a刻も")] public bool hitGround;
    [Tooltip("�T�w/щ�Y�昿�")] public bool follow;
    public enum HitPoint { high, low };
    [Tooltip("ю聖ゴ�I葵�C:葵/�C")] public HitPoint hitPoint;
    [Tooltip("Counter�俵Z")] public bool counterRange;
    [Tooltip("�i�_�鼎え梢m")] public bool isADef;



    public void ActionMessage(List<string> atName, int drt)
    {
        //print(atName);
        direction = drt;
        //if (!animator.IsInTransition(0))
        for (int i = 0; i < atName.Count; i++)
        {
            if (cancelList != null && cancel)
            {
                for (int j = 0; j < cancelList.Count; j++)
                    if (atName[i] == cancelList[j] && CompareState(atName[i]))
                    {
                        cancelList.Clear();
                        animator.CrossFadeInFixedTime(atName[i], 0.02f);
                        return;
                    }
            }
            if (cancelOtherList != null && cancel)
            {
                for (int j = 1; j < cancelOtherList.Count; j++)
                    if (atName[i] == cancelOtherList[j] && CompareState(atName[i]))
                    {
                        animator.CrossFadeInFixedTime(cancelOtherList[0], 0.02f);
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
            case MoveMode.move:
                rgBody.velocity = new Vector2(moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.z), rgBody.velocity.y) * timeScaleRate;
                break;
            case MoveMode.soar:
                rgBody.velocity = new Vector2(rgBody.velocity.x, soarHeight) * timeScaleRate;
                break;
            case MoveMode.all:
                rgBody.velocity = new Vector2(moveSpeed * (drtMode != DirectionMode.noTurn_noCtrl ? direction : transform.localScale.z), soarHeight) * timeScaleRate;
                break;
            case MoveMode.none:
                rgBody.velocity *= timeScaleRate;
                break;
        }
        if ((opponent.transform.position.x - transform.position.x < -5 && rgBody.velocity.x > 0) || (opponent.transform.position.x - transform.position.x > 5 && rgBody.velocity.x < 0))
            rgBody.position -= Vector2.right * Time.deltaTime * rgBody.velocity.x;
        if (timeScale)
            foreach (Animator anims in FindObjectsOfType<Animator>())
                anims.SetFloat("scale", timeScaleRate);
        if (!animator.IsInTransition(0))
        {
            if (rgBody.velocity.y < -1f)
            {
                //    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") || animator.GetCurrentAnimatorStateInfo(0).IsName("FJump"))
                //        NextState("Fall");
                //    if (animator.GetCurrentAnimatorStateInfo(0).IsName("HitFly"))
                //        NextState("Drop");
            }
            //else
            if (rgBody.position.y < 0.02f)
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Fall") || animator.GetCurrentAnimatorStateInfo(0).IsName("ADef"))
                    NextState("Land");
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Drop"))
                    NextState("Down");
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("LDrop"))
                    NextState("LDown");
            }
        }
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
        if (oppoCol == "HurtBox" && !hited)
        {
            hited = true;
            print("beat");
        }
    }

    public void Hurted()
    {

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
        if (!hitboxes.activeSelf)
            hited = false;
        ActionEvent();
    }
}
