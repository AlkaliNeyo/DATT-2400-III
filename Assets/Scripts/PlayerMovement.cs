
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float gravity = -9f;
    [SerializeField] private float acceleration = 10;
    [SerializeField] private float maxAcceleration = 25;
    [SerializeField] private float maxXVelocity = 100;
    [SerializeField] private Vector2 velocity;
    [SerializeField] private float jumpVelocity = 10f;
    [SerializeField] private float moveVelocity = 0.01f;
    [SerializeField] private float groundHeight = -5f;
    [SerializeField] private float maxHoldTime = 0.4f;
    [SerializeField] private float jumpThreshHold = 0.5f;
    [SerializeField] private float finalMaxHoldTime = 0.2f;
    [SerializeField] private bool isDead = false;
    [SerializeField] private float botScore = 5000;

    private Animator anim;
    private float frozenTimeScale;
    private float powerUpMulti = 5f;
    private float powerUpMax = 1f;
    private float timeMulti = 0.05f;
    [SerializeField] Slider tetrisPower;
    private float holdTime = 0.0f;
    public bool isGrounded = false;
    public bool canAdd = true;
    public static float distance = 0;
    private bool isHold = false;
    public bool isPowerHold = false;

    private float detectionRadius;

    CameraController cameraController;
    public LayerMask groundLayerMask;
    public LayerMask ObjLayerMask;
    public LayerMask enemyLayerMask;
    public LayerMask laserLayerMask;
    public AudioSource audioName;
    public AudioSource collectName;
    private List<SpriteRenderer> highlightedEnemies = new List<SpriteRenderer>();

   

    void Start() {
        cameraController = Camera.main.GetComponent<CameraController>();
        anim = GetComponent<Animator>();
        detectionRadius = 0.3f;
    }

    public void PlayerDead() {
        SceneManager.LoadSceneAsync(3);
    }
    public void BeenShot() {
        //print("beenShot");
        tetrisPower.value -= Time.fixedUnscaledDeltaTime * powerUpMulti;
    }
    
    
    void Update() {
        Vector2 pos = transform.position;
        //print(pos);

        PlayerCheck();
        float groundDist = Mathf.Abs(pos.y - groundHeight);

        if (isGrounded || groundDist <= jumpThreshHold) 
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                isGrounded = false;
                velocity.y = jumpVelocity;
                isHold = true;
                anim.SetBool("isJump", true);
                anim.SetBool("isGround", false);
            }
        }

        if(Input.GetKeyUp(KeyCode.Space)) {
            isHold = false;
            anim.SetBool("isJump", false);
        } 
        if(Input.GetKey(KeyCode.E)  && ((tetrisPower.value > powerUpMax/2 ) || isPowerHold)) {
            if(!isPowerHold) {
                frozenTimeScale = Time.fixedDeltaTime;
            }
                isPowerHold = true;
                TimeSlow();   
        } else {
            isPowerHold = false;
            TimeReset();
        }

        if(isPowerHold == true) {
            pos = transform.position;
            if(Input.GetKey(KeyCode.UpArrow)) {
                pos.y += moveVelocity * frozenTimeScale;
                //print(pos.x);
            } 
            if(Input.GetKey(KeyCode.RightArrow)) {
                pos.x += moveVelocity * (frozenTimeScale/2);
                //print(pos.x);
            } 
            if(Input.GetKey(KeyCode.LeftArrow)) {
                pos.x -= moveVelocity * (frozenTimeScale/2);
                //print(pos.x);
            }
            transform.position = pos;
        }

        //collsion for laser/projectiles
        
        Collider2D[] lHits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, laserLayerMask);
        foreach (Collider2D hit in lHits)
        {   
            Projectile t = hit.GetComponent<Projectile>();

            if( t != null) {
                //print("T is here " + t.enemyType);
                if(t.enemyType == 0) {
                    PlayerDead();
                }
                if(t.enemyType == 1) {
                    t.Delete();
                    BeenShot();
                }

            }
            
        }
 
        //collsion with enemy and attack range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius*30, enemyLayerMask);
        List<SpriteRenderer> currentHighlighted = new List<SpriteRenderer>();
            //for detecting enemies 
            //print(hits.Count());
            Collider2D closestEnemy = null;
            float shortestDistance = Mathf.Infinity;
            
            foreach (Collider2D hit in hits)
            {
                if(hit.GetComponent<Enemy>() != null) {
                    //print(hit);
                    // Highlight the enemy (e.g., change color or show an icon)
                    SpriteRenderer renderer = hit.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {   
                        renderer.color = Color.green; // Highlight enemy 
                        currentHighlighted.Add(renderer);
                    }
                    // Calculate the distance to this enemy
                    float distance = Vector2.Distance(transform.position, hit.transform.position);
                    // Check if this is the closest enemy
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        closestEnemy = hit;
                    }
                }
            
            }
            //unhighlight them
            foreach (SpriteRenderer renderer in highlightedEnemies)
            {
                    if (!currentHighlighted.Contains(renderer) && renderer != null)
                    {
                        renderer.color = Color.white; // Reset to default color
                    }
            }
            highlightedEnemies = currentHighlighted;
                

                // Attack the closest enemy (if any)
                if (closestEnemy != null)
                {   
                    if(Input.GetKeyDown(KeyCode.Q)  && (tetrisPower.value >= powerUpMax)) {
                        Enemy e = closestEnemy.GetComponent<Enemy>();
                        if(e.transform.position.x < 28 || e.transform.position.x > -28 || e.transform.position.y > -10 || e.transform.position.y < 7 ) 
                        {
                            Contact(e);
                            transform.position = e.transform.position;
                        }
                        
                    } 
                }
                closestEnemy = null;

        //Debug.DrawRay(transform.position, transform.position + Vector3.up * detectionRadius, Color.green);
        //collsion with Coin
        Collider2D[] cHits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, ObjLayerMask);

        foreach (Collider2D hit in cHits)
        {   
            Collect collect = hit.GetComponent<Collect>();
            collectName.Play();
            HitCollect(collect);
        }


        //attacking enemy
        Collider2D[] bHits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayerMask);

        foreach (Collider2D hit in bHits)
        {   
            Enemy e = hit.GetComponent<Enemy>();
            if(e != null) {
                Contact(e);
                }
        }

        


        // draw detection radius
        //Debug.DrawLine(transform.position, transform.position + Vector3.up * detectionRadius, Color.green);


    }

    public void Contact(Enemy e) {
        e.Destory();
        if(canAdd){
            distance += (velocity.x * Time.fixedUnscaledDeltaTime)*30;
            canAdd = false;
        }
        tetrisPower.value = 0;  
        anim.SetTrigger("isAttack");
        PlayAudio();
    }

    public void PlayAudio()
    {
        audioName.Play();
    }

    private void PlayerCheck() {
        if(transform.position.x > 31.5 || transform.position.x < -31.5 || transform.position.y < -12 ) {
            PlayerDead();
        }
    }

    public Vector2 getVelocity() {
        return velocity;
    }
    public float getJumpVelocity() {
        return jumpVelocity;
    }
    public float getMaxHoldTime() {
        return maxHoldTime;
    }
    public float getGravity() {
        return gravity;
    }
    public float getMaxVelocity() {
        return maxXVelocity;
    }

    public float getAcceleration() {
        return acceleration;
    }

    private void FixedUpdate() {
        Vector2 pos = transform.position;
        if(pos.y < -20) {
            PlayerDead();
        }
        if(isDead == true) {
            Time.timeScale = 0f;
            velocity = new Vector2(0,0);
        }

        //in the air
        if(!isGrounded) 
        {
            //holding jump button?
            if(isHold) 
            {
                holdTime += Time.fixedDeltaTime;
                if(holdTime >= maxHoldTime) {
                    isHold = false;
                }
            }

            //jumping movement 
            pos.y += velocity.y * Time.fixedDeltaTime;
            if(!isHold) {
                velocity.y += gravity * Time.fixedDeltaTime;
                holdTime = 0.0f;
            }

            //check for collsion 
            Vector2 rayOrigin = new Vector2(pos.x + 0.5f, pos.y);
            Vector2 rayDir = Vector2.up;
            float rayDist = velocity.y * Time.fixedUnscaledDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDir, rayDist, groundLayerMask);
            if(hit2D.collider != null)   {
                Ground ground = hit2D.collider.GetComponent<Ground>();
                if(ground != null)
                {   
                    //print(pos.y >= ground.groundHeight);
                    //print(ground.groundHeight + " " + pos.y);
                    if(pos.y <= ground.groundHeight) 
                    {   
                        groundHeight = ground.groundHeight;
                        //print("PLAYER " + groundHeight);
                        pos.y = groundHeight;
                        velocity.y=0;
                        isGrounded = true;
                        anim.SetBool("isGround", true);
                    }
                }
            } 

            Debug.DrawRay(rayOrigin, rayDir * rayDist, Color.red);
        }

        Vector2 wallOrgin = new Vector2(pos.x+0.1f,pos.y);
        RaycastHit2D wallHit = Physics2D.Raycast(wallOrgin, Vector2.right, velocity.x * Time.fixedDeltaTime);

        if(wallHit.collider != null) 
        {
            Ground ground = wallHit.collider.GetComponent<Ground>();
            if(ground != null) 
            {
                if(pos.y < ground.groundHeight-1) {
                    velocity.x = 0;
                    Time.timeScale = 0f;
                }
            }
        }

        distance += velocity.x * Time.fixedDeltaTime;
        //On Ground
        if(isGrounded) {
            anim.SetBool("isJump", false);
            canAdd = true;
            float veloRatio = velocity.x / maxXVelocity;

            acceleration = maxAcceleration * (1 - veloRatio);
            maxHoldTime = finalMaxHoldTime * veloRatio;
            velocity.x += acceleration * Time.fixedDeltaTime;
            
            if(velocity.x >= maxXVelocity) {
                velocity.x = maxXVelocity;
            }
            Vector2 rayOrigin = new Vector2(pos.x - 0.3f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedUnscaledDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
            if (hit2D.collider == null)
            {
                isGrounded = false;
            }
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.yellow);
        }



        //collsion with collect:

        /*Vector2 collectOrgin = new Vector2(pos.x, pos.y);
        RaycastHit2D collectHitX = Physics2D.Raycast(collectOrgin, Vector2.right, velocity.x * Time.fixedUnscaledDeltaTime, ObjLayerMask);

        if(collectHitX.collider != null) {
            Collect collect = collectHitX.collider.GetComponent<Collect>();
            if(collect != null) {
                hitCollect(collect);
            }
        }
        RaycastHit2D collectHitY = Physics2D.Raycast(collectOrgin, Vector2.up, velocity.y * Time.fixedUnscaledDeltaTime, ObjLayerMask);
        if(collectHitY.collider != null) {
            Collect collect = collectHitY.collider.GetComponent<Collect>();
            if(collect != null) {
                hitCollect(collect);
            }
        } */
        transform.position = pos;
        
    }


    public void HitCollect(Collect c) {
        //velocity.x *= 0.6f; //change to end game later
        tetrisPower.value += Time.fixedUnscaledDeltaTime * powerUpMulti*3;
        Destroy(c.gameObject);
    }

    private void TimeSlow() {
        if(isPowerHold == true) {
            tetrisPower.value -= Time.fixedUnscaledDeltaTime * timeMulti;
            if(tetrisPower.value <= 0) {
                isPowerHold = false;
                TimeReset();
            }
            while(Time.timeScale > 0.2f) {
            Time.timeScale -= Time.fixedUnscaledDeltaTime * timeMulti;
        }
        }
    }
    private void TimeReset() {
        while(Time.timeScale < 1) {
            Time.timeScale += Time.fixedUnscaledDeltaTime * powerUpMulti;
        }
        if(Time.timeScale > 1) Time.timeScale = 1;

    }
}
