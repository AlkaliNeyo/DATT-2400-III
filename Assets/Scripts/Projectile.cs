using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Unity.VisualScripting;


//using System.Numerics;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public PlayerMovement player;
    private Animator anim;

    private float angle;
    private float moveSpd = 18f;
    private Vector2 dir;
    private Vector3 target;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.3f; 
    private float detectionRadius = 0.5f;
    public LayerMask allLayerMask;
    public float enemyType;
    Enemy e;

    private void Awake() {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        target = player.transform.position;
        //print("TARGET " + target);
        dir = (player.transform.position - transform.position).normalized;
        angle = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
    }

    public void SetEnemy(Enemy e) {
        this.e = e;
    }
    private void Update() {

        if(enemyType != 0) { 
            //collsion with laser
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, allLayerMask);
            Move();
            foreach (Collider2D hit in hits)
            {
                //print(hit);
                anim.SetBool("isHit", true);
                moveSpd = 0;
            }
            Check();
        }

    }

    private void Check() {
        if(transform.position.y < -50 || transform.position.x < -30 ||transform.position.y > 50 || transform.position.x > 40 ) {
            Delete();
        }
    }

    private void Move() {
        Vector2 pos = transform.position;
        transform.position += (Vector3)(dir * moveSpd * Time.unscaledDeltaTime);
    }

    public void Delete(){
    e.CanShoot();
    Destroy(gameObject);
    }
}
