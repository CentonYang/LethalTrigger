using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Selecter : MonoBehaviour
{
    public CharacterSelect characterSelect;
    public int pc;

    void Start()
    {
        pc = gameObject.name == "Player1" ? 0 : 1;
        PlayerController.InstallDevices(gameObject, pc);
    }

    void FixedUpdate()
    {

    }

    public void SelectMotion(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<Vector2>().x < -.1f)
            characterSelect.arrow[pc] = -1;
        else if (ctx.ReadValue<Vector2>().x > .1f)
            characterSelect.arrow[pc] = 1;
        else characterSelect.arrow[pc] = 0;
    }

    public void SelectAction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            if (ctx.action.name + ctx.ReadValue<float>() == "W_cls1")
            {
                switch (characterSelect.layer[pc])
                {
                    case 0: characterSelect.layer[pc] = 1; break;
                    case 1: characterSelect.layer[pc] = 2; break;
                }
            }
            if (ctx.action.name + ctx.ReadValue<float>() == "R_cls1")
            {
                switch (characterSelect.layer[pc])
                {
                    case 0: characterSelect.layer[pc] = -1; break;
                    case 1: characterSelect.layer[pc] = 0; break;
                }
            }
        }
    }
}
