using UnityEngine;

public class Gust : SpellModule
{
    [SerializeField] private int manaCost;
    public override SpellCastType CastType { get; } = SpellCastType.Press;

    public override void Cast(Player player)
    {
        if (player.Stats.DecreaseMana(manaCost))
        {
            player.Motor.Gust();
        }
    }
}
