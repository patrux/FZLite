using UnityEngine;
using System.Collections;

public class PlayerController : Bolt.EntityEventListener<IPlayerState>
{
    Rigidbody rigidBody;
    public Animator animator;
    public PlayerMotor motor;

    /// <summary>
    /// Set up the BoltNetwork State for this entity.
    /// </summary>
    public override void Attached()
    {
        state.Transform.SetTransforms(transform);
        state.SetAnimator(animator);
        state.Animator.applyRootMotion = entity.isOwner;
    }

    Vector3 velocity = Vector3.zero;

    public float speed = 3f;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        PollKeys(true);
    }

    public override void ControlGained()
    {
        gameObject.name = "Player(" + PlayerSettings.GetPlayerName() + ")";

        SmartCamera sc = Camera.main.GetComponent<SmartCamera>();
        sc.enabled = true;
        sc.targetName = gameObject.name;
    }

    bool moveUp = false;
    bool moveDown = false;
    bool moveLeft = false;
    bool moveRight = false;

    void PollKeys(bool _mouse)
    {
        moveUp = Input.GetKey(PlayerSettings.keyMoveUp);
        moveDown = Input.GetKey(PlayerSettings.keyMoveDown);
        moveLeft = Input.GetKey(PlayerSettings.keyMoveLeft);
        moveRight = Input.GetKey(PlayerSettings.keyMoveRight);

        if (_mouse) { } // get mouse info
    }

    public override void SimulateController()
    {
        PollKeys(false);

        IPlayerCommandInput command = PlayerCommand.Create();
        command.moveUp = moveUp;
        command.moveDown = moveDown;
        command.moveLeft = moveLeft;
        command.moveRight = moveRight;
        entity.QueueInput(command);
    }

    PlayerMotor.State lastMotorState;
    Vector3 moveInput;

    public override void ExecuteCommand(Bolt.Command cmd, bool resetState)
    {
        //if (entity.isOwner) // hack to only call on controller
        //    return;

        PlayerCommand pCmd = (PlayerCommand)cmd;

        if (resetState)
        {
            // we got a correction from the server, reset (this only runs on the client)
            motor.SetState(pCmd.Result.Position, pCmd.Result.Velocity, true, 0);
        }
        else
        {
            // apply movement (this runs on both server and client)
            PlayerMotor.State motorState = motor.Move(pCmd.Input.moveUp, pCmd.Input.moveDown, pCmd.Input.moveLeft, pCmd.Input.moveRight, false, 0f);
            lastMotorState = motorState;

            // copy the motor state to the commands result (this gets sent back to the client)
            pCmd.Result.Position = motorState.position;
            pCmd.Result.Velocity = motorState.velocity;

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
            // Look in mouse direction
            else
            {
                state.isMoving = false;
            }
        }
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
}
