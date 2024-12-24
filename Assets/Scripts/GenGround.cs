
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEditor;

//Make Ground Fall, work and also the camerashake.
public class GenGround : MonoBehaviour
{   
    [SerializeField] private Transform ground_start;
    [SerializeField] private Transform ground1;

    [SerializeField] Ground ground;
    PlayerMovement player;
    public Collect collect;
    public float screenRight;
    private Vector3 lastEndPos;
    BoxCollider2D groundCollider2D;
    Transform lastgroundTransform;

    // spawn in lasers
    private float timeForLast;
    private bool isLaser = false;
    [SerializeField] GameObject spawner;

    private void Awake() {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        screenRight = (height * cam.aspect) /2;
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        groundCollider2D = ground.GetComponent<BoxCollider2D>();
        lastEndPos = new Vector3(ground_start.Find("EndPosition").position.x + (groundCollider2D.size.x*2), ground_start.Find("EndPosition").position.y-3, ground_start.Find("EndPosition").position.z);
        SpawnGround();
    }
    private void Update(){ 
        Vector2 pos;
        if(lastgroundTransform.position.x < screenRight) {

            pos = getXandY();
            lastEndPos = new Vector3(pos.x, pos.y, lastgroundTransform.Find("EndPosition").position.z);
            SpawnGround();
        }

        if(!isLaser && player.getVelocity().x >= 30) {
            Instantiate(spawner);
            isLaser = true;
        }
    }
    private void SpawnGround() {

        lastgroundTransform = SpawnGround(lastEndPos);
        groundCollider2D = lastgroundTransform.GetComponent<BoxCollider2D>();
        lastEndPos = new Vector3(lastgroundTransform.Find("EndPosition").position.x + (groundCollider2D.size.x*2), lastgroundTransform.Find("EndPosition").position.y, lastgroundTransform.Find("EndPosition").position.z);
    }
    private Transform SpawnGround(Vector3 spawnPos) {
        Transform groundTransform = Instantiate(ground1, spawnPos, Quaternion.identity);
        int collectNum = UnityEngine.Random.Range(1, 3);
        BoxCollider2D bCollider = groundTransform.GetComponent<BoxCollider2D>();



        //spawn in points
        for(int i=0; i<collectNum;i++){
            GameObject piece  = Instantiate(collect.gameObject);
            float y = bCollider.bounds.center.y + bCollider.bounds.extents.y + piece.transform.localScale.y/2;
           // print("AKSDJLASD " + y);
            float halfWidth = groundCollider2D.size.x/2 - piece.transform.localScale.x;
            float left = groundTransform.transform.position.x - halfWidth;
            float right = groundTransform.transform.position.x + halfWidth;
            float x = UnityEngine.Random.Range(left, right);
            Vector2 piecePos = new Vector2(x,y);     
            piece.transform.position = piecePos;
        }
        ground_start = groundTransform;
        ground1 = groundTransform;
        return groundTransform;
    }


    public Vector2 getXandY() {

        Vector2 pos;
         // max the player can jump
        float maxHeightHold = player.getJumpVelocity() * player.getMaxHoldTime();  
        float heightPeakTime = player.getJumpVelocity() / -player.getGravity();
        //physics equation for height of jump with gravity 
        float maxHeightPeak = player.getJumpVelocity() * heightPeakTime + (0.5f * (player.getGravity() * (heightPeakTime * heightPeakTime)));
        float maxJumpHeight = maxHeightHold + maxHeightPeak;
        float maxAllowedHeight = -14.15f;
        //highest max platform //get 50% of the max platform height so its fair for the player
        float maxFloorY = maxJumpHeight * 0.5f;
        maxFloorY += lastgroundTransform.position.y + groundCollider2D.size.y /2; 
        float minFloorY = -8f;
        float floorY = UnityEngine.Random.Range(minFloorY, maxFloorY);
        pos.y = floorY  - groundCollider2D.size.y /2; 
        if(pos.y > maxAllowedHeight) pos.y = maxAllowedHeight; 


        float t1 = heightPeakTime + player.getMaxHoldTime();
        float t2 = Mathf.Sqrt(Math.Abs((2.0f * (maxFloorY - floorY)) / - player.getGravity()));
        float totalTime = t1 + t2;
        //print("totalTime = " + totalTime);
        float maxX = totalTime * player.getVelocity().x;
        //print("player velo = " + player.getVelocity().x);
        maxX *= 0.8f;
        //print("maxX = " + maxX);
        //print("minX = " + lastgroundTransform.Find("EndPosition").position.x);
        maxX += lastgroundTransform.Find("EndPosition").position.x + groundCollider2D.size.x/2;
        float minX = lastgroundTransform.Find("EndPosition").position.x + groundCollider2D.size.x/2;

        //print(maxX + " asdasd " + minX);
        float actualX = UnityEngine.Random.Range(minX, maxX);
        pos.x = actualX + (ground.groundRight);

        return pos;
    }
    


}
