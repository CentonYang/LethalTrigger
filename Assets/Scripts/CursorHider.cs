using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorHider : MonoBehaviour
{
    Vector2 mousePos = Vector2.zero;
    int wait;

    void Start()
    {
        Cursor.visible = false;
        mousePos = Mouse.current.position.ReadValue();
        wait = 60;
    }

    void FixedUpdate()
    {
        if (mousePos != Mouse.current.position.ReadValue())
        { Cursor.visible = true; mousePos = Mouse.current.position.ReadValue(); wait = 60; }
        else if (wait <= 0) { Cursor.visible = false; wait = 0; }
        else wait--;
    }
}