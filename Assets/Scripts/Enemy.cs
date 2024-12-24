using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;


//using System.Numerics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;


public class Enemy : MonoBehaviour
{
    public GameObject projectile;
    public PlayerMovement player;
    private Animator anim;
    private float angle;
    private Vector2 dir;
    private float xOffset = 10f;
    private Vector3 velocity = Vector3.zero;
    
    private bool canShoot = true;
    public bool canShootPlayer = true;
    public float smoothTime = 0.3f; 
    private Vector3 finalHeight = new Vector3(0,4,0); // offset of 8 
    public LayerMask groundLayerMask;
    GameObject obstacle;

    public ObjectSpawner objectSpawner;

    private bool isShot = false;
    private void Awake() {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        anim = GetComponent<Animator>();
        finalHeight.x = Random.Range(transform.position.x-xOffset, transform.position.x+xOffset);
        //objectSpawner = GameObject.Find("ObjectSpawner").GetComponent<ObjectSpawner>();
        if(finalHeight.x < - 18) finalHeight.x = -18;
        else if(finalHeight.x > 25) finalHeight.x = 25;
    }

    public void SetSpawner(ObjectSpawner objectSpawner) {
        this.objectSpawner = objectSpawner;
    }

    public void Stopped() {
        canShoot = false; 
    }
    private void Update() {
    
        Move();
        //print("ASDASASDASDSA " + objectSpawner != null);
        if(canShoot) {
            anim.SetTrigger("isShooting");
            isShot = true;
        }
        
    }
    private void FixedUpdate() {
        SetFinalHeight();

        if(canShootPlayer == false) {
            anim.SetTrigger("isDead");
        }
    }

    public void CanShoot() {
        canShoot =true;
    }

    public void Shoot() {
        //float angle;
        //print(canShoot + " " + canShootPlayer);
        if(canShoot == true && canShootPlayer == true) {
            isShooting();
        }
    }

    public void isShooting() {
            dir = (player.transform.position - transform.position).normalized;
            angle = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,angle);
            anim.SetTrigger("isShoot");
            obstacle = Instantiate(projectile, transform.position, Quaternion.identity);
            obstacle.transform.rotation = Quaternion.Euler(0, 0, -angle);
            obstacle.GetComponent<Projectile>().SetEnemy(this);
            canShoot = false;
    }
    private void Move() {
        Vector2 pos = transform.position;
        dir = (finalHeight - transform.position).normalized;
        angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        transform.position = Vector3.SmoothDamp(transform.position, finalHeight, ref velocity, smoothTime);
    }

    public void Delete() {
        //print(objectSpawner != null);
        if(objectSpawner != null) objectSpawner.RemoveBot(this);
        objectSpawner.canSpawn = true;
        canShoot = true;
        Destroy(gameObject);
    }

    public void Destory() {
        canShootPlayer = false;
        anim.SetTrigger("isDead");
        Debug.Log("Destroying");
    }


    private void SetFinalHeight() {
            Vector2 pos = transform.position;
            Vector2 rayOrigin = new Vector2(pos.x + 0.5f, pos.y);
            Vector2 rayDir = Vector2.up;
            float rayDist = -8;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDir, rayDist, groundLayerMask);
            if(hit2D.collider != null)   {
                Ground ground = hit2D.collider.GetComponent<Ground>();
                
                if(ground != null)
                {   
                    if(pos.y >= ground.groundHeight - 0.5f) 
                    {   
                        finalHeight.y += ground.groundHeight;
                    }
                    if(finalHeight.y < ground.groundHeight) finalHeight.y = ground.groundHeight + 8f;
                }
            } 
        Debug.DrawRay(rayOrigin, rayDir * rayDist, Color.red);
        
    }
}
