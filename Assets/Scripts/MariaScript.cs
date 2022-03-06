using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MariaScript : MonoBehaviour
{
    public GameObject controller, opponent;
    public Rigidbody2D rgBody;
    public MoveList moveList;
    public Animator animator;
    public string actionMsg;
    public int direction;
    public List<string> actionName;
    public float jumpHight, moveSpeed, motionScale;

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
        for (int i = 0; i < moveList.data.Length; i++)
        {
            actionName.Add(moveList.data[i].name);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (rgBody.velocity.y == 0) animator.SetBool("fall", false);
        if (opponent.transform.position.x - transform.position.x < -5)
            if (rgBody.velocity.x > 0)
                rgBody.velocity = -Vector2.right;
        if (opponent.transform.position.x - transform.position.x > 5)
            if (rgBody.velocity.x < 0)
                rgBody.velocity = Vector2.right;
        motionScale = animator.GetFloat("scale");
    }

    public void GameMode()
    {

    }

    void ActionEvent()
    {
        switch (actionMsg) //ground: 0 == idle, -1 == def, 1 == walk, 2 == run, 3 == roll, 4 == jump, -2 == dodge
        {
            case "idle":
                animator.SetInteger("ground", 0); break;
            case "def":
                animator.SetInteger("ground", -1); break;
            case "Rdef":
                animator.SetInteger("ground", -1); direction = 1; break;
            case "Ldef":
                animator.SetInteger("ground", -1); direction = -1; break;
            case "Rwalk":
                animator.SetInteger("ground", 1); direction = 1; break;
            case "Lwalk":
                animator.SetInteger("ground", 1); direction = -1; break;
            case "Rrun":
                animator.SetInteger("ground", 2); direction = 1; break;
            case "Lrun":
                animator.SetInteger("ground", 2); direction = -1; break;
            case "Rroll":
                animator.SetInteger("ground", 3); direction = 1; break;
            case "Lroll":
                animator.SetInteger("ground", 3); direction = -1; break;
            case "dodge":
                animator.SetInteger("ground", -2); break;
            case "jump":
                animator.SetInteger("ground", 4); break;
            case "Rjump":
                animator.SetInteger("ground", 4); direction = 1; break;
            case "Ljump":
                animator.SetInteger("ground", 4); direction = -1; break;
            case "M":
                animator.SetBool("M", true); break;
            case "W":
                animator.SetBool("W", true); break;
            case "m":
                animator.SetBool("M", false); break;
            case "w":
                animator.SetBool("W", false); break;
        }
    }

    public void ActionMessage(string actionName)
    {
        actionMsg = actionName;
        ActionEvent();
        print(actionMsg);
    }

    public void SetEvent(string evt)
    {
        //print(evt);
        switch (evt)
        {
            case "idle":
                Moving(true, 0);
                break;
            case "def":
                Moving(true, 2);
                break;
            case "walk":
                Moving(false, 2);
                break;
            case "run":
                Moving(false, 2);
                break;
            case "runstop":
                Moving(true, 0);
                break;
            case "roll":
                Physics2D.IgnoreLayerCollision(3, 3, true);
                animator.SetInteger("ground", 0);
                Moving(false, 2);
                break;
            case "dodge":
                animator.SetInteger("ground", 0);
                Moving(false, 0);
                break;
            case "jump":
                Moving(true, 0, 0.1f); ;
                if (actionMsg == "Rjump" || actionMsg == "Ljump")
                {
                    Moving(false, 1);
                }
                else Moving(false, 0, 0); ;
                break;
            case "fall":
                animator.SetBool("fall", true);
                Physics2D.IgnoreLayerCollision(3, 3, true); break;
            case "land":
                Physics2D.IgnoreLayerCollision(3, 3, false);
                break;
            case "fjump":
                Physics2D.IgnoreLayerCollision(3, 3, true);
                transform.localScale = new Vector3(1, 1, direction);
                rgBody.velocity *= 0;
                rgBody.AddForce(Vector2.up * jumpHight, ForceMode2D.Impulse);
                rgBody.velocity += Vector2.right * moveSpeed * direction;
                break;
            case "hm":
                rgBody.velocity = Vector2.right * moveSpeed * transform.localScale.z;
                animator.SetBool("C", false); break;
            case "w":
                rgBody.velocity = Vector2.right * moveSpeed * transform.localScale.z;
                break;
            case "w2":
                rgBody.velocity = Vector2.right * moveSpeed * transform.localScale.z;
                animator.SetBool("W", false);
                break;
            case "m2":
                rgBody.velocity = Vector2.right * moveSpeed * transform.localScale.z;
                animator.SetBool("C", false);
                break;
            case "2w":
                rgBody.velocity = Vector2.right * moveSpeed * transform.localScale.z;
                animator.SetBool("W", false); break;
            case "art":
                rgBody.velocity = Vector2.right * moveSpeed * transform.localScale.z;
                break;
        }
    }

    public void Moving(bool stop, int directionMode, float moveScale = 1, float jumpScale = 1) //directionMode.0 = 不轉身且方向不可控, 1 = 不轉身但方向可控, 2 = 轉身且方向可控
    {
        if (directionMode == 2) transform.localScale = new Vector3(1, 1, direction);
        if (stop)
            rgBody.velocity *= Vector2.right * moveSpeed * moveScale * (directionMode > 0 ? direction : transform.localScale.z) * motionScale;
        else
            rgBody.velocity = (Vector2.right * moveSpeed * moveScale * (directionMode > 0 ? direction : transform.localScale.z)) + (Vector2.up * jumpHight * jumpScale) * motionScale;
    }

    void ResetAllBool() //重置所有Bool
    {
        foreach (var param in animator.parameters)
            if (param.type == AnimatorControllerParameterType.Bool && param.name != "delay")
                animator.SetBool(param.name, false);
    }

    void IsDelay(int setDelay)
    {
        if (setDelay == 0)
            animator.SetBool("delay", true);
        else
            animator.SetBool("delay", false);
    }

    void TimeSlow(float scale)
    {
        foreach (var i in FindObjectsOfType<Animator>())
        {
            i.SetFloat("scale", scale);
        }
    }
}
