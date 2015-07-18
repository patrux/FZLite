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

    public override void ControlGained()
    {
        gameObject.name = "Player(" + PlayerSettings.GetPlayerName() + ")";

        ControlToken ct = (ControlToken)entity.controlGainedToken;
        controllingPlayer = NetPlayer.GetNetPlayer(ct.playerID);

        Debug.Log("controllingPlayer[" + controllingPlayer + "]");

        SmartCamera sc = Camera.main.GetComponent<SmartCamera>();
        sc.enabled = true;
        sc.targetName = gameObject.name;
    }

    void PollKeys()
    {
        moveUp = Input.GetKey(PlayerSettings.keyMoveUp);
        moveDown = Input.GetKey(PlayerSettings.keyMoveDown);
        moveLeft = Input.GetKey(PlayerSettings.keyMoveLeft);
        moveRight = Input.GetKey(PlayerSettings.keyMoveRight);
        rotationY = GetMouseRotation();
    }

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

    public override void ExecuteCommand(Bolt.Command cmd, bool resetState)
    {
        PlayerCommand playerCommand = (PlayerCommand)cmd;

        if (resetState)
        {
            // we got a correction from the server, reset (this only runs on the client)
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

            lastMotorState = motorState;

            // copy the motor state to the commands result (this gets sent back to the client)
            playerCommand.Result.Position = motorState.position;
            playerCommand.Result.Velocity = motorState.velocity;

            // Handle rotation and animation
            moveInput = lastMotorState.input;

            // Look in walk direction
            if (moveInput.x != 0f || moveInput.z != 0f)
            {
                Vector3 targetPosition = new Vector3(
                    transform.position.x + moveInput.x,
                    transform.position.y,
                    transform.position.z + moveInput.z);

                transform.LookAt(targetPosition);

                state.isMoving = true;
            }
            else
            {
                state.isMoving = false;
            }
        }
    }

    bool IsController()
    {
        return (entity.IsController(controllingPlayer.connection));
    }

    public override void SimulateOwner()
    {
        //Vector3 moveInput = Vector3.zero;
        //moveInput.x = Input.GetAxis("Horizontal");
        //moveInput.z = Input.GetAxis("Vertical");

        //if (moveInput != Vector3.zero)
        //{
        //    animator.SetFloat("velocity", Mathf.Max(Mathf.Abs(moveInput.x), Mathf.Abs(moveInput.z)));
        //    transform.position = transform.position + (moveInput.normalized * speed * BoltNetwork.frameDeltaTime);
        //    transform.localRotation = Quaternion.LookRotation(moveInput.normalized);
        //}
        //else
        //{
        //    animator.SetFloat("velocity", 0f);
        //}
    }

    float GetMouseRotation()
    {
        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - objectPos;
        float rotation = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        return rotation;
    }
}
