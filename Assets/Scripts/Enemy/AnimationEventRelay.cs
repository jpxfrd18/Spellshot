using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    [SerializeField] RangedAttack rangedAttack;

    public void Fire()
    {
        rangedAttack.Fire();
    }
}
