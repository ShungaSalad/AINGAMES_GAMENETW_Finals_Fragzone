using System;
using Unity.Jobs;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCharacter : MonoBehaviour, IPlayerController
{
    //Inherited from IPlayerController
    [Header("Base Values")]
    public float WalkingSpeed { get; set; } //this will be the basic walk speed of the character
    public float RunningSpeed { get; set; } //this will serve as the character's maximum movement speed aka run
    public float JumpForce { get; set; } //this will be the character's jumping power
    public float Gravity { get; set; } //this will serve as the world gravity

    [Header("Camera Reference")]
    public Camera PlayerCamera { get; set; } //this will be referenced to the main camera/ the camera that will serve as the player's vision

    [Header("Camera Rotation")]
    public float LookSpeed { get; set; } //sets the speed sensitivity
    public float LookXLimit { get; set; } //the angle of the look up and down

    [Header("Controller Properties")]
    public CharacterController CharacterController { get; set; } //reference to the character controller component
    public Vector3 MoveDirection { get; set; } //identifies the direction for movement
    public float RotationX { get; set; } //this is the base rotation of the character

    [Header("Movement Condition")]
    public bool CanMove { get; set; } //this identifies if the character is allowed to move

    [Header("Temp Values")]
    public float BaseStamina { get; set; } //this will serve as the character's base stamina
    public float TempStamina { get; set; } //this is for the changing stamina value
    public float BaseHP { get; set; } //this is the character's base health
    public float TempHP { get; set; } //this is for the changing hp value
    public bool IsAlive { get; set; } //Is player alive yet?

    //Local Values
    public float LookSPD, WalkSPD, RunSPD, JForce, Grav;
    public Camera Cam;
    Vector3 MovDir;
    CharacterController CharCon;
    public bool Moveable, Living;
    public float CurrHP, MaxHP, CurrStam, MaxStam;
    float TimeBeforeRecover;
    bool RecoveringStamina;

    [SerializeField]
    private Weapon currentWeapon;

    private Gun _currentGun;
    public Gun CurrentGun => _currentGun;
    void Awake()
    {
        //Setting Inherited Variables - Base Values 
        WalkingSpeed = WalkSPD;
        RunningSpeed = RunSPD;
        JumpForce = JForce;
        Gravity = Grav;

        //Setting Inherited Variables - Camera Reference and Rotation
        PlayerCamera = Cam;
        LookSpeed = LookSPD;
        LookXLimit = 45.0f;

        //Setting Inherited Variables - Controller Properties
        CharCon = GetComponent<CharacterController>();
        CharacterController = CharCon;
        MoveDirection = Vector3.zero;
        RotationX = 0.0f;

        //Setting Inherited Variables - Commom Values & Movement Condition
        CanMove = Moveable;
        BaseHP = MaxHP;
        TempHP = CurrHP;
        BaseStamina = MaxStam;
        TempStamina = CurrStam;
        IsAlive = Living;

        //Initialize Current HP and Stamina
        TempHP = BaseHP;
        TempStamina = BaseStamina;

        //Initialize Current Gun Type
        _currentGun = Gun.None;
    }

    void RefreshStats()
    {
        //Refreshes Status
        LookSpeed = LookSPD;
        CharacterController = CharCon;
        MoveDirection = MovDir;
        WalkingSpeed = WalkSPD;
        RunningSpeed = RunSPD;
        JumpForce = JForce;
        Gravity = Grav;
        CanMove = Moveable;
        BaseHP = MaxHP;
        TempHP = CurrHP;
        BaseStamina = MaxStam;
        TempStamina = CurrStam;
    }

    void ControlCharacter()
    {
        //this is for showing the cursor------------------------------------
        if (Input.GetKey(KeyCode.Z))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        //end cursor conditions---------------------------------------------

        // We are grounded, so recalculate move direction based on axes
        Vector3 Forward = transform.TransformDirection(Vector3.forward);
        Vector3 Right = transform.TransformDirection(Vector3.right);

        //conditions for movement
        // if ? then : else
        bool IsRunning = Input.GetKey(KeyCode.LeftShift);
        float CurSpeedX = Moveable ? (IsRunning && CurrStam >= 1 ? RunSPD : WalkSPD) * Input.GetAxis("Vertical") : 0;
        float CurSpeedY = Moveable ? (IsRunning && CurrStam >= 1 ? RunSPD : WalkSPD) * Input.GetAxis("Horizontal") : 0;
        float MovementDirectionY = MovDir.y;
        MovDir = (Forward * CurSpeedX) + (Right * CurSpeedY);
        //for the jumping condition
        if (Input.GetButton("Jump") && Moveable && CharCon.isGrounded)
        {
            MovDir.y = JForce;
        }
        else
        {
            MovDir.y = MovementDirectionY;
        }

        //press left shift to run
        //this will return true if the specific button is pressed (lShift)
        if (IsRunning && Input.GetKey(KeyCode.W) && CurrStam >= 0)
        {
            //sliderStamina.gameObject.SetActive(true);
            CurrStam -= 10 * Time.deltaTime; //decreases the stamina overtime;
            if (CurrStam <= 0)
            {
                CurrStam = 0;
            }
        }
        if (CurrStam <= MaxStam && !IsRunning)
        {
            CurrStam += 2.0f * Time.deltaTime;
            if (CurrStam >= MaxStam)
            {
                CurrStam = MaxStam; //this will prevent the stamina from geting a higher value
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!CharCon.isGrounded)
        {
            //pull the object down
            MovDir.y -= Grav * Time.deltaTime;
        }

        // Move the controller
        CharacterController.Move(MovDir * Time.deltaTime);

        //Player and Camera rotation
        if (Moveable)
        {
            RotationX += -Input.GetAxis("Mouse Y") * LookSpeed;
            RotationX = Mathf.Clamp(RotationX, -LookXLimit, LookXLimit); //this limits the angle of the x rotation
            Cam.transform.localRotation = Quaternion.Euler(RotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * LookSpeed, 0);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RecoveringStamina = false;
        RefreshStats();
    }

    // Update is called once per frame
    void Update()
    {
        //RefreshStats();
        if (CurrHP <= 0) { Living = false; } else { Living = true; }
        if (Living)
        {
            if (CurrStam <= MaxStam)
            {
                if (MovDir == Vector3.zero && !RecoveringStamina)
                {
                    TimeBeforeRecover = 3.0f;
                    RecoveringStamina = true;
                }
                if (RecoveringStamina)
                {
                    if (TimeBeforeRecover > 0) { TimeBeforeRecover -= 1.0f * Time.deltaTime; }
                    else { TimeBeforeRecover = 0.0f; CurrStam += 1.0f * Time.deltaTime; }
                }
            }
            ControlCharacter();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        UpdateAttack();
    }
    
    private void UpdateAttack()
    {
        //Attacks with Weapon when mouse button is hit
        if (Input.GetMouseButtonDown(0))
        {
            currentWeapon.OnShoot();
        }
    }
}
