using UnityEngine;

public interface IPlayerController
{
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
}