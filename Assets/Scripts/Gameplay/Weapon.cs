using Photon.Pun.Demo.Asteroids;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Weapon : MonoBehaviourPun
{
    [SerializeField]
    GameObject bullet;
    [SerializeField]
    private int maxAmmo;

    private Rigidbody rb;
    private PhotonView ownerView;

    public Gun gunType;

    [Header("Network sync variables")]
    private Vector3 networkPosition;
    private Quaternion networkRotation;

    public bool isBomb;
    public int currentAmmo;
    private Transform bulletSpawnPoint;

    //[SerializeField]
    //private int damage = 50;



    //public int Damage => damage;

    private void Start()
    {
        
        bulletSpawnPoint = gameObject.transform.GetChild(0).transform;
        /*
        if (GetComponent<Rigidbody>() == null)
        {
            Debug.Log("No Rigid Body detected.");
            return;
        }
        */
        rb = GetComponent<Rigidbody>();
        networkPosition = transform.position;
        networkRotation = transform.rotation;
    }
    

    public void OnShoot()
    {
        //spawn a bullet
        GameObject projectile=Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }

    [PunRPC]
    public void IsABomb(bool bomb)
    {
        isBomb = bomb;
    }

    [PunRPC]
    public Gun GetWeaponType()
    {
        return gunType;
    }

    [PunRPC]
    public void PickUp(int playerViewID)
    {
        PhotonView playerView = PhotonView.Find(playerViewID);
        if (playerView != null)
        {
            ownerView = playerView;
            rb.isKinematic = true;
        }
    }

    [PunRPC]
    public void RPC_Release()
    {
        ownerView = null;
        rb.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (ownerView == null)
        {
            if (!photonView.IsMine)
            {
                transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
                transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
            }
        }
    }


}
