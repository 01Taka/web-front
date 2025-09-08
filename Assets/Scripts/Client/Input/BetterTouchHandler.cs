using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class BetterTouchHandler : MonoBehaviour
{
    public event Action<Vector2> OnTouchStartEvent;
    public event Action<Vector2> OnTouchEndEvent;
    public event Action<Vector2> OnTouchMoveEvent;

    private InputActions controls;
    private bool isTouching = false;

    private Action<InputAction.CallbackContext> touchStarted;
    private Action<InputAction.CallbackContext> touchEnded;
    private Action<InputAction.CallbackContext> touchMoved;

    // ���t���[���d���h�~�p
    private int lastTouchFrame = -1;

    void Awake()
    {
        controls = new InputActions();
    }

    void OnEnable()
    {
        touchStarted = ctx => HandleTouchStart();
        touchEnded = ctx => HandleTouchEnd();
        touchMoved = ctx => HandleTouchMove(ctx);

        controls.Touch.TouchPress.started += touchStarted;
        controls.Touch.TouchPress.canceled += touchEnded;
        controls.Touch.TouchPos.performed += touchMoved;

        controls.Enable();
    }

    void OnDisable()
    {
        controls.Touch.TouchPress.started -= touchStarted;
        controls.Touch.TouchPress.canceled -= touchEnded;
        controls.Touch.TouchPos.performed -= touchMoved;

        controls.Disable();
    }

    private Vector2 ReadTouchPos() => controls.Touch.TouchPos.ReadValue<Vector2>();

    private void HandleTouchStart()
    {
        // �����t���[���ł̏d�����΂͖���
        if (lastTouchFrame == Time.frameCount) return;

        // Start ���Ă΂��ׂ���ԂłȂ��ꍇ�͖�����
        if (isTouching)
        {
            return;
        }

        lastTouchFrame = Time.frameCount;
        isTouching = true;
        OnTouchStartEvent?.Invoke(ReadTouchPos());
    }

    private void HandleTouchEnd()
    {
        // �����t���[���� Start ���Ă΂ꂽ�ꍇ�͖�����
        if (lastTouchFrame == Time.frameCount) return;

        // End ���Ă΂��ׂ���ԂłȂ��ꍇ�͖�����
        if (!isTouching)
        {
            return;
        }

        lastTouchFrame = Time.frameCount;
        isTouching = false;
        OnTouchEndEvent?.Invoke(ReadTouchPos());
    }

    private void HandleTouchMove(InputAction.CallbackContext ctx)
    {
        if (!isTouching) return;
        Vector2 pos = ctx.ReadValue<Vector2>();
        OnTouchMoveEvent?.Invoke(pos);
    }
}
