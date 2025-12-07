using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviourPun
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
        transform.position += transform.forward * speed * Time.deltaTime;
        /*
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100f))
        {
            transform.position = Vector3.MoveTowards(transform.position, hit.point, speed * Time.deltaTime);
        }
        else {  }
        */
    }




    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];

        if (collision.collider.CompareTag("Enemy"))
        {
            CharacterStats enemy = collision.collider.GetComponent<CharacterStats>();
            if (enemy != null)
            {
                //enemy.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
                Debug.LogWarning("Bullet Explode 2");
            }
        }

        if (collision.collider.CompareTag("Player"))
        {
            CharacterStats player = collision.collider.GetComponent<CharacterStats>();
            if (player != null)
            {
                //player.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
                
                Debug.LogWarning("Bullet Explode 3");
            }
        }
        Debug.LogWarning("Bullet Explode");
        Instantiate(explosion, contact.point, Quaternion.identity);
        // Destroy gameobject since it already collided with something
        Destroy(gameObject);
    }
}