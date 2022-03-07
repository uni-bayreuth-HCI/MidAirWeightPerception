using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class motionWP : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject sphere = GameObject.Find("Sphere");
        sphere.GetComponent<Rigidbody>().useGravity = false;
       

    }

    // Update is called once per frame
    void Update()
    {
        GameObject sphere = GameObject.Find("Sphere");
        

        GameObject leftHand = GameObject.Find("LoPoly Rigged Hand Left");
        if (leftHand != null && leftHand.activeSelf)
        {
            sphere.GetComponent<Rigidbody>().useGravity = true;
        }

    }
    
}
