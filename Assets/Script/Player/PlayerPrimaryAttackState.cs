using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int ComboCounter { get; private set; }
    private float lastTimeAttacked;
    private float comboWindow = 2;

    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string animBoolName) : base(_player, _stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        xInput = 0;

        if (ComboCounter > 2||Time.time>=lastTimeAttacked+comboWindow)
        {
            ComboCounter = 0;
        }
        player.anim.SetInteger("ComboCounter", ComboCounter);

        #region Choose attack direction
        
        float attackDir = player.facingDir;
        if (xInput != 0)
        {
            attackDir = xInput;
        }

        #endregion

        player.SetVelocity(player.attackMovement[ComboCounter].x * attackDir, player.attackMovement[ComboCounter].y);

        stateTimer = .1f;
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine("BusyFor", .15f);

        ComboCounter++;
        lastTimeAttacked = Time.time;
    }

    public override void Update()
    {
        base.Update();
        if (stateTimer < 0)
        {
            player.SetZeroVelocity();
        }
        if (triggerCalled)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}
