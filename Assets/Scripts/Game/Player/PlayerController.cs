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

    // The controlling NetPlayer (server only?)
    public NetPlayer controllingPlayer;

    // Keys to track
    bool cmdMoveUp = false;
    bool cmdMoveDown = false;
    bool cmdMoveLeft = false;
    bool cmdMoveRight = false;
    bool cmdAbility0 = false;
    bool cmdAbility1 = false;

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
        //state.Transform.SetTransforms(transform);
        state.SetTransforms(state.Transform, transform);
        state.SetAnimator(animator);
        state.Animator.applyRootMotion = entity.isOwner;

        //state.AddCallback("UsedAbility", UsedAbilityAux);
        state.OnUsedAbility += UsedAbilityAux;
    }

    float lastRot = 0f;

    void UsedAbilityAux()
    {
        print("Local ability effects here");
        // Local effects
        //UseAbility(0, 0f);
    }

    /// <summary>
    /// Called when a NetPlayer takes control of this BoltEntity.
    /// </summary>
    public override void ControlGained()
    {
        ControlToken ct = (ControlToken)entity.controlGainedToken;

        NetPlayer netPlayer = NetPlayer.GetNetPlayer(ct.playerID);
        controllingPlayer = netPlayer;
        netPlayer.playerController = this;

        gameObject.name = "Player(" + PlayerSettings.GetPlayerName() + ")";

        GameObject.Find("Main Camera").GetComponent<SmartCamera>().Initialize(gameObject);

        Debug.Log("[" + controllingPlayer.ToString() + "] has taken control over " + gameObject.name);
    }

    public override void ControlLost()
    {
        ControlToken ct = (ControlToken)entity.controlGainedToken;

        NetPlayer netPlayer = NetPlayer.GetNetPlayer(ct.playerID);
        controllingPlayer = netPlayer;
        netPlayer.playerController = this;

        gameObject.name = "Player(" + controllingPlayer.playerName + ")";
    }

    /// <summary>
    /// Simulates server logic for this entity.
    /// </summary>
    public override void SimulateOwner()
    {

    }

    void Update()
    {
        PollKeys();
        //CheckAbilityUse();
    }

    /// <summary>
    /// Create a command from the player input and send it.
    /// </summary>
    public override void SimulateController()
    {
        IPlayerCommandInput command = PlayerCommand.Create();

        // Movement input
        command.moveUp = cmdMoveUp;
        command.moveDown = cmdMoveDown;
        command.moveLeft = cmdMoveLeft;
        command.moveRight = cmdMoveRight;

        // Mouse rotation
        command.rotationY = rotationY;

        // Ability usage
        command.fireAbility0 = cmdAbility0;
        command.fireAbility1 = cmdAbility1;

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
            PlayerMotor.State resultMotorState = motor.Move(
                pCmd.Input.moveUp,
                pCmd.Input.moveDown,
                pCmd.Input.moveLeft,
                pCmd.Input.moveRight,
                false,
                pCmd.Input.rotationY);

            // Copy the motor state to the commands result (this gets sent back to the client)
            pCmd.Result.Position = resultMotorState.position;
            pCmd.Result.Velocity = resultMotorState.velocity;

            if (cmd.IsFirstExecution)
            {
                // Rotation and animation
                if (!IsMoving(resultMotorState.input))
                {
                    // Rotate towards look direction
                    float walkRotation = GetWalkRotation(resultMotorState.input);
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

                // Abilities
                if (pCmd.Input.fireAbility0)
                    UseAbility(0, pCmd.Input.rotationY);
                else if (pCmd.Input.fireAbility1)
                    UseAbility(1, pCmd.Input.rotationY);
            }
        }
    }

    bool IsMoving(Vector3 _vector)
    {
        if (_vector.x != 0f || _vector.z != 0f)
        {
            return false;
        }
        return true;
    }

    int fireFrame = 0;
    void UseAbility(int _index, float _rotation)
    {
        state.UsedAbility();

        if (BoltNetwork.isServer)
        {
            if (fireFrame + 200 <= BoltNetwork.serverFrame)
            {
                fireFrame = BoltNetwork.serverFrame;
            }
            else
            {
                return;
            }

                if (_index == 0)
            {
                Vector3 origin = new Vector3(transform.position.x, characterBaseHeight, transform.position.z);
                BoltEntity be = BoltNetwork.Instantiate(BoltPrefabs.FireBall, new Vector3(-1000f, -1000f, 0f), Quaternion.identity);

                if (controllingPlayer == null)
                    be.gameObject.name = "Fireball(null)";
                else
                    be.gameObject.name = "Fireball(" + controllingPlayer.playerName + ")";

                be.GetComponent<FireballTest>().Initialize(origin, _rotation, controllingPlayer);
            }
            else
            {
                Debug.Log("[UseAbility] Unknown ability index " + _index);
            }
        }
    }

    /// <summary>
    /// Save player keypresses
    /// </summary>
    void PollKeys()
    {
        cmdMoveUp = Input.GetKey(PlayerSettings.keyMoveUp);
        cmdMoveDown = Input.GetKey(PlayerSettings.keyMoveDown);
        cmdMoveLeft = Input.GetKey(PlayerSettings.keyMoveLeft);
        cmdMoveRight = Input.GetKey(PlayerSettings.keyMoveRight);

        rotationY = GetMouseRotation();

        cmdAbility0 = Input.GetKey(PlayerSettings.keyAttack1);
        cmdAbility1 = Input.GetKey(PlayerSettings.keyAttack2);
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

        if (_moveInput.z > 0f) // UP
        {
            rot = 0f;

            if (_moveInput.x > 0f) // RIGHT
                rot += 45f;
            else if (_moveInput.x < 0f) // LEFT
                rot -= 45f;
        }
        else if (_moveInput.z < 0f) // DOWN
        {
            rot = 180f;

            if (_moveInput.x > 0f) // RIGHT
                rot -= 45f;
            else if (_moveInput.x < 0f) // LEFT
                rot += 45f;
        }
        else
        {
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

    public void Dispose()
    {
        //netPlayer.playerController = null;
        //playerList.Remove(this);

        // destroy
        if (entity)
        {
            BoltNetwork.Destroy(entity.gameObject);
        }
    }
}
