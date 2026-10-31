using UnityEngine;

public class Gust : SpellModule
{
    [SerializeField] private int manaCost;
    [SerializeField] private float force = 12.6f;
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

        player.Motor.Gust(force);
    }
}
