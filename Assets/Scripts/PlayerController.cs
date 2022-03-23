using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int[] moveTimer = { 0, 10 }, diraction = { 1, 1 };
    public char[] moveKey = { '5', '5' };
    public char actionKey;
    public MoveList moveList;
    public List<string> actionName, actionStep, actionMsg;
    public GameSystem gameSystem;
    public string storageName;

    void Awake()
    {
        moveList = GetComponentInChildren<MoveList>();
    }

    void Start()
    {

    }

    void Update()
    {
        gameSystem.gameStep += Time.deltaTime * gameSystem.fps;
        if (gameSystem.gameStep >= 1)
        {
            GameMode();
            gameSystem.gameStep--;
        }
        if (actionName.Count > 99)
        {
            actionName.RemoveRange(0, actionName.Count - 99);
            actionStep.RemoveRange(0, actionStep.Count - 99);
        }
    }

    public void GameMode()
    {
        TransformOutput(moveKey[0]);
        if (moveKey[0] != '5')
            TransformOutput(moveKey[1]);
        if (moveTimer[0] < 1)
        {
            actionName.Clear(); actionStep.Clear();
            moveTimer[0] = moveTimer[1];
        }
        if (moveTimer[0] > 0) moveTimer[0]--;
        if (actionMsg.Count > 0 && !GetComponentInChildren<ActionSystem>().animator.IsInTransition(0))
        {
            GetComponentInChildren<ActionSystem>().ActionMessage(actionMsg[0], diraction[0]);
            actionMsg.RemoveAt(0);
        }
    }

    public void InputMove(InputAction.CallbackContext ctx)
    {
        moveTimer[0] = moveTimer[1];
        if (ctx.phase != InputActionPhase.Started)
        {
            moveKey[0] =
                ctx.ReadValue<Vector2>().x < 0 && ctx.ReadValue<Vector2>().y < 0 ? '1' :
                ctx.ReadValue<Vector2>().x == 0 && ctx.ReadValue<Vector2>().y < 0 ? '2' :
                ctx.ReadValue<Vector2>().x > 0 && ctx.ReadValue<Vector2>().y < 0 ? '3' :
                ctx.ReadValue<Vector2>().x < 0 && ctx.ReadValue<Vector2>().y == 0 ? '4' :
                ctx.ReadValue<Vector2>().x > 0 && ctx.ReadValue<Vector2>().y == 0 ? '6' :
                ctx.ReadValue<Vector2>().x < 0 && ctx.ReadValue<Vector2>().y > 0 ? '7' :
                ctx.ReadValue<Vector2>().x == 0 && ctx.ReadValue<Vector2>().y > 0 ? '8' :
                ctx.ReadValue<Vector2>().x > 0 && ctx.ReadValue<Vector2>().y > 0 ? '9' : '5';
            TransformOutput(moveKey[0]);
            if (ctx.ReadValue<Vector2>().x > 0)
            { moveKey[1] = '>'; diraction[0] = 1; }
            if (ctx.ReadValue<Vector2>().x < 0)
            { moveKey[1] = '<'; diraction[0] = -1; }
            if (ctx.ReadValue<Vector2>().y > 0)
                moveKey[1] = '^';
            if (ctx.ReadValue<Vector2>().y < 0)
                moveKey[1] = 'v';
            if (moveKey[0] == '5') moveKey[1] = '5';
            TransformOutput(moveKey[1]);
        }
    }

    public void InputAction(InputAction.CallbackContext ctx)
    {
        moveTimer[0] = moveTimer[1];
        if (ctx.phase != InputActionPhase.Performed)
        {
            actionKey =
                ctx.action.name + ctx.ReadValue<float>() == "M_cls1" ? 'M' :
                ctx.action.name + ctx.ReadValue<float>() == "M_cls0" ? 'm' :
                ctx.action.name + ctx.ReadValue<float>() == "W_cls1" ? 'W' :
                ctx.action.name + ctx.ReadValue<float>() == "W_cls0" ? 'w' :
                ctx.action.name + ctx.ReadValue<float>() == "S_cls1" ? 'S' :
                ctx.action.name + ctx.ReadValue<float>() == "S_cls0" ? 's' :
                ctx.action.name + ctx.ReadValue<float>() == "R_cls1" ? 'R' : 'r';
        }
        TransformOutput(actionKey);
    }

    public void TransformOutput(char compareKey)
    {
        for (int i = 0; i < moveList.data.Length; i++)
        {
            if (!(actionName.Contains(moveList.data[i].name) && actionStep.Contains(moveList.data[i].step)))
                if (moveList.data[i].step[0] == compareKey)
                {
                    //moveTimer[0] = moveTimer[1];
                    actionName.Add(moveList.data[i].name);
                    actionStep.Add(moveList.data[i].step.Substring(1));
                }
        }
        for (int i = 0; i < actionName.Count; i++)
        {
            if (actionStep[i].Length > 0 && compareKey == actionStep[i][0])
                actionStep[i] = actionStep[i].Remove(0, 1);
            if (actionStep[i].Length < 1)
            {
                if (actionName[i] != storageName)
                    if (!actionMsg.Contains(actionName[i]) || diraction[0] != diraction[1])
                    {
                        storageName = actionName[i];
                        actionMsg.Add(actionName[i]);
                        diraction[1] = diraction[0];
                        print(actionName[i]);
                    }
                actionName.RemoveAt(i); actionStep.RemoveAt(i); i--;
            }
        }
        //print(compareKey);
    }
}