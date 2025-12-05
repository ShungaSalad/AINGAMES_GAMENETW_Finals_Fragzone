using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    //Explosion Effect
    [SerializeField]
    private GameObject explosion;

    [SerializeField]
    private float speed = 50.0f;
    [SerializeField]
    private float lifeTime = 3.0f;
    [SerializeField]
    private int damage = 50;


    //public int Damage => damage;

    private void Start()
    {
        // Simply destroy the gameobject after the given lifeTime duration
        Destroy(gameObject, lifeTime);
    }


    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100f))
        {
            transform.position = Vector3.MoveTowards(transform.position, hit.point, speed * Time.deltaTime);
        }
        else { transform.position += transform.forward * speed * Time.deltaTime; }
    }


    private void OnCollisionEnter(Collision collision)
    {
        // Spawn the explosion effect on the collision point
        ContactPoint contact = collision.contacts[0];
        Instantiate(explosion, contact.point, Quaternion.identity);
        // Destroy gameobject since it already collided with something
        Destroy(gameObject);
    }
}