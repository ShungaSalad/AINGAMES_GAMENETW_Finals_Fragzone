using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCharacter : MonoBehaviourPun
{
    EnemyBehavior EBehavior;
    [Header("Local variables")]
    public float CurrentEnemyHP;
    public float MaxEnemyHP;
    public Gun WeaponType;
    [Header("Network sync variables")]
    private Vector3 NetworkPosition;
    private Quaternion NetworkRotation;
    private float NetworkHealth;
    private Gun NetworkWeapon;
    public HealthManager hpman;

    void Awake()
    {
        EBehavior = GetComponent<EnemyBehavior>();
        CurrentEnemyHP = MaxEnemyHP;
        hpman.maxHealth = MaxEnemyHP;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        NetworkPosition = transform.position;
        NetworkRotation = transform.rotation;
        NetworkHealth = CurrentEnemyHP;
        NetworkWeapon = WeaponType;
    }

    // Update is called once per frame
    void Update()
    {
        switch (WeaponType)
        {
            //To do: Adjust shoot delays, firing rate percent, and detection range based on weapon currently used
            case Gun.Handgun: EBehavior.shootDelay = 2.0f; EBehavior.detectionRange = 75.0f; EBehavior.firingRangePercent = 50.0f; break;
            case Gun.Assault: EBehavior.shootDelay = 3.0f; EBehavior.detectionRange = 75.0f; EBehavior.firingRangePercent = 50.0f; break;
            case Gun.Sniper: EBehavior.shootDelay = 10.0f; EBehavior.detectionRange = 75.0f; EBehavior.firingRangePercent = 50.0f; break;
            case Gun.Bazooka: EBehavior.shootDelay = 5.0f; EBehavior.detectionRange = 75.0f; EBehavior.firingRangePercent = 50.0f; break;
        }
        CurrentEnemyHP = NetworkHealth;
        if (hpman.isDead) { Destroy(gameObject); Debug.Log("Enemy down!"); }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            switch (WeaponType)
            {
                //To do: Adjust shoot delays, firing rate percent, and detection range based on weapon currently used
                case Gun.Handgun: hpman.TakeDamage(25); break;
                case Gun.Assault: hpman.TakeDamage(15); break;
                case Gun.Sniper: hpman.TakeDamage(10); break;
                case Gun.Bazooka: hpman.TakeDamage(10); break;
                default: hpman.TakeDamage(50); break;
            }
            Debug.LogWarning("Enemy Damaged!");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // Local player -> send data
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(CurrentEnemyHP);
            stream.SendNext(WeaponType);
        }
        else // Remote player -> receive data
        {
            NetworkPosition = (Vector3)stream.ReceiveNext();
            NetworkRotation = (Quaternion)stream.ReceiveNext();
            NetworkHealth = (float)stream.ReceiveNext();
            NetworkWeapon = (Gun)stream.ReceiveNext();
        }
    }
}
