using UnityEngine;

public class FireballController : MonoBehaviour
{
    [SerializeField] private GameObject explosion;
    private bool exploded = false;

    private void OnCollisionEnter(Collision collider)
    {
        Explode();
    }

    public void Explode()
    {
        if (!exploded)
        {
            exploded = true;
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
