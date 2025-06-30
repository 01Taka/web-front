using UnityEngine;
using System;

public class BetterTouchHandler : MonoBehaviour
{
    public event Action<Vector2> OnTouchStartEvent;
    public event Action<Vector2> OnTouchEndEvent;
    public event Action<Vector2> OnTouchMoveEvent;

    private InputActions controls;
    private bool isTouching = false;

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

    void Update()
    {
        if (!isTouching) return;

        Vector2 pos = controls.Touch.TouchPos.ReadValue<Vector2>();
        OnTouchMoveEvent?.Invoke(pos);
    }

    void OnTouchStart()
    {
        isTouching = true;
        Vector2 pos = controls.Touch.TouchPos.ReadValue<Vector2>();
        OnTouchStartEvent?.Invoke(pos);
    }

    void OnTouchEnd()
    {
        isTouching = false;
        Vector2 pos = controls.Touch.TouchPos.ReadValue<Vector2>();
        OnTouchEndEvent?.Invoke(pos);
    }
}
