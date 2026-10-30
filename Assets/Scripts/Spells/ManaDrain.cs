using UnityEngine;

public class ManaDrain : SpellModule
{
    public override SpellCastType CastType { get; } = SpellCastType.Press;
    [SerializeField] private float maxDistance;

    private int mask;

    private void Awake()
    {
        mask = LayerMask.GetMask("Enemy");
    }

    public override void Cast(Player player)
    {
        if (Physics.Raycast(player.AimSource, player.AimForward, out RaycastHit hit, maxDistance, mask))
        {
            Rigidbody rb = hit.rigidbody;

            if (rb == null)
            {
                return;
            }

            EnemyHealth enemy = rb.gameObject.GetComponent<EnemyHealth>();

            if (enemy == null)
            {
                return;
            }

            enemy.ManaDrain();
        }
    }
}
