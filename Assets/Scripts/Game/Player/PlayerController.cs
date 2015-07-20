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
    public float rotationSmoothTime = 0.05f;
    float rotationY = 0f;
    float rotationVelocity = 0.0f;

    // Logic
    float characterBaseHeight = 1f; // height to spawn stuff at

    /// <summary>
    /// Set up BoltEntity.
    /// </summary>
    public override void Attached()
    {
        state.Transform.SetTransforms(transform);
        state.SetAnimator(animator);
        state.Animator.applyRootMotion = entity.isOwner;
    }

    /// <summary>
    /// Called when a NetPlayer takes control of this BoltEntity.
    /// </summary>
    public override void ControlGained()
    {
        gameObject.name = "Player(" + PlayerSettings.GetPlayerName() + ")";

        ControlToken ct = (ControlToken)entity.controlGainedToken;
        controllingPlayer = NetPlayer.GetNetPlayer(ct.playerID);

        GameObject.Find("Main Camera").GetComponent<SmartCamera>().Initialize(gameObject);

        Debug.Log("[" + controllingPlayer.ToString() + "] has taken control over " + gameObject.name);
    }

    /// <summary>
    /// Simulates server logic for this entity.
    /// </summary>
    public override void SimulateOwner()
    {
        
    }

    /// <summary>
    /// Create a command from the player input and send it.
    /// </summary>
    public override void SimulateController()
    {
        PollKeys();
        CheckAbilityUse();

        IPlayerCommandInput command = PlayerCommand.Create();
        command.moveUp = moveUp;
        command.moveDown = moveDown;
        command.moveLeft = moveLeft;
        command.moveRight = moveRight;
        command.rotationY = rotationY;
        entity.QueueInput(command);
    }


    void CheckAbilityUse()
    {
        if (Input.GetKeyDown(PlayerSettings.keyAttack1))
        {
            // Create event wich will be sent to the server
            evUseAbility useAbility = evUseAbility.Create(entity);

            // The ability slot used
            useAbility.AbilitySlotID = 0;

            // Mouse position at time of use
            Vector3 mousePos = new Vector3(Input.mousePosition.x, GetMainCamera().transform.position.y, Input.mousePosition.z);
            Vector3 mouseWorld = GetMainCamera().ScreenToWorldPoint(mousePos);

            // Get direction
            Vector3 direction = (mouseWorld - transform.position);
            direction.y = 0f;

            if (direction != Vector3.zero)
                direction.Normalize();

            Debug.Log("[FB::Client] direction[" + FZTools.WriteVector(direction) + "] mouseWorld[" + FZTools.WriteVector(mouseWorld) + "] position[" + FZTools.WriteVector(transform.position) + "]");

            useAbility.DirectionX = direction.x;
            useAbility.DirectionY = direction.z;

            useAbility.Send();
        }
    }

    public override void OnEvent(evUseAbility evnt)
    {
        if (BoltNetwork.isServer)
        {
            Vector3 origin = new Vector3(transform.position.x, characterBaseHeight, transform.position.z);
            BoltEntity be = BoltNetwork.Instantiate(BoltPrefabs.FireBall, new Vector3(0f, 10000f, 0f), Quaternion.identity);

            be.TakeControl();
            be.gameObject.name = "Fireball(" + NetPlayer.GetNetPlayer(evnt.RaisedBy.ConnectionId).playerName + ")";
            be.GetComponent<FireballTest>().Initialize(origin, new Vector3(evnt.DirectionX, 0f, evnt.DirectionY));

            //Debug.DrawLine(origin, new Vector3(evnt.DirectionX, characterBaseHeight, evnt.DirectionY), Color.red);
        }
    }

    /// <summary>
    /// Executes the Controllers command on this BoltEntity.
    /// </summary>
    /// <param name="cmd">The PlayerCommand to execute on this BoltEntity.</param>
    /// <param name="resetState"></param>
    public override void ExecuteCommand(Bolt.Command cmd, bool resetState)
    {
        PlayerCommand playerCommand = (PlayerCommand)cmd;

        if (resetState) // Got a correction from server, reset (this runs on the controller (proxies too?))
        {
            motor.SetState(playerCommand.Result.Position, playerCommand.Result.Velocity, true, 0);
        }
        else
        {
            // Apply movement (this runs on both server and client)
            PlayerMotor.State motorState = motor.Move(
                playerCommand.Input.moveUp,
                playerCommand.Input.moveDown,
                playerCommand.Input.moveLeft,
                playerCommand.Input.moveRight,
                false,
                playerCommand.Input.rotationY);

            // Copy the motor state to the commands result (this gets sent back to the client)
            playerCommand.Result.Position = motorState.position;
            playerCommand.Result.Velocity = motorState.velocity;

            // Handle rotation and animation
            if (motorState.input.x != 0f || motorState.input.z != 0f)
            {
                // Rotate towards look direction
                float walkRotation = GetWalkRotation(motorState.input);
                float rot = Mathf.SmoothDampAngle(transform.localRotation.eulerAngles.y, walkRotation, ref rotationVelocity, rotationSmoothTime);
                transform.localRotation = Quaternion.Euler(new Vector3(0f, rot, 0f));

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
    /// Returns the top-down rotation of this game object.
    /// </summary>
    float GetMouseRotation()
    {
        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 dir = Input.mousePosition - objectPos;
        return Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Get the rotation of the direction this entity is walking in.
    /// Note: Remake using Atan2 instead.
    /// </summary>
    float GetWalkRotation(Vector3 _moveInput)
    {
        float rot = 0f;

        // Z
        if (_moveInput.z > 0f) // UP
        {
            rot = 0f;

            if (_moveInput.x > 0f) // RIGHT
                rot += 45;
            else if (_moveInput.x < 0f) // LEFT
                rot -= 45;
        }
        else if (_moveInput.z < 0f) // DOWN
        {
            rot = 180f;

            if (_moveInput.x > 0f) // RIGHT
                rot -= 45;
            else if (_moveInput.x < 0f) // LEFT
                rot += 45;
        }
        else
        {
            // X
            if (_moveInput.x > 0f) // RIGHT
                rot = 90f;
            else if (_moveInput.x < 0f) // LEFT
                rot = 270f;
        }

        return rot;
    }

    Camera GetMainCamera()
    {
        return GameObject.Find("Main Camera").GetComponent<Camera>();
    }
}
