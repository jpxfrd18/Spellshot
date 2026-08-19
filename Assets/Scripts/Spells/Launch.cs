using UnityEngine;

public class Launch : SpellModule
{
    [SerializeField] private int manaCost;
    public override SpellCastType CastType { get; } = SpellCastType.Press;

    public override void Cast(Player player)
    {
        if (!player.Stats.DecreaseMana(manaCost))
        {
            if (player.Stats.DecreaseHealth(2 * manaCost))
            {
                return;
            }
        }

        player.Motor.Launch(player.AimForward);

    }
}
