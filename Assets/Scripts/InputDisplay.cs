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
            mChar[1] = mChar[0] == '1' ? '¡ú' :
            mChar[0] == '2' ? '¡õ' :
            mChar[0] == '3' ? '¡û' :
            mChar[0] == '4' ? '¡ö' :
            mChar[0] == '6' ? '¡÷' :
            mChar[0] == '7' ? '¡ø' :
            mChar[0] == '8' ? '¡ô' :
            mChar[0] == '9' ? '¡ù' : '¡¹';
            moveInput = mChar[1].ToString() + moveInput;
        }
        if (player1.actionKey != aChar[0] && player1.actionKey != ' ')
        {
            aChar[0] = player1.actionKey;
            aChar[1] = aChar[0] == 'm' ? '¢õ' :
            aChar[0] == 'w' ? '£@' :
            aChar[0] == 'g' ? '¢ï' :
            aChar[0] == 's' ? '¢û' :
            aChar[0] == 'M' ? '¢Û' :
            aChar[0] == 'W' ? '¢å' :
            aChar[0] == 'G' ? '¢Õ' : '¢û';
            moveInput = aChar[1].ToString() + moveInput;
        }
        if (moveInput.Length > 20)
            moveInput = moveInput.Remove(moveInput.Length - 1, 1);
        moveText.text = moveInput;
    }
}


