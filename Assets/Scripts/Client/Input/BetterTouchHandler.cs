using UnityEngine;
using System;

public class BetterTouchHandler : MonoBehaviour
{
    public event Action<Vector2> OnTouchStartEvent;
    public event Action<Vector2> OnTouchEndEvent;

    private InputActions controls;

    void Awake()
    {
        controls = new InputActions();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.Touch.TouchPress.started += ctx => OnTouchStart();
        controls.Touch.TouchPress.canceled += ctx => OnTouchEnd();
    }

    void OnDisable()
    {
        controls.Touch.TouchPress.started -= ctx => OnTouchStart();
        controls.Touch.TouchPress.canceled -= ctx => OnTouchEnd();
        controls.Disable();
    }

    void OnTouchStart()
    {
        Vector2 pos = controls.Touch.TouchPos.ReadValue<Vector2>();
        OnTouchStartEvent?.Invoke(pos);
    }

    void OnTouchEnd()
    {
        Vector2 pos = controls.Touch.TouchPos.ReadValue<Vector2>();
        OnTouchEndEvent?.Invoke(pos);
    }
}
