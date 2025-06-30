using UnityEngine;

public class TouchInputState
{
    public bool IsTouching { get; private set; }
    public Vector2 TouchStart { get; private set; }
    public Vector2 TouchEnd { get; private set; }
    public float StartTime { get; private set; }
    public float HoldDuration => Time.time - StartTime;

    public void StartTouch(Vector2 start)
    {
        IsTouching = true;
        TouchStart = start;
        StartTime = Time.time;
    }

    public void EndTouch(Vector2 end)
    {
        IsTouching = false;
        TouchEnd = end;
    }
}
