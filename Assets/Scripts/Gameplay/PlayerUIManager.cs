using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class PlayerUIManager : MonoBehaviourPun
{
    
    public float Width, Height;
    //values
    public float maximumHP, currentHP, maximumStamina, currentStamina;
    [SerializeField]
    private RectTransform healthBar;
    [SerializeField]
    private RectTransform staminaBar;
    [SerializeField]
    private TMP_Text roomInfo;


    private void SetHP()
    {
        float newWidth = (currentHP / maximumHP) * Width;
        healthBar.sizeDelta = new Vector2(newWidth, Height);
    }

    private void SetStamina()
    {
        float newWidth = (currentStamina / maximumStamina) * Width;
        staminaBar.sizeDelta = new Vector2(newWidth, Height);
    }

    [PunRPC]
    public void UpdateCurrentHPUI(float setCurrentHP)
    {
        //Sets Current HP in User Interface
        currentHP = setCurrentHP;
        SetHP();
    }

    [PunRPC]
    public void UpdateCurrentStaminaUI(float setCurrentStamina)
    {
        //Sets Current Stamina in User Interface
        currentStamina = setCurrentStamina;
        SetStamina();
        //Debug.Log("Stamina Updated");
    }

    [PunRPC]
    public void SetStatusUI(float maximumStam, float currentStam, float maximumHealth, float currentHealth)
    {
        maximumHP = maximumHealth;
        currentHP = currentHealth;
        currentStamina = currentStam;
        maximumStamina = maximumStam;
        Debug.Log("Status Set");
        SetHP();
        SetStamina();
    }

    [PunRPC]
    public void SetStatusUI(CharacterStats stats)
    {
        maximumHP = stats.maxHP;
        currentHP = stats.currentHP;
        currentStamina = stats.currentStamina;
        maximumStamina = stats.maxStamina;
        SetHP();
        SetStamina();
    }

    [PunRPC]
    public void UpdateRoomInfo(string txt)
    {
        roomInfo.text = txt;
    }
}
