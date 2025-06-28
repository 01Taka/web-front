using UnityEngine;

public class VolleyQueue
{
    private const int MaxVolleyShots = 10;
    private int queuedShots = 0;

    public int QueuedShots => queuedShots;
    public bool HasQueuedShots => queuedShots > 0;

    public void QueueShot()
    {
        queuedShots = Mathf.Min(queuedShots + 1, MaxVolleyShots);
    }

    public bool ShouldFireVolley(float swipeLength, float threshold)
    {
        return queuedShots > 0 && swipeLength > threshold;
    }

    public int Consume()
    {
        int result = queuedShots;
        queuedShots = 0;
        return result;
    }
}