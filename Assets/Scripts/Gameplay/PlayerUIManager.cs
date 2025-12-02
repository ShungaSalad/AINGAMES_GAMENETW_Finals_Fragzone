using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    
    public float Width, Height;
    //values
    private float maximumHP, currentHP, maximumStamina, currentStamina;
    [SerializeField]
    private RectTransform healthBar;
    [SerializeField]
    private RectTransform staminaBar;

    public void UpdateCurrentHPUI(float setCurrentHP)
    {
        //Sets Current HP in User Interface
        currentHP = setCurrentHP;
        SetHP();
    }

    public void UpdateCurrentStaminaUI(float setCurrentStamina)
    {
        //Sets Current Stamina in User Interface
        currentStamina = setCurrentStamina;
        SetStamina();
        Debug.Log("Stamina Updated");
    }

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

    public void SetStatus(float maximumStam, float currentStam, float maximumHealth, float currentHealth)
    {
        maximumHP = maximumHealth;
        currentHP = currentHealth;
        currentStamina = currentStam;
        maximumStamina = maximumStam;
        Debug.Log("Status Set");
    }
    
}
