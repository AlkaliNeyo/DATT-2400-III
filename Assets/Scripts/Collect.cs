using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Collect : MonoBehaviour
{
    PlayerMovement player;
    public int state;
    void Awake() {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        this.state = Random.Range(1,7);
    }

    void FixedUpdate() {
        Vector2 pos = transform.position;

        pos.x -= player.getVelocity().x * Time.fixedDeltaTime;
        if(pos.x < -30) {
            Destroy(gameObject);
            return;
        }

        transform.position = pos;
    }

    public int getState() {
        return this.state;
    }
}
