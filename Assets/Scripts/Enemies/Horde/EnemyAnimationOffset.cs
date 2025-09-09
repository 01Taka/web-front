using UnityEngine;

public class EnemyAnimationOffset : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // �e�I�u�W�F�N�g���܂߂�Animator�R���|�[�l���g��T��
        animator = GetComponentInParent<Animator>();

        if (animator != null)
        {
            float randomTime = Random.Range(0f, 1f);
            animator.Play(0, 0, randomTime);
        }
        else
        {
            Debug.LogError("Animator�R���|�[�l���g��������܂���ł����B");
        }
    }
}