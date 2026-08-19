using UnityEngine;

public class Portal : SpellModule
{
    public override SpellCastType CastType { get; } = SpellCastType.Press;
    [SerializeField] private int manaCost;
    [SerializeField] private GameObject circlePrefab;
    private GameObject circle = null;

    public override void Cast(Player player)
    {
        if (!circle)
        {
            if (!player.Stats.DecreaseMana(manaCost))
            {
                if (player.Stats.DecreaseHealth(2 * manaCost))
                {
                    return;
                }
            }
            
            Vector3 direction = player.AimForward;
            direction.y = 0;
            circle = Instantiate(circlePrefab, player.AimSource, Quaternion.identity);

            if (direction.sqrMagnitude > 0.01)
            {
                circle.transform.forward = direction.normalized;
            }
        }
        else
        {
            player.Motor.Teleport(circle.transform.position);
            player.Motor.SetYaw(circle.transform.forward);
            player.Motor.SetVelocityDirection(circle.transform.forward);
            player.Motor.MultiplyVelocity(0.8f);
            Destroy(circle);
            circle = null;
        }
    }
}
