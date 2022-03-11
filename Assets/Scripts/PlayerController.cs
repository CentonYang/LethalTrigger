using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int[] moveTimer = { 0, 10 }, diraction = { 1, 1 };
    public char moveKey, actionKey;
    public List<char> inputMove, inputAction;
    public MoveList moveList;
    public List<string> actionName, actionStep;
    public string actionMsg;
    public GameSystem gameSystem;

    void Awake()
    {
        moveList = GetComponentInChildren<MoveList>();
    }

    void Start()
    {
        TransformOutput('5');
    }

    void Update()
    {
        gameSystem.gameStep += Time.deltaTime * gameSystem.fps;
        if (gameSystem.gameStep >= 1)
        {
            GameMode();
            gameSystem.gameStep--;
        }
        if (inputMove.Count > 0)
        {
            moveKey = inputMove[0];
            TransformOutput(moveKey);
            inputMove.RemoveAt(0);
        }
        if (inputAction.Count > 0)
        {
            actionKey = inputAction[0];
            TransformOutput(actionKey);
            inputAction.RemoveAt(0);
        }
    }

    public void GameMode()
    {
        if (moveTimer[0] < 1)
        {
            actionName.Clear(); actionStep.Clear();
        }
        if (moveTimer[0] > 0) moveTimer[0]--;
        TransformOutput(moveKey);
    }

    public void InputMove(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started)
        {
            inputMove.Add(
                ctx.ReadValue<Vector2>().x < 0 && ctx.ReadValue<Vector2>().y < 0 ? '1' :
                ctx.ReadValue<Vector2>().x == 0 && ctx.ReadValue<Vector2>().y < 0 ? '2' :
                ctx.ReadValue<Vector2>().x > 0 && ctx.ReadValue<Vector2>().y < 0 ? '3' :
                ctx.ReadValue<Vector2>().x < 0 && ctx.ReadValue<Vector2>().y == 0 ? '4' :
                ctx.ReadValue<Vector2>().x > 0 && ctx.ReadValue<Vector2>().y == 0 ? '6' :
                ctx.ReadValue<Vector2>().x < 0 && ctx.ReadValue<Vector2>().y > 0 ? '7' :
                ctx.ReadValue<Vector2>().x == 0 && ctx.ReadValue<Vector2>().y > 0 ? '8' :
                ctx.ReadValue<Vector2>().x > 0 && ctx.ReadValue<Vector2>().y > 0 ? '9' : '5');
            if (ctx.ReadValue<Vector2>().x > 0)
            { inputMove.Add('R'); diraction[0] = 1; }
            if (ctx.ReadValue<Vector2>().x < 0)
            { inputMove.Add('L'); diraction[0] = -1; }
            if (ctx.ReadValue<Vector2>().y > 0)
                inputMove.Add('U');
            if (ctx.ReadValue<Vector2>().y < 0)
                inputMove.Add('D');
        }
    }

    public void InputAction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started)
        {
            inputAction.Add(
                ctx.action.name + ctx.ReadValue<float>() == "M_cls1" ? 'M' :
                ctx.action.name + ctx.ReadValue<float>() == "M_cls0" ? 'm' :
                ctx.action.name + ctx.ReadValue<float>() == "W_cls1" ? 'W' :
                ctx.action.name + ctx.ReadValue<float>() == "W_cls0" ? 'w' :
                ctx.action.name + ctx.ReadValue<float>() == "S_cls1" ? 'S' :
                ctx.action.name + ctx.ReadValue<float>() == "S_cls0" ? 's' :
                ctx.action.name + ctx.ReadValue<float>() == "R_cls1" ? 'R' : 'r');
        }
    }

    public void TransformOutput(char compareKey)
    {
        for (int i = 0; i < moveList.data.Length; i++)
        {
            if (!actionName.Contains(moveList.data[i].name))
                if (moveList.data[i].step[0] == compareKey)
                {
                    moveTimer[0] = moveTimer[1];
                    actionName.Add(moveList.data[i].name);
                    actionStep.Add(moveList.data[i].step.Substring(1));
                }
        }
        bool pushMsg = true;
        for (int i = 0; i < actionName.Count; i++)
        {
            if (actionStep[i].Length > 0 && compareKey == actionStep[i][0])
                actionStep[i] = actionStep[i].Remove(0, 1);
            if (actionStep[i].Length < 1) //指令表過濾
            {
                if ((actionMsg != actionName[i] && pushMsg) || (diraction[0] != diraction[1]))
                {
                    actionMsg = actionName[i];
                    diraction[1] = diraction[0];
                    pushMsg = false;
                    GetComponentInChildren<ActionSystem>().ActionMessage(actionMsg, diraction[0]);
                }
                actionName.RemoveAt(i); actionStep.RemoveAt(i); i--;
            }
        }
    }
}