using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame() {
   PlayerMovement.distance = 0;
    SceneManager.LoadSceneAsync(1);
   }
   public void PlayCredits() {
    SceneManager.LoadSceneAsync(2);
   }
   public void PlayMenu() {
    SceneManager.LoadSceneAsync(0);
   }
   public void PlayHowToPlay() {
    SceneManager.LoadSceneAsync(4);
   }
   public void PlayExitGame() {
    Application.Quit();
}

}
