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

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100f))
        {
            transform.position = Vector3.MoveTowards(transform.position, hit.point, speed * Time.deltaTime);
        }
        else { transform.position += transform.forward * speed * Time.deltaTime; }
    }




    void OnCollisionEnter(Collision collision)
    {
        if (!photonView.IsMine) return;

        if (collision.collider.CompareTag("Enemy"))
        {
            CharacterStats enemy = collision.collider.GetComponent<CharacterStats>();
            if (enemy != null)
            {
                enemy.photonView.RPC("TakeDamage", RpcTarget.AllBuffered, damage);
            }
        }
    }
}