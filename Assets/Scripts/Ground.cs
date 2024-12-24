using System;
using UnityEngine;

public class Ground : MonoBehaviour
{

    PlayerMovement player;

    public Collect collect;
    public float groundHeight;
    public float groundRight;
    public float screenRight;
    BoxCollider2D boxCollider2D;

    void Awake() {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        groundHeight = boxCollider2D.bounds.center.y + boxCollider2D.bounds.extents.y;
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        screenRight = (height * cam.aspect) /2;
        //print(screenRight);
    }

    private void FixedUpdate() {

        Vector2 pos = transform.position;
        pos.x -= player.getVelocity().x * Time.fixedDeltaTime;

        //print(player.getVelocity().x + "  asd");
        groundRight = boxCollider2D.bounds.center.x + boxCollider2D.bounds.extents.x;
        //print("RIGHT " + groundRight);
        if(groundRight < (-screenRight - (boxCollider2D.size.x + 100))) {
            Destroy(gameObject);
            return;
        }
        //print("before " + transform.position);
        transform.position = pos;
        //print("after " + transform.position);
    }




}
