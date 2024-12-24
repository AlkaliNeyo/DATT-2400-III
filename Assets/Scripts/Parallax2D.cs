using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax2D : MonoBehaviour
{
    public float depth = 1;

    PlayerMovement player;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        float realVelocity = player.getVelocity().x / depth;
        Vector2 pos = transform.position;

        pos.x -= realVelocity * Time.fixedDeltaTime;

        if (pos.x <= -25)
            pos.x = 25;

        transform.position = pos;
    }


}
