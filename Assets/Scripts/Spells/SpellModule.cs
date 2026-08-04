using UnityEngine;

public abstract class SpellModule : MonoBehaviour
{
    public abstract void Cast(Player player);
    public abstract SpellCastType CastType { get; }
}