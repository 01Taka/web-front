using UnityEngine;

public class EnemyAnimationOffset : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // 親オブジェクトも含めてAnimatorコンポーネントを探す
        animator = GetComponentInParent<Animator>();

        if (animator != null)
        {
            float randomTime = Random.Range(0f, 1f);
            animator.Play(0, 0, randomTime);
        }
        else
        {
            Debug.LogError("Animatorコンポーネントが見つかりませんでした。");
        }
    }
}