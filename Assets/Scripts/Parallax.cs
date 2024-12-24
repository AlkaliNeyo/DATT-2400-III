using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallox : MonoBehaviour
{
    Material mat;
    float distance;

     [Range(0.0f, 0.5f)]
    public float parallaxSpeed = 0.2f;

    void Start() {
        if(GetComponent<Renderer>() != null) {
            mat = GetComponent<Renderer>().material;
            
        }
        Renderer childRenderer = GetComponentInChildren<Renderer>();
        if (childRenderer != null && childRenderer.material != null && mat == null)
        {
            mat = childRenderer.material; // Overwrite mat with child's material
            Debug.Log($"Material found on child: {mat}");
        }
        else if (childRenderer == null)
        {
            Debug.LogWarning("No Renderer found in child objects!");
        }

    }
    
    void FixedUpdate() {
        distance += Time.fixedDeltaTime * parallaxSpeed;
        mat.SetTextureOffset("_MainTex", Vector2.right *distance); 
    }
}
