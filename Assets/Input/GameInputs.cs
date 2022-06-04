using UnityEngine;
using UnityEngine.InputSystem;

#if ENABLE_INPUT_SYSTEM
#endif

public class GameInputs : MonoBehaviour
{
    [Header("Player input values")]
    public Vector2 move;
    public Vector2 look;
    public bool hookshot;

#if !UNITY_IOS || !UNITY_ANDROID
    [Header("Mouse cursor settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }
    public void OnLook(InputValue value)
    {
        LookInput(value.Get<Vector2>());
    }
    public void OnHookshot(InputValue value)
    {
        HookshotInput(value.isPressed);
    }
#endif

    public void MoveInput(Vector2 newMoveVector)
    {
        move = newMoveVector;
    }

    public void LookInput(Vector2 newLookVector)
    {
        look = newLookVector;
    }

    public void HookshotInput(bool newValue)
    {
        hookshot = newValue;
    }


#if !UNITY_IOS || !UNITY_ANDROID
    private void OnApplicationFocus(bool focus)
    {
        SetCursorState(cursorLocked);
    }
    void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
#endif
}
