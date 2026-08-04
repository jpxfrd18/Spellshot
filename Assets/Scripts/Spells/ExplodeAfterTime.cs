using UnityEngine;

public class ExplodeAfterTime : MonoBehaviour
{
    [SerializeField] private float lifetime;
    private FireballController fireball;
    private float timer = 0f;

    private void Awake()
    {
        fireball = GetComponent<FireballController>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lifetime)
        {
            fireball.Explode();
        }
    }
}
