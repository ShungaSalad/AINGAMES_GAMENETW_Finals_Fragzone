using Photon.Pun.Demo.Asteroids;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    GameObject bullet;

    private Transform bulletSpawnPoint;

    //[SerializeField]
    //private int damage = 50;



    //public int Damage => damage;

    private void Start()
    {
        bulletSpawnPoint = gameObject.transform.GetChild(0).transform;
    }
    

    public void OnShoot()
    {
        //spawn a bullet
        GameObject projectile=Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }
}
