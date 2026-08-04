using UnityEngine;

public class SecretUnlock : MonoBehaviour
{
    [field: SerializeField] public SpellType spellToUnlock { get; private set; }
    public static SecretUnlock Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}