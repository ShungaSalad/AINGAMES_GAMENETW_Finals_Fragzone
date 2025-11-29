using Autodesk.Fbx;
using Photon.Pun.Demo.Asteroids;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    GameObject explosion;
    [SerializeField]
    GameObject bullet;

    private Transform bulletSpawnPoint;

    [SerializeField]
    private float speed = 50.0f;
    [SerializeField]
    private float lifeTime = 3.0f;
    //[SerializeField]
    //private int damage = 50;



    //public int Damage => damage;

    private void Initialize()
    {
        bulletSpawnPoint = gameObject.transform.GetChild(0).transform;
    }

    private void Update()
    {
        // Make the object always move forward
        transform.position +=
        transform.forward * speed * Time.deltaTime;
    }
    

    public void OnShoot()
    {
       
        //spawn a bullet
        Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }
}
