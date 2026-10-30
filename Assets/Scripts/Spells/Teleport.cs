using UnityEngine;

public class Teleport : SpellModule
{
    public override SpellCastType CastType { get; } = SpellCastType.Press;
    [SerializeField] private int manaCost;
    [SerializeField] private float maxDistance;

    private int mask;

    private void Awake()
    {
        mask = LayerMask.GetMask("Ground", "Enemy");
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

            Enemy enemy = rb.gameObject.GetComponentInChildren<Enemy>();

            if (enemy == null)
            {
                return;
            }

            if (!player.Stats.DecreaseMana(manaCost))
            {
                if (player.Stats.DecreaseHealth(2 * manaCost))
                {
                    Debug.Log("Player died");
                    return;
                }
            }

            player.Motor.Teleport(enemy.enemyTransform.position);

        }
    }
}
