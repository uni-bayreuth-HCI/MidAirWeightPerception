using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Ultrahaptics;

public class motionWP : MonoBehaviour
{
    

    AmplitudeModulationEmitter _emitter;
    public bool touching_hand = false;

    void Start()
    {
        GameObject sphere = GameObject.Find("Sphere");
        sphere.GetComponent<Rigidbody>().useGravity = false;

        //ultrahaptics
        // Initialize the emitter
        _emitter = new AmplitudeModulationEmitter();
        _emitter.initialize();

        

    }
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

        if (touching_hand) {

            // Set the position to be 20cm above the centre of the array Ultrahaptics.Vector3 position = new Ultrahaptics.Vector3(0.0f, 0.0f, 0.2f);
            Ultrahaptics.Vector3 position = new Ultrahaptics.Vector3(0.0f, 0.0f, 0.2f);
            // Create a control point object using this position, with full intensity, at 200Hz
            AmplitudeModulationControlPoint point = new AmplitudeModulationControlPoint(position, 0.5f, 200.0f);
            // Output this point; technically we don't need to do this every update since nothing is changing.
            _emitter.update(new List<AmplitudeModulationControlPoint> { point });
        }
       



    }

    void OnCollisionEnter(Collision collision)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.name == "Contact Fingerbone" || collision.gameObject.name == "Contact Palm Bone")
        {
            touching_hand = true;

        }
    }

    void OnDisable()
    {
        _emitter.stop();
    }

    // Ensure the emitter is immediately disposed when destroyed
    void OnDestroy()
    {
        _emitter.Dispose();
        _emitter = null;
    }

   





}
