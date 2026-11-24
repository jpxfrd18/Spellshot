using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    [SerializeField] RangedAttack rangedAttack;
    [SerializeField] EnemyFollow enemyFollow;

    public void Fire()
    {
        if (!rangedAttack)
        {
            Debug.LogError("RangedAttack reference is missing in AnimationEventRelay.");
            return;
        }

        rangedAttack.Fire();
    }

    public void CancelRotation()
    {
        if (!enemyFollow)
        {
            Debug.LogError("EnemyFollow reference is missing in AnimationEventRelay.");
            return;
        }

        enemyFollow.CancelRotation();
    }

    public void EnableRotation()
    {
        if (!enemyFollow)
        {
            Debug.LogError("EnemyFollow reference is missing in AnimationEventRelay.");
            return;
        }
        enemyFollow.EnableRotation();
    }
}
