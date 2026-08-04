using UnityEngine;

public class ManaOrb : MonoBehaviour
{
    [SerializeField] private int mana;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && !triggered)
        {
            triggered = true;
            PlayerStats.Instance.IncreaseMana(mana);
            Destroy(gameObject);
        }
    }
}
