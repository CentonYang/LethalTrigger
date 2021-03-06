using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public int[] moveTimer = { 0, 10 };
    public int movesNum, diraction = 1;
    public char actionKey;
    public List<string> actionName, actionStep, actionMsg;
    public GameSystem gameSystem;
    public string moveString, comString;
    public string[] movesName;

    void Awake()
    {
        Time.timeScale = 1;
        //moveList = GetComponentInChildren<MoveList>();
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
    }

    public void GameMode()
    {
        if (moveString.Length > 1)
        {
            moveTimer[0]++;
            if (moveTimer[1] > 6)
            {
                moveTimer[1] = 0;
                moveString = moveString.Substring(1);
            }
        }
        else
            moveTimer[0] = 0;
        if (moveTimer[0] > 20)
            moveTimer[1]++;
    }

    public void InputMove(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started)
        {
            float angle = Mathf.Atan2(ctx.ReadValue<Vector2>().y, ctx.ReadValue<Vector2>().x) * Mathf.Rad2Deg;
            movesNum = ctx.ReadValue<Vector2>().x == 0 && ctx.ReadValue<Vector2>().y == 0 ? 5 :
                InRange(-22.5f, 22.5f, angle) ? 6 :
                InRange(22.5f, 67.5f, angle) ? 9 :
                InRange(67.5f, 112.5f, angle) ? 8 :
                InRange(112.5f, 157.5f, angle) ? 7 :
                InRange(-157.5f, -112.5f, angle) ? 1 :
                InRange(-112.5f, -67.5f, angle) ? 2 :
                InRange(-67.5f, -22.5f, angle) ? 3 : 4;
            if (moveString.Length == 0 || moveString[moveString.Length - 1].ToString() != movesNum.ToString())
            {
                switch (movesNum)
                {
                    case 3: case 6: case 9: diraction = 1; break;
                    case 1: case 4: case 7: diraction = -1; break;
                }
                moveString += movesNum;
                if (moveString.Length > 6)
                    moveString = moveString.Substring(1);
                comString = ConvertMoves(moveString, comString);
            }
        }
    }

    public void InputAction(InputAction.CallbackContext ctx)
    {
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
            moveString = movesNum.ToString();
        }
        if (ctx.action.name == "Start")
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    bool InRange(float min, float max, float v)
    {
        return v > min && v <= max;
    }

    string ConvertMoves(string str, string cto)
    {
        char spliter = ',';
        if (str.Length > 0)
            for (int i = 0; i < movesName.Length; i++)
                for (int j = 1; j < movesName[i].Split(spliter).Length; j++)
                    if (str.Contains(movesName[i].Split(spliter)[j]))
                        return movesName[i].Split(spliter)[0];
        return cto;
    }
}