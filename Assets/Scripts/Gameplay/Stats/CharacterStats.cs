using Photon.Pun;
using UnityEngine;

public class CharacterStats : MonoBehaviourPun
{
    public float maxHP = 100f;
    public float currentHP = 50f;

    public float maxStamina = 100f;
    public float currentStamina = 50f;


    [PunRPC]
    public void TakeDamage(float amount)
    {
        currentHP -= amount;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void ChangeStamina(float amount)
    { 
        currentStamina = Mathf.Clamp(currentStamina += amount, 0, maxStamina);

    }

    void Die()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonNetwork.Destroy(gameObject);
    }

}
