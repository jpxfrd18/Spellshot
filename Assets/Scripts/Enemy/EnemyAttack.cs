using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public bool isAttacking { get; private set; } = false;
    public Vector3 animationDelta { get; private set; } = Vector3.zero;
    [SerializeField] private Animator animator;
    [SerializeField] private string attackName = "Attack";

    public void Attack()
    {
        isAttacking = true;
        if (string.IsNullOrWhiteSpace(attackName))
        {
            Debug.LogWarning($"{gameObject.name} No attack name specified. Falling back to 'Attack'");
            animator.SetTrigger("Attack");
        }
        else
        {
            animator.SetTrigger(attackName);
        }
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
