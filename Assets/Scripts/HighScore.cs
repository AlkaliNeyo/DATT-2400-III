using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.UI;

public class HighScore : MonoBehaviour
{
    Text highScoreText; 
    Text scoreText; 
    private static int realScore;
    void Awake() {
        int score = Mathf.FloorToInt(PlayerMovement.distance);
        if(realScore < score) {
            realScore = score;
        }
        highScoreText = GameObject.Find("HighScore").GetComponent<Text>();
        scoreText =  GameObject.Find("Score").GetComponent<Text>();
        highScoreText.text = realScore.ToString();
        scoreText.text = score.ToString();

    }
}
