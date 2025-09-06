using UnityEngine;

public class ColliderCounter : MonoBehaviour
{
    // ���o�̒��S�ʒu
    public Transform detectionCenter;

    // ���o�̔��a�iOverlapCircle�p�j
    public float detectionRadius = 5.0f;

    // ���o�̋�`�T�C�Y�iOverlapBox�p�j
    public Vector2 detectionBoxSize = new Vector2(5.0f, 5.0f);

    // ���o�̕����iRaycast�p�j
    public Vector2 raycastDirection = Vector2.right;

    // ���o�Ώۂ̃��C���[
    public LayerMask targetLayer;

    private void Update()
    {
        // ���O���N���A���āA���₷������
        Debug.ClearDeveloperConsole();

        // Physics2D.OverlapCircleAll(): �~���̂��ׂẴR���C�_�[�����o
        CheckOverlapCircleAll();

        // Physics2D.OverlapBoxAll(): ��`���̂��ׂẴR���C�_�[�����o
        CheckOverlapBoxAll();

        // Physics2D.RaycastAll(): �w�肵�������̃��C�ɓ����邷�ׂẴR���C�_�[�����o
        CheckRaycastAll();

        // Physics2D.OverlapCircle(): �P��̃R���C�_�[�����o�i�ł��߂����́j
        CheckOverlapCircle();
    }

    private void OnDrawGizmos()
    {
        // Gizmos���g���ăf�o�b�O�p�̐}�`��`��
        Vector3 center = detectionCenter != null ? detectionCenter.position : transform.position;

        // OverlapCircle�̉~
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, detectionRadius);

        // OverlapBox�̋�`
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, detectionBoxSize);

        // Raycast�̃��C
        Gizmos.color = Color.red;
        Gizmos.DrawLine(center, center + (Vector3)raycastDirection * 10f); // 10f�͓K���Ȓ���
    }

    //---------------------------------------------------------
    // �e�팟�m���\�b�h
    //---------------------------------------------------------

    private void CheckOverlapCircleAll()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(detectionCenter.position, detectionRadius, targetLayer);
        Debug.Log("OverlapCircleAll: " + hitColliders.Length + " �̃I�u�W�F�N�g�����o���܂����B");
        foreach (Collider2D collider in hitColliders)
        {
            Debug.Log("  - ���o�����I�u�W�F�N�g: " + collider.gameObject.name);
        }
    }

    private void CheckOverlapBoxAll()
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(detectionCenter.position, detectionBoxSize, 0f, targetLayer);
        Debug.Log("OverlapBoxAll: " + hitColliders.Length + " �̃I�u�W�F�N�g�����o���܂����B");
        foreach (Collider2D collider in hitColliders)
        {
            Debug.Log("  - ���o�����I�u�W�F�N�g: " + collider.gameObject.name);
        }
    }

    private void CheckRaycastAll()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(detectionCenter.position, raycastDirection, 10f, targetLayer);
        Debug.Log("RaycastAll: " + hits.Length + " �̃I�u�W�F�N�g�����o���܂����B");
        foreach (RaycastHit2D hit in hits)
        {
            Debug.Log("  - ���o�����I�u�W�F�N�g: " + hit.collider.gameObject.name);
        }
    }

    private void CheckOverlapCircle()
    {
        Collider2D hitCollider = Physics2D.OverlapCircle(detectionCenter.position, detectionRadius, targetLayer);
        if (hitCollider != null)
        {
            Debug.Log("OverlapCircle: " + hitCollider.gameObject.name + " �����o���܂����B");
        }
    }
}