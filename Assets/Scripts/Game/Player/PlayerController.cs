using UnityEngine;
using System.Collections;
using UdpKit;
using Bolt;

public class PlayerController : Bolt.EntityEventListener<IPlayerState>
{
    // Character animator
    public Animator animator;

    // Character motor
    public PlayerMotor motor;

    // The controlling NetPlayer
    public NetPlayer controllingPlayer;

    // Keys to track
    bool moveUp = false;
    bool moveDown = false;
    bool moveLeft = false;
    bool moveRight = false;

    // Rotation
    float rotationY = 0f;

    // Client Smoothing
    float rotationSmoothTime = 0.2f;

    /// <summary>
    /// Set up BoltEntity.
    /// </summary>
    public override void Attached()
    {
        state.Transform.SetTransforms(transform);
        state.SetAnimator(animator);
        state.Animator.applyRootMotion = entity.isOwner;

        //state.AddCallback("CubeColor", ColorChanged); // Whenever the CubeColor value changed, ColorChanged will be called
    }

    /// <summary>
    /// Called when a NetPlayer takes control of this BoltEntity.
    /// </summary>
    public override void ControlGained()
    {
        gameObject.name = "Player(" + PlayerSettings.GetPlayerName() + ")";

        ControlToken ct = (ControlToken)entity.controlGainedToken;
        controllingPlayer = NetPlayer.GetNetPlayer(ct.playerID);

        GameObject.Find("Camera Main").GetComponent<SmartCamera>().Initialize(gameObject);

        Debug.Log("[" + controllingPlayer.ToString() + "] has taken control over " + gameObject.name);
    }

    /// <summary>
    /// Save player keypresses
    /// </summary>
    void PollKeys()
    {
        moveUp = Input.GetKey(PlayerSettings.keyMoveUp);
        moveDown = Input.GetKey(PlayerSettings.keyMoveDown);
        moveLeft = Input.GetKey(PlayerSettings.keyMoveLeft);
        moveRight = Input.GetKey(PlayerSettings.keyMoveRight);
        rotationY = GetMouseRotation();
    }

    /// <summary>
    /// Create a command from the player input and send it.
    /// </summary>
    public override void SimulateController()
    {
        PollKeys();

        IPlayerCommandInput command = PlayerCommand.Create();
        command.moveUp = moveUp;
        command.moveDown = moveDown;
        command.moveLeft = moveLeft;
        command.moveRight = moveRight;
        command.rotationY = rotationY;
        entity.QueueInput(command);
    }

    // 
    PlayerMotor.State lastMotorState;
    Vector3 moveInput;

    /// <summary>
    /// Executes the Controllers command on this BoltEntity.
    /// </summary>
    /// <param name="cmd">The PlayerCommand to execute on this BoltEntity.</param>
    /// <param name="resetState"></param>
    public override void ExecuteCommand(Bolt.Command cmd, bool resetState)
    {
        PlayerCommand playerCommand = (PlayerCommand)cmd;

        if (resetState) // we got a correction from the server, reset (this runs on the controller (proxies too?))
        {
            motor.SetState(playerCommand.Result.Position, playerCommand.Result.Velocity, true, 0);
        }
        else
        {
            // apply movement (this runs on both server and client)
            PlayerMotor.State motorState = motor.Move(
                playerCommand.Input.moveUp, 
                playerCommand.Input.moveDown, 
                playerCommand.Input.moveLeft, 
                playerCommand.Input.moveRight, 
                false, 
                playerCommand.Input.rotationY);

            // Store input (remove?)
            lastMotorState = motorState;
            moveInput = lastMotorState.input;

            // copy the motor state to the commands result (this gets sent back to the client)
            playerCommand.Result.Position = motorState.position;
            playerCommand.Result.Velocity = motorState.velocity;

            // Handle rotation and animation
            // Rotation
            if (moveInput.x != 0f || moveInput.z != 0f)
            {
                // Look in move direction
                Vector3 targetPosition = new Vector3(
                    transform.position.x + moveInput.x,
                    transform.position.y,
                    transform.position.z + moveInput.z);

                // Rotate towards look direction
                print("LookAt[" + Vector3.Lerp(transform.rotation.eulerAngles, targetPosition, rotationSmoothTime) + "]");
                //transform.LookAt(Vector3.Lerp(transform.rotation.eulerAngles, targetPosition, rotationSmoothTime));
                transform.localRotation = Quaternion.Euler(new Vector3(0f, playerCommand.Input.rotationY, 0f));

                // Set state to moving
                state.isMoving = true;
            }
            else
            {
                // Set state to not moving
                state.isMoving = false;
            }
        }
    }

    /// <summary>
    /// Returns the top-down rotation of this game object.
    /// </summary>
    float GetMouseRotation()
    {
        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - objectPos;
        return Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
    }
}
