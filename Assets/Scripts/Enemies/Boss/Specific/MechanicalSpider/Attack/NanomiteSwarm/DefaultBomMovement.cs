using UnityEngine;

public interface IBomMovement
{
    void Move(Transform transform, Vector2 target, float speed);
}

public class DefaultBomMovement : IBomMovement
{
    public void Move(Transform transform, Vector2 target, float speed)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
}