using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Ultrahaptics;
using Valve.VR;
using Valve.VR.Extras;
using System.IO;
using System;

public class WithoutHaptics : MonoBehaviour
{

    public SteamVR_ActionSet m_ActionSet;
    public SteamVR_Action_Boolean m_BooleanAction;
    public float frequency = 200f;
    bool experiment_over = true;
    bool gameStarted = false;
    GameObject cubeBlue;
    TextMeshPro mText;
    TextMeshPro WeightIncreaseInst;
    TextMeshPro WeightDecreaseInst;
    

    List<string> currentCollisions = new List<string>();

    private float intensity;

    public SteamVR_LaserPointer laserPointer;

    Stack<float[]> contentStack = new Stack<float[]>();
    float[] intensities = new float[] { 0.6f, 0.7f, 0.8f, 0.9f, 1f };

    GameObject startButton;
    GameObject sphere;
    public int level = 1;
    float[] currentValues;
    Dictionary<float[], float> answers = new Dictionary<float[], float>();
    private string selected;
    private bool answering = false;

    List<string> BallPosition = new List<string>();


    void Start()
    {
        m_BooleanAction = SteamVR_Actions._default.GrabPinch;

        //ultrahaptics
        // Initialize the emitter
        

        sphere = GameObject.Find("Sphere");

        mText = GameObject.Find("TextInstructions").GetComponent<TextMeshPro>();
        mText.fontSize = 8;
        mText.text = "In the game, throw the ball above using your fingers and palm, " +
            "do not lift your arm, or you may hit the board in real life. If the ball touches " +
            "the blue magic cube, the weight decreases. If the ball touched the red magic " +
            "brick, the weight increases. You win by reaching the maximum weight of the ball, " +
            "if you reach the minimum weight you lose. Press start button when you are ready.";

        startButton = GameObject.Find("StartButton");

        WeightIncreaseInst = GameObject.Find("WeightIncreaseInst").GetComponent<TextMeshPro>();
        WeightDecreaseInst = GameObject.Find("WeightDecreaseInst").GetComponent<TextMeshPro>();

        laserPointer.PointerIn += PointerInside;
        cubeBlue = GameObject.Find("DynamicObjectBlue");
        experiment_over = true;
        sphere.transform.position = new UnityEngine.Vector3(-1.227f, 0.870999992f, 1421.06165f);
        startButton.SetActive(true);

    }



    // Update is called once per frame
    private void Update()
    {
        if (!IsobjectStillTouchingHand())
        {
            //print("on collision exit hand");
           
        }


        GameObject leftHand = GameObject.Find("LoPoly Rigged Hand Left");

        if (!sphere.GetComponent<Rigidbody>().useGravity)
        {
            if (experiment_over)
            {
                sphere.transform.position = new UnityEngine.Vector3(-1.227f, 0.870999992f, 1421.06165f);

            }
        }
        if (gameStarted && leftHand != null && leftHand.activeSelf)
        {
            BallPosition.Add("SpherePosition:" + sphere.transform.position.ToString() + "|BlueCubePositionX:" +
                        cubeBlue.transform.position.x + ": BlueCubeSpeed:" + cubeBlue.GetComponent<SwingFR>().speed + "level" + level);


            sphere.GetComponent<Rigidbody>().useGravity = true;
        }

        if (m_BooleanAction.stateDown)
        {
            TriggerButtonClicked();
        }
    }

    private void TriggerButtonClicked()
    {
        GameObject leftHand = GameObject.Find("LoPoly Rigged Hand Left");

        if (leftHand != null && leftHand.activeSelf && selected == "StartButton")
        {
            startButton.SetActive(false);
            gameStarted = true;
            sphere.GetComponent<Rigidbody>().useGravity = true;
            intensity = 0.7f;
            mText.text = "";
        }


        /*if left hand is null and left hand is active and sphere gravity is disabled then enable gravity*/
        else if (leftHand == null)
        {
            mText.text = "Please place your left hand on blue screen, and make sure " +
                "it is detected correctly in the scene. Try to remove and keep the hand " +
                "again, or spread your fingers.";
        }
    }



    void OnCollisionEnter(Collision collision)
    {
        currentCollisions.Add(collision.gameObject.name);
        if (collision.gameObject.name == "NFloor")
        {
            if (experiment_over)
            {
                sphere.transform.position = new UnityEngine.Vector3(-1.227f, 0.870999992f, 1421.06165f);
                sphere.GetComponent<Rigidbody>().useGravity = false;
            }
        }


        if ((collision.gameObject.name == "DynamicObject" || collision.gameObject.name == "DynamicObjectBlue") && experiment_over)
        {
            ChnageSphereWeight(collision.gameObject.name);
        }


    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "Contact Fingerbone" || collision.gameObject.name == "Contact Palm Bone")
        {
            
            BallPosition.Add("On Hand");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        print("on collision exit" + collision.gameObject.name);
        // Remove the GameObject collided with from the list.
        currentCollisions.Remove(collision.gameObject.name);
        // Print the entire list to the console.

        if (!IsobjectStillTouchingHand())
        {
            //print("on collision exit hand");
            
        }
    }


    private bool IsobjectStillTouchingHand()
    {
        if (currentCollisions.Contains("Contact Fingerbone") || currentCollisions.Contains("Contact Palm Bone"))
        {
            return true;
        }
        return false;
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        selected = e.target.name;

    }




    private void ChnageSphereWeight(string collisionName)
    {
        BallPosition.Add(collisionName);
        if (collisionName == "DynamicObject")
        {
            if (intensity == 1f)
            {
                level++;
                mText.text = "Hurray! You have won the game. You have reached level " + level + ". To play the this, press the start button.";
                cubeBlue.GetComponent<SwingFR>().decreaseSpeed();
                sphere.transform.position = new UnityEngine.Vector3(-1.227f, 0.870999992f, 1421.06165f);
               
                sphere.GetComponent<Rigidbody>().useGravity = false;
                gameStarted = false;
                startButton.SetActive(true);

                return;
            }
            intensity = intensity + 0.1f;
            WeightIncreaseInst.text = $"Weight +100Gms";

            cubeBlue.GetComponent<SwingFR>().increaseSpeed();
            StartCoroutine(ClearGameInst(WeightIncreaseInst));
        }
        else
        {

            print((float)Math.Floor(intensity * 10));
            if ((float)Math.Floor(intensity * 10) == 5)
            {
                level--;
                mText.text = "You have lost the game. The ball reached its minimum weight. To replay Level " + level + ", press the start button.";
                sphere.transform.position = new UnityEngine.Vector3(-1.227f, 0.870999992f, 1421.06165f);
               
                sphere.GetComponent<Rigidbody>().useGravity = false;
                gameStarted = false;
                startButton.SetActive(true);

                return;
            }
            intensity = intensity - 0.1f;

            cubeBlue.GetComponent<SwingFR>().decreaseSpeed();
            WeightDecreaseInst.text = $"Weight -100gms";
            StartCoroutine(ClearGameInst(WeightDecreaseInst));
        }
    }

    IEnumerator ClearGameInst(TextMeshPro TMPObject)
    {
        yield return new WaitForSeconds(2f);
        TMPObject.text = "";
    }

    public void OnApplicationQuit()
    {
        File.AppendAllText(@"D://Users/Anuj Sharma/Documents/MidAirWeightPerception/MidAirWeightPerception/dataWithoutHaptics.txt", 
            Valve.Newtonsoft.Json.JsonConvert.SerializeObject(BallPosition) + System.Environment.NewLine);
    }
}
