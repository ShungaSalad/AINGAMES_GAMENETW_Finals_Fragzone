using Photon.Pun.Demo.Asteroids;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviourPun
{
    [SerializeField]
    GameObject bullet;
    [SerializeField]
    private int maxAmmo;

    public bool isBomb;
    public int currentAmmo;
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

    [PunRPC]
    public void isABomb(bool bomb)
    {
        isBomb = bomb;
    }
}
