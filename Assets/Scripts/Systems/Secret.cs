using UnityEngine;

public class Secret : MonoBehaviour
{
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered)
        {
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PauseController pc = PauseController.Instance;

            if (pc)
            {
                pc.ShowSecret();
            }

            triggered = true;

            PersistantPlayerStats.Instance.UnlockSpell(SecretUnlock.Instance.spellToUnlock);

            Destroy(gameObject);
        }
    }
}
