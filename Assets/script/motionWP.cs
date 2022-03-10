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
        GameObject sensation = GameObject.Find("Sensation");
        sensation.SetActive(false);


    }
    bool fig = true;
    // Update is called once per frame
    void Update()
    {
        GameObject sphere = GameObject.Find("Sphere");


        GameObject leftHand = GameObject.Find("LoPoly Rigged Hand Left");

        /*if left hand is null and left hand is active and sphere gravity is disabled then enable gravity*/
        if (leftHand != null && leftHand.activeSelf && !sphere.GetComponent<Rigidbody>().useGravity)
        {
            sphere.GetComponent<Rigidbody>().useGravity = true;
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.name == "Contact Fingerbone" || collision.gameObject.name == "Contact Palm Bone") {
            GameObject sensation = GameObject.Find("Sensation");
            
            sensation.SetActive(true);

        }
    }

}
