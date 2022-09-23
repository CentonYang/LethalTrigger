using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputDisplay : MonoBehaviour
{
    public Sprite cross, button;
    public List<Image> inputBgs;
    public PlayerController player1;
    char[] mChar = { ' ', ' ' }, aChar = { ' ', ' ' };
    public char inputChar;
    public string[] moveInput = { "", "" };

    void Awake()
    {
        player1 = GameObject.Find("Player1").GetComponent<PlayerController>();
    }
    void Start()
    {

    }

    public void GetInput(char i)
    {
        switch (i)
        {
            case '1': inputChar = '��'; break;
            case '2': inputChar = '��'; break;
            case '3': inputChar = '��'; break;
            case '4': inputChar = '��'; break;
            case '6': inputChar = '��'; break;
            case '7': inputChar = '��'; break;
            case '8': inputChar = '��'; break;
            case '9': inputChar = '��'; break;
            case 'M': inputChar = '��'; break;
            case 'W': inputChar = '��'; break;
            case 'S': inputChar = '��'; break;
            case 'R': inputChar = '��'; break;
            default: inputChar = '�@'; break;
        }
        if (inputChar != '�@')
            moveInput[0] = inputChar.ToString() + moveInput[0];
    }

    void Update()
    {
        //if (player1.moveString.Length > 0 && player1.moveString[player1.moveString.Length - 1] != mChar[0])
        //{
        //    mChar[0] = player1.moveString[player1.moveString.Length - 1];
        //    switch (mChar[0])
        //    {
        //        case '1': mChar[1] = '��'; break;
        //        case '2': mChar[1] = '��'; break;
        //        case '3': mChar[1] = '��'; break;
        //        case '4': mChar[1] = '��'; break;
        //        case '6': mChar[1] = '��'; break;
        //        case '7': mChar[1] = '��'; break;
        //        case '8': mChar[1] = '��'; break;
        //        case '9': mChar[1] = '��'; break;
        //        default: mChar[1] = '�@'; break;
        //    }
        //    if (mChar[1] != '�@')
        //        moveInput[0] = mChar[1].ToString() + moveInput[0];
        //}
        //if (player1.actionKey != aChar[0] && player1.actionKey != ' ')
        //{
        //    aChar[0] = player1.actionKey;
        //    switch (aChar[0])
        //    {
        //        case 'M': aChar[1] = '��'; break;
        //        case 'W': aChar[1] = '��'; break;
        //        case 'S': aChar[1] = '��'; break;
        //        case 'R': aChar[1] = '��'; break;
        //        default: aChar[1] = '�@'; break;
        //    }
        //    if (aChar[1] != '�@')
        //        moveInput[0] = aChar[1].ToString() + moveInput[0];
        //}
        if (moveInput[0] != null && moveInput[0].Length > 20)
            moveInput[0] = moveInput[0].Substring(0, 20);
        if (moveInput[0] != moveInput[1])
        {
            for (int i = 0; i < moveInput[0].Length; i++)
            {
                foreach (Transform item in inputBgs[i].GetComponentsInChildren<Transform>())
                {
                    item.gameObject.SetActive(false);
                }
                inputBgs[i].sprite = cross;
                switch (moveInput[0][i])
                {
                    case '��': Display(i, "InputLeft"); Display(i, "InputDown"); break;
                    case '��': Display(i, "InputDown"); break;
                    case '��': Display(i, "InputRight"); Display(i, "InputDown"); break;
                    case '��': Display(i, "InputLeft"); break;
                    case '��': Display(i, "InputRight"); break;
                    case '��': Display(i, "InputLeft"); Display(i, "InputUp"); break;
                    case '��': Display(i, "InputUp"); break;
                    case '��': Display(i, "InputRight"); Display(i, "InputUp"); break;
                    case '��': inputBgs[i].sprite = button; Display(i, "InputM"); break;
                    case '��': inputBgs[i].sprite = button; Display(i, "InputW"); break;
                    case '��': inputBgs[i].sprite = button; Display(i, "InputS"); break;
                    case '��': inputBgs[i].sprite = button; Display(i, "InputR"); break;
                }
                inputBgs[i].gameObject.SetActive(true);
            }
        }
        moveInput[1] = moveInput[0];
    }

    void Display(int i, string str)
    {
        inputBgs[i].transform.Find(str).gameObject.SetActive(true);
    }
}


