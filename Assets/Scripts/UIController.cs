using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{   
    Text distanceText; 
    PlayerMovement player;

    void Awake() {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        distanceText = GameObject.Find("DistanceText").GetComponent<Text>();
    }



    void Update() {
        int d = Mathf.FloorToInt(PlayerMovement.distance);
        distanceText.text = d + " m";
    }


}
