using UnityEngine;

public class LightningStrike : SpellModule
{
    [SerializeField] protected GameObject lightningPrefab;
    [SerializeField] protected int manaCost;
    public override SpellCastType CastType { get; } = SpellCastType.Press;
    private int mask;

    private void Awake()
    {
        mask = LayerMask.GetMask("Ground", "Enemy");
    }

    public override void Cast(Player player)
    {
        if (Physics.Raycast(player.AimSource, player.AimForward, out RaycastHit hit, 100f, mask))
        {
            if (!player.Stats.DecreaseMana(manaCost))
            {
                if (player.Stats.DecreaseHealth(2 * manaCost))
                {
                    return;
                }
            }

            Vector3 toPlayer = player.transform.position - hit.point;
            toPlayer.y = 0f;
            toPlayer.Normalize();

            GameObject lightning = Instantiate(lightningPrefab, hit.point, Quaternion.LookRotation(toPlayer, Vector3.up));
        }
    }
}