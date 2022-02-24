using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int[] moveTimer = { 0, 10 }; //value, max
    public char moveKey, actionKey;
    public MoveList moveList;
    public List<string> actionName, actionStep;
    public string actionMsg;
    public bool restMove;
    public GameSystem gameSystem;

    void Awake()
    {
        moveList = GetComponentInChildren<MoveList>();
    }

    void Start()
    {
        MoveTransform(moveKey, true);
    }

    void Update()
    {
        gameSystem.gameStep += Time.deltaTime * gameSystem.fps;
        if (gameSystem.gameStep >= 1)
        {
            GameMode();
            gameSystem.gameStep--;
        }
    }

    public void GameMode()
    {
        if (moveTimer[0] < 1)
        { actionName.Clear(); actionStep.Clear(); MoveTransform(moveKey, true); }
        if (moveTimer[0] > 0) moveTimer[0]--;
        //transform.GetChild(0).SendMessage("GameMode", SendMessageOptions.DontRequireReceiver);
    }

    public void InputMove(InputAction.CallbackContext ctx)
    {
        MoveTransform(
            ctx.ReadValue<Vector2>().x < 0 && ctx.ReadValue<Vector2>().y < 0 ? '1' :
            ctx.ReadValue<Vector2>().x == 0 && ctx.ReadValue<Vector2>().y < 0 ? '2' :
            ctx.ReadValue<Vector2>().x > 0 && ctx.ReadValue<Vector2>().y < 0 ? '3' :
            ctx.ReadValue<Vector2>().x < 0 && ctx.ReadValue<Vector2>().y == 0 ? '4' :
            ctx.ReadValue<Vector2>().x > 0 && ctx.ReadValue<Vector2>().y == 0 ? '6' :
            ctx.ReadValue<Vector2>().x < 0 && ctx.ReadValue<Vector2>().y > 0 ? '7' :
            ctx.ReadValue<Vector2>().x == 0 && ctx.ReadValue<Vector2>().y > 0 ? '8' :
            ctx.ReadValue<Vector2>().x > 0 && ctx.ReadValue<Vector2>().y > 0 ? '9' : '5', false
            );
    }

    public void InputAction(InputAction.CallbackContext ctx)
    {
        ActionTransform(
            ctx.action.name + ctx.ReadValue<float>() == "M_cls1" ? 'M' :
            ctx.action.name + ctx.ReadValue<float>() == "M_cls0" ? 'm' :
            ctx.action.name + ctx.ReadValue<float>() == "W_cls1" ? 'W' :
            ctx.action.name + ctx.ReadValue<float>() == "W_cls0" ? 'w' :
            ctx.action.name + ctx.ReadValue<float>() == "G_cls1" ? 'G' :
            ctx.action.name + ctx.ReadValue<float>() == "G_cls0" ? 'g' :
            ctx.action.name + ctx.ReadValue<float>() == "S_cls1" ? 'S' : 's'
            );
    }

    public void MoveTransform(char getMove, bool rest)
    {
        if (!rest)
            if (moveKey == getMove) return;
        restMove = rest;
        for (int i = 0; i < moveList.data.Length; i++)
        {
            if (!actionName.Contains(moveList.data[i].name))
                if (moveList.data[i].step[0] == getMove)  //移動鍵找指令加入actionList
                {
                    moveTimer[0] = moveTimer[1];
                    moveKey = getMove;
                    actionName.Add(moveList.data[i].name);
                    actionStep.Add(moveList.data[i].step.Substring(1));
                }
        }
        TransformOutput(moveKey);
    }

    public void ActionTransform(char getAction)
    {
        if (actionKey == getAction) return;
        actionKey = getAction; restMove = false;
        for (int i = 0; i < moveList.data.Length; i++)
        {
            if (!actionName.Contains(moveList.data[i].name))
                if (moveList.data[i].step[0] == getAction)  //動作鍵找指令加入actionList
                {
                    actionName.Add(moveList.data[i].name);
                    actionStep.Add(moveList.data[i].step.Substring(1));
                }
        }
        TransformOutput(actionKey);
    }

    public void TransformOutput(char compareKey)
    {
        for (int i = 0; i < actionName.Count; i++)
        {
            if (actionStep[i].Length > 0)
                if (compareKey == actionStep[i][0]) //指令表篩選
                    actionStep[i] = actionStep[i].Remove(0, 1);
            if (actionStep[i].Length < 1) //指令表過濾
            {
                if (actionMsg != actionName[i] && !restMove)
                {
                    actionMsg = actionName[i];
                    transform.GetChild(0).SendMessage("ActionMessage", actionMsg, SendMessageOptions.DontRequireReceiver);
                    restMove = true;
                }
                actionName.RemoveAt(i); actionStep.RemoveAt(i); i--;
            }
        }
    }
}