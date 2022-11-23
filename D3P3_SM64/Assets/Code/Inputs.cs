using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    public static Action OnRun;
    public static Action OnEndRun;
    public static Action OnJump;
    public static Action OnEndJump;
    public static Action OnJumpDown;
    public static Action<float, float> OnMove;



    public void Move(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);

    }

    public void Jump(InputAction.CallbackContext context)
    {

        if (context.performed)
        {

            OnJump?.Invoke();
        }

        if (context.canceled)
        {

            OnEndJump?.Invoke();
        }
    }

    public void Run(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnRun?.Invoke();
        }

        if (context.canceled)
        {
            OnEndRun?.Invoke();
        }
    }

    public void JumpDown(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJumpDown?.Invoke();
        }

       
    }
}
