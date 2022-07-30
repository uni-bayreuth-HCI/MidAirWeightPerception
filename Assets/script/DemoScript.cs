using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Ultrahaptics;
using Valve.VR;
using Valve.VR.Extras;
using System.IO;

public class DemoScript : MonoBehaviour
{
    public SteamVR_ActionSet m_ActionSet;
    public SteamVR_Action_Boolean m_BooleanAction;
    public float frequency = 200f;
    public bool experiment_over = false;


    TextMeshPro mText;
    TextMeshPro gameText;
    AmplitudeModulationEmitter _emitter;

    List<string> currentCollisions = new List<string>();

    private float intensity;
    private float intensity2;
    private bool firstSphereProcessed = false;
    public SteamVR_LaserPointer laserPointer;

    Stack<float[]> contentStack = new Stack<float[]>();
    float[] intensities = new float[] { 0.6f, 0.7f, 0.8f, 0.9f, 1f };


    GameObject buttons;
    GameObject startButton;
    GameObject dynamicObject;
    GameObject sphere;


    float[] currentValues;
    Dictionary<float[], float> answers = new Dictionary<float[], float>();
    private string selected;
    private bool answering = false;


    void Start()
    {

        FillIntensityValues();

        m_BooleanAction = SteamVR_Actions._default.GrabPinch;

        //ultrahaptics
        // Initialize the emitter
        _emitter = new AmplitudeModulationEmitter();
        _emitter.initialize();

        sphere = GameObject.Find("Sphere");

        mText = GameObject.Find("TextInstructions").GetComponent<TextMeshPro>();
        mText.text = "Welcome to the Haptics User Study.";
        buttons = GameObject.Find("Buttons");
        startButton = GameObject.Find("StartButton");
        dynamicObject = GameObject.Find("DynamicObject");
        buttons.SetActive(false);
        startButton.SetActive(false);
        dynamicObject.SetActive(false);


        gameText = GameObject.Find("GameInstructions").GetComponent<TextMeshPro>();

        laserPointer.PointerIn += PointerInside;
    }



    // Update is called once per frame
    private void Update()
    {
        if (m_BooleanAction.stateDown)
        {
            TriggerButtonClicked();
        }
    }

    private void TriggerButtonClicked()
    {

        if (IsButtonClicked())
        {
            return;
        }

        GameObject leftHand = GameObject.Find("LoPoly Rigged Hand Left");
        /*if left hand is null and left hand is active and sphere gravity is disabled then enable gravity*/
        if (leftHand != null && leftHand.activeSelf && !answering)
        {
            if (!experiment_over)
            {

                if (firstSphereProcessed)
                {
                    StartCoroutine(ProcessSecondBall());
                }
                else
                {
                    StartCoroutine(ProcessFirstBall());
                }
            }
            else
            {
                sphere.GetComponent<Rigidbody>().useGravity = true;
            }


        }
        else if (leftHand == null)
        {
            mText.text = "Please place your left hand on white paper, and make sure it is detected correctly in the scene. Try to remove and keep the hand again, or spread your fingers";
        }
    }

    private bool IsButtonClicked()
    {
        if (selected == "BallButton1" || selected == "BallButton2")
        {
            if (selected == "BallButton1")
            {
                answers.Add(currentValues, currentValues[0]);
            }
            else if (selected == "BallButton2")
            {
                answers.Add(currentValues, currentValues[1]);
            }
            answering = false;

            buttons.SetActive(false);

            firstSphereProcessed = false;
            mText.text = "This is the first ball for the new case. please press trigger button to make it fall on your hand.";
            if (contentStack.Count == 0)
            {
                File.AppendAllText(@"D://Users/Anuj Sharma/Documents/MidAirWeightPerception/MidAirWeightPerception/answers.json", Valve.Newtonsoft.Json.JsonConvert.SerializeObject(answers) + System.Environment.NewLine);

                experiment_over = true;
                mText.fontSize = 8;
                mText.text = "This was one part of the experiment. We will start the game now. In the game, throw the ball above using your fingers and make the ball touch the magic cube above. Ball will magically change weight as it touch the brick above. Click ready button whenever you are ready.";

                sphere.transform.position = new UnityEngine.Vector3(-1.20899999f, 1.06599998f, 1421f);

                dynamicObject.SetActive(true);
                startButton.SetActive(true);

            }


            return true;
        }
        if (selected == "StartButton")
        {

            startButton.SetActive(false);
            intensity = 0.6f;
            sphere = GameObject.Find("Sphere");
            sphere.GetComponent<Rigidbody>().useGravity = true;
            mText.text = "";
        }

        return false;
    }


    IEnumerator ProcessFirstBall()
    {

        currentValues = contentStack.Pop();
        intensity = currentValues[0];
        intensity2 = currentValues[1];
        mText.text = "This ball will be on your hand for 2 seconds, and then it will disappear.";
        //print("Inside Sphere Coroutine");
        sphere.GetComponent<Rigidbody>().useGravity = true;
        //print("Gravity True");
        yield return new WaitForSeconds(2f);

        sphere.transform.position = new UnityEngine.Vector3(-1.20899999f, 1.27199996f, 1421);
        sphere.GetComponent<Rigidbody>().useGravity = false;

        _emitter.stop();
        mText.text = "This is the second ball, press the trigger button to make it fall on your hand.";
        firstSphereProcessed = true;

    }



    IEnumerator ProcessSecondBall()
    {

        intensity = intensity2;
        print("processing second ball");
        mText.text = "This ball will be on your hand for 2 seconds, and then it will disappear.";
        //print("Inside Sphere Coroutine");
        sphere.GetComponent<Rigidbody>().useGravity = true;
        //print("Gravity True");
        yield return new WaitForSeconds(2f);

        sphere.transform.position = new UnityEngine.Vector3(-1.20899999f, 1.27199996f, 1421);
        sphere.GetComponent<Rigidbody>().useGravity = false;
        //Vector3(-1.20899999, 1.27199996, 1421)
        _emitter.stop();
        answering = true;
        //print("Which ball was heavy");
        mText.text = "Which ball was heavier?";
        buttons.SetActive(true);
    }


    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.name == "ComputerDesk" || collision.gameObject.name == "PaperTray")
        {
            if (experiment_over)
            {
                sphere.transform.position = new UnityEngine.Vector3(-1.20899999f, 1.06599998f, 1421f);
                sphere.GetComponent<Rigidbody>().useGravity = false;
                sphere.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
                gameText.text = "press trigger button once hand is detected";
            }
        }
        currentCollisions.Add(collision.gameObject.name);

        if (collision.gameObject.name == "DynamicObject")
        {
            ChnageSphereWeight();
        }


    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name == "Contact Fingerbone" || collision.gameObject.name == "Contact Palm Bone")
        {
            Ultrahaptics.Vector3 position = new Ultrahaptics.Vector3(0.0f, 0.0f, 0.2f);
            AmplitudeModulationControlPoint point = new AmplitudeModulationControlPoint(position, intensity, 180f);
            _emitter.update(new List<AmplitudeModulationControlPoint> { point });
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Remove the GameObject collided with from the list.
        currentCollisions.Remove(collision.gameObject.name);
        // Print the entire list to the console.

        if (!IsobjectStillTouchingHand())
        {
            //print("on collision exit hand");
            _emitter.stop();
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


    private void FillIntensityValues()
    {
        float[] arr = new float[2] { 0.7f, 0.7f };
        contentStack.Push(arr);
    }

    private void ChnageSphereWeight()
    {
        //int index = Random.Range(0, intensities.Length);
        //float difference = intensities[index] - intensity;
        //if (difference > 0)
        //{
        //    gameText.text = $"Weight increased by {Mathf.Round(difference * 1000)}Gms";
        //}
        //else if (difference < 0)
        //{
        //    gameText.text = $"Weight decreased by {Mathf.Round(-difference * 1000)}Gms";
        //}
        //else
        //{
        //    gameText.text = $"Weight remains the same.";
        //}
        //intensity = intensities[index];

        gameText.text = $"Every time you toss the ball, and it touches the magic cube above, it will change its weight. try to toss the ball using your fingers and palm only. please dont lift your forearm.";
        intensity = 1.0f;
    }




}
