using UnityEngine;

public class ManaRegen : MonoBehaviour
{
    private PlayerRef playerRef;
    private float timer;
    [SerializeField] float regenInterval;

    private void Awake()
    {
        playerRef = GetComponent<PlayerRef>();
        timer = regenInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = regenInterval;
            playerRef.playerStats.IncreaseMana(1);
        }
    }
}
