using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim; // Animator 组件
    public string id; // 唯一 ID
    public bool activationStatus; // 激活状态

    private void Awake()
    {
        // 提前初始化 Animator
        anim = GetComponent<Animator>();

    }

    private void Start()
    {
        
    }

    [ContextMenu("Generate checkpoint id")]
    private void GenerateId()
    {
        id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.GetComponent<Player>() != null)
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {

        if (activationStatus == false) 
        {
            AudioManager.instance.PlaySFX(4, transform);
        }
        if (anim == null)
        {
            return;
        }

        activationStatus = true;
        anim.SetBool("active", true);
    }
}