using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Selecter : MonoBehaviour
{
    public CharacterSelect characterSelect;
    public int pc;
    public bool swap;
    public float timeStep;

    void Start()
    {
        pc = gameObject.name == "Player1" ? 0 : 1;
        PlayerController.InstallDevices(gameObject, pc);
    }

    void FixedUpdate()
    {
        if (timeStep > 0) timeStep--;
    }

    public void SelectMotion(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<Vector2>().x < -.25f && ctx.ReadValue<Vector2>().y < .25f && ctx.ReadValue<Vector2>().y > -.25f)
            characterSelect.arrowX[swap ? 1 - pc : pc] = -1;
        else if (ctx.ReadValue<Vector2>().x > .25f && ctx.ReadValue<Vector2>().y < .25f && ctx.ReadValue<Vector2>().y > -.25f)
            characterSelect.arrowX[swap ? 1 - pc : pc] = 1;
        else if (ctx.ReadValue<Vector2>().y < -.25f && ctx.ReadValue<Vector2>().x < .25f && ctx.ReadValue<Vector2>().x > -.25f)
            characterSelect.arrowY[swap ? 1 - pc : pc] = -1;
        else if (ctx.ReadValue<Vector2>().y > .25f && ctx.ReadValue<Vector2>().x < .25f && ctx.ReadValue<Vector2>().x > -.25f)
            characterSelect.arrowY[swap ? 1 - pc : pc] = 1;
        else { characterSelect.arrowX[swap ? 1 - pc : pc] = 0; characterSelect.arrowY[swap ? 1 - pc : pc] = 0; }
    }

    public void SelectAction(InputAction.CallbackContext ctx)
    {
        if (ctx.phase != InputActionPhase.Performed)
        {
            if (ctx.action.name + ctx.ReadValue<float>() == "W_cls1" && timeStep <= 0)
            {
                switch (characterSelect.layer[swap ? 1 - pc : pc])
                {
                    case 0: characterSelect.layer[swap ? 1 - pc : pc] = 1; break;
                    case 1: characterSelect.layer[swap ? 1 - pc : pc] = 2; break;
                }
            }
            if (ctx.action.name + ctx.ReadValue<float>() == "R_cls1" && timeStep <= 0)
            {
                switch (characterSelect.layer[swap ? 1 - pc : pc])
                {
                    case 0: characterSelect.layer[swap ? 1 - pc : pc] = -1; break;
                    case 1: characterSelect.layer[swap ? 1 - pc : pc] = 0; break;
                }
            }
            if (ctx.action.name + ctx.ReadValue<float>() == "S_cls1")
                swap = true;
            if (ctx.action.name + ctx.ReadValue<float>() == "S_cls0")
                swap = false;
            if (ctx.action.name == "Start")
            {
                switch (characterSelect.layer[pc])
                { case 0: characterSelect.layer[pc] = 4; break; }
            }
        }
    }
}
