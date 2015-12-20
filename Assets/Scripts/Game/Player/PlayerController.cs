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
    float characterBaseHeight = 0.5f; // height to spawn stuff at

    /// <summary>
    /// Set up BoltEntity.
    /// </summary>
    public override void Attached()
    {
        state.Transform.SetTransforms(transform);
        state.SetAnimator(animator);
        state.Animator.applyRootMotion = entity.isOwner;

        state.AddCallback("UsedAbility", UsedAbilityAux);
    }

    float lastRot = 0f;

    void UsedAbilityAux()
    {
        UseAbility(0, 0f);
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

    public override void ControlLost()
    {
        ControlToken ct = (ControlToken)entity.controlGainedToken;
        controllingPlayer = NetPlayer.GetNetPlayer(ct.playerID);
        gameObject.name = "Player(" + controllingPlayer.playerName + ")";
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
        //CheckAbilityUse();

        IPlayerCommandInput command = PlayerCommand.Create();
        // Movement input
        command.moveUp = moveUp;
        command.moveDown = moveDown;
        command.moveLeft = moveLeft;
        command.moveRight = moveRight;

        // Mouse rotation
        command.rotationY = rotationY;

        // Ability usage
        command.fireAbility0 = Input.GetMouseButtonDown(0);
        command.fireAbility1 = Input.GetMouseButtonDown(1);

        entity.QueueInput(command);
    }

    /// <summary>
    /// Executes the Controllers command on this BoltEntity.
    /// </summary>
    /// <param name="cmd">The PlayerCommand to execute on this BoltEntity.</param>
    /// <param name="resetState"></param>
    public override void ExecuteCommand(Bolt.Command cmd, bool resetState)
    {
        PlayerCommand pCmd = (PlayerCommand)cmd;
        lastRot = pCmd.Input.rotationY;
        if (resetState) // Got a correction from server, reset (this runs on the controller)
        {
            motor.SetState(pCmd.Result.Position, pCmd.Result.Velocity, true, 0);
        }
        else
        {
            // Apply movement (this runs on both server and client)
            PlayerMotor.State motorState = motor.Move(
                pCmd.Input.moveUp,
                pCmd.Input.moveDown,
                pCmd.Input.moveLeft,
                pCmd.Input.moveRight,
                false,
                pCmd.Input.rotationY);

            // Copy the motor state to the commands result (this gets sent back to the client)
            pCmd.Result.Position = motorState.position;
            pCmd.Result.Velocity = motorState.velocity;

            // Handle rotation and animation
            if (motorState.input.x != 0f || motorState.input.z != 0f)
            {
                // Rotate towards look direction
                float walkRotation = GetWalkRotation(motorState.input);
                float rotation = Mathf.SmoothDampAngle(transform.localRotation.eulerAngles.y, walkRotation, ref rotationVelocity, rotationSmoothTime);
                FZTools.SetRotation(transform, rotation);

                // Set state to moving
                state.isMoving = true;
            }
            else
            {
                // Set state to not moving
                state.isMoving = false;
            }
        }

        //if (pCmd.IsFirstExecution && BoltNetwork.isServer)
        //{
        //    if (pCmd.Input.fireAbility0)
        //        UseAbility(0, pCmd.Input.rotationY);
        //    else if (pCmd.Input.fireAbility1)
        //        UseAbility(1, pCmd.Input.rotationY);
        //}
    }

    void UseAbility(int _index, float _rotation)
    {
        if (_index == 0)
        {
            Vector3 origin = new Vector3(transform.position.x, characterBaseHeight, transform.position.z);
            BoltEntity be = BoltNetwork.Instantiate(BoltPrefabs.FireBall, new Vector3(-1000f, -1000f, 0f), Quaternion.identity);

            if (controllingPlayer == null)
                be.gameObject.name = "Fireball(null)";
            else
                be.gameObject.name = "Fireball(" + controllingPlayer.playerName + ")";

            be.GetComponent<FireballTest>().Initialize(origin, _rotation);
        }
        else
        {
            Debug.Log("[UseAbility] Unknown ability index " + _index);
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
