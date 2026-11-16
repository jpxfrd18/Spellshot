using UnityEngine;
using UnityEngine.SceneManagement;

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
            GameObject explosionObj = Instantiate(explosion, transform.position, transform.rotation);
            SceneManager.MoveGameObjectToScene(explosionObj, gameObject.scene);

            Destroy(gameObject);
        }
    }
}
