using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputDisplay : MonoBehaviour
{
    public Sprite cross, button;
    public List<Image> inputBgs;
    char[] mChar = { ' ', ' ' }, aChar = { ' ', ' ' };
    public char inputChar;
    public string[] moveInput = { "", "" };

    void Awake()
    {

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


