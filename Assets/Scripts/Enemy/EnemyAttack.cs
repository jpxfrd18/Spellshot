using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public bool isAttacking { get; private set; } = false;
    public Vector3 animationDelta { get; private set; } = Vector3.zero;
    [SerializeField] Animator animator;

    public void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
    }

    public void FinishAttack()
    {
        isAttacking = false;
    }

    private void OnDisable()
    {
        isAttacking = false;
    }

    private void OnAnimatorMove()
    {
        animationDelta = animator.deltaPosition;
    }
}
