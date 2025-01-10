using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected Enemy enemyBase;
    protected EnemyStateMachine stateMachine;
    protected Rigidbody2D rb;

    protected bool triggerCalled;
    private string animBoolName;

    protected float stateTimer;

    public EnemyState(Enemy _enemyBase,EnemyStateMachine _stateMachine,string _animBoolName)
    {
        this.enemyBase = _enemyBase;
        this.stateMachine = _stateMachine;
        this.animBoolName = _animBoolName;
    }
    public virtual void Enter()
    {
        triggerCalled = false;
        enemyBase.anim.SetBool(animBoolName, true);
        rb = enemyBase.rb;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
    }


    public virtual void Exit()
    {
        enemyBase.anim.SetBool(animBoolName, false);

    }

    public virtual void AnimationFinishTrigger()
    {
        triggerCalled = true;
    }

}
