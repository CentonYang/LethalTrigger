using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerController : MonoBehaviour
{
    public int[] moveTimer = { 0, 10 };
    public int movesNum, diraction = 1;
    public char actionKey;
    public List<string> actionName, actionStep, actionMsg, movesName;
    public GameSystem gameSystem;
    public string moveString, comString;
    public InputDisplay id;
    public BattleMenu battleMenu;
    PlayerInput pi;


    void Awake()
    {
        Time.timeScale = 1;
        if (id == null)
            id = FindObjectOfType<InputDisplay>();
        pi = GetComponent<PlayerInput>();
        pi.user.UnpairDevices();
        if (InputSystem.devices.Count > 1)
            if (gameObject.name == "Player1")
                InputUser.PerformPairingWithDevice(InputSystem.devices[InputSystem.devices.Count - 2], pi.user);
            else
                InputUser.PerformPairingWithDevice(InputSystem.devices[InputSystem.devices.Count - 1], pi.user);
        else if (gameObject.name == "Player1")
            InputUser.PerformPairingWithDevice(InputSystem.devices[0], pi.user);
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
        //if (moveString.Length > 1)
        //{
        moveTimer[0]++;
        if (moveTimer[0] > moveTimer[1])
        {
            if (moveString.Length > 1)
                moveString = moveString[moveString.Length - 1].ToString();
            moveTimer[0] = 0;
            //comString = ConvertMoves(moveString, comString);
        }
        //}
    }

    public void InputMove(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Started && !battleMenu.gameObject.activeSelf)
        {
            moveTimer[0] = 0;
            float angle = Mathf.Atan2(ctx.ReadValue<Vector2>().y, ctx.ReadValue<Vector2>().x) * Mathf.Rad2Deg;
            int num = ctx.ReadValue<Vector2>().x == 0 && ctx.ReadValue<Vector2>().y == 0 ? 5 :
                InRange(-22.5f, 22.5f, angle) ? 6 :
                InRange(22.5f, 67.5f, angle) ? 9 :
                InRange(67.5f, 112.5f, angle) ? 8 :
                InRange(112.5f, 157.5f, angle) ? 7 :
                InRange(-157.5f, -112.5f, angle) ? 1 :
                InRange(-112.5f, -67.5f, angle) ? 2 :
                InRange(-67.5f, -22.5f, angle) ? 3 : 4;
            if (num != movesNum)
            {
                movesNum = num;
                id.GetInput(movesNum.ToString()[0]);
            }
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
        if (ctx.phase != InputActionPhase.Performed && !battleMenu.gameObject.activeSelf)
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
            id.GetInput(actionKey);
            if (ctx.action.name == "Start")
            {
                battleMenu.layer = 0; battleMenu.changeLayer = true;
                battleMenu.gameObject.SetActive(true);
            }
        }
    }

    bool InRange(float min, float max, float v)
    {
        return v > min && v <= max;
    }

    public string ConvertMoves(string str, string cto)
    {
        char spliter = ',';
        if (str.Length > 0)
            for (int i = 0; i < movesName.Count; i++)
                for (int j = 1; j < movesName[i].Split(spliter).Length; j++)
                    if (str.Contains(movesName[i].Split(spliter)[j]))
                    {
                        movesName.Add(movesName[i]);
                        movesName.RemoveAt(i);
                        if (movesName[i].Split(spliter)[0].Length > 1)
                            moveString = moveString[moveString.Length - 1].ToString();
                        return movesName[movesName.Count - 1].Split(spliter)[0];
                    }
        return cto;
    }
}