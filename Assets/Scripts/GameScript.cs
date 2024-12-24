using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    public ArrayList blockList;
    private float powerUpMulti = 1f;
   [SerializeField] Slider tetrisPower;

    public float pause = 1f;
    // Start is called before the first frame update
    void Awake() {
        blockList = new ArrayList();
        blockList.Add(Random.Range(1,7));
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = pause;
    }

    public void addBlock(int n) {
        //Debug.Log("works");
        blockList.Add(n);
        tetrisPower.value += Time.fixedDeltaTime * powerUpMulti;
        blockList.Add(Random.Range(1,7));

        
    }
    public void removeBlock() {
        blockList.Remove(blockList.Count -1);
    }

    public int getLength() {
        return blockList.Count;
    }

}
