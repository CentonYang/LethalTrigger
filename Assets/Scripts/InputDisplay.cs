using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputDisplay : MonoBehaviour
{
    public TMP_Text moveText, actionText;
    public PlayerController player1;
    char[] mChar = { ' ', ' ' }, aChar = { ' ', ' ' };
    string moveInput;

    void Awake()
    {
        player1 = GameObject.Find("Player1").GetComponent<PlayerController>();
    }
    void Start()
    {

    }

    void Update()
    {
        if (player1.moveKey != mChar[0])
        {
            mChar[0] = player1.moveKey;
            switch (mChar[0])
            {
                case '1': mChar[1] = '¡ú'; break;
                case '2': mChar[1] = '¡õ'; break;
                case '3': mChar[1] = '¡û'; break;
                case '4': mChar[1] = '¡ö'; break;
                case '6': mChar[1] = '¡÷'; break;
                case '7': mChar[1] = '¡ø'; break;
                case '8': mChar[1] = '¡ô'; break;
                case '9': mChar[1] = '¡ù'; break;
                default: mChar[1] = '¡@'; break;
            }
            if (mChar[1] != '¡@')
                moveInput = mChar[1].ToString() + moveInput;
        }
        if (player1.actionKey != aChar[0] && player1.actionKey != ' ')
        {
            aChar[0] = player1.actionKey;
            switch (aChar[0])
            {
                case 'M': aChar[1] = '¢Û'; break;
                case 'W': aChar[1] = '¢å'; break;
                case 'S': aChar[1] = '¢á'; break;
                case 'R': aChar[1] = '¢à'; break;
                default: aChar[1] = '¡@'; break;
            }
            if (aChar[1] != '¡@')
                moveInput = aChar[1].ToString() + moveInput;
        }
        if (moveInput != null && moveInput.Length > 20)
            moveInput = moveInput.Remove(moveInput.Length - 1, 1);
        moveText.text = moveInput;
    }
}


