using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ultrahaptics;
using Valve.VR;
using Valve.VR.Extras;
using UnityEngine.EventSystems;
using System.IO;

public class ControllerOfExperiment : MonoBehaviour
{

    private SteamVR_Action_Boolean m_BooleanAction;
    private float intensity;
    private float intensity2;
    private bool firstSphereProcessed = false;

    TextMeshPro mText;
    GameObject buttons;
    AmplitudeModulationEmitter _emitter;

    Stack<float[]> contentStack = new Stack<float[]>();
    float[] currentValues;
    Dictionary<float[], float> answers = new Dictionary<float[], float>();
    public SteamVR_LaserPointer laserPointer;
    private string selected;
    private bool answering = false;

    void Start()
    {
        float[] arr1 = new float[2] { 0.6f, 0.8f };
        float[] arr2 = new float[2] { 1.0f, 0.8f };
        float[] arr3 = new float[2] { 0.6f, 1.0f };
        float[] arr4 = new float[2] { 0.5f, 1.0f };
        float[] arr5 = new float[2] { 0.5f, 0.6f };
        float[] arr6 = new float[2] { 0.7f, 0.6f };
        float[] arr7 = new float[2] { 0.7f, 0.9f };
        contentStack.Push(arr1);
        contentStack.Push(arr2);
        contentStack.Push(arr3);
        contentStack.Push(arr4);
        contentStack.Push(arr5);
        contentStack.Push(arr6);
        contentStack.Push(arr7);


        m_BooleanAction = SteamVR_Actions._default.GrabPinch;

        //ultrahaptics
        // Initialize the emitter
        _emitter = new AmplitudeModulationEmitter();
        _emitter.initialize();


        mText = GameObject.Find("TextInstructions").GetComponent<TextMeshPro>();
        mText.text = "Welcome to the Haptics User Study. AI8(Serious Games) University Of Bayreuth. Press trigger button to continue. We will take zou through the demo of entire experiment there will be 3 cases. Each case will be presented further. Lets look at the first case Press the trigger button to continue.";
        buttons = GameObject.Find("Buttons");
        buttons.SetActive(false);

        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;


    }
    public void PointerInside(object sender, PointerEventArgs e)
    {
        selected = e.target.name;
    }
    public void PointerOutside(object sender, PointerEventArgs e)
    {

        print(e.target.name);
    }


    private void EnableButtons()
    {
        buttons.SetActive(true);
    }


    // Update is called once per frame
    private void Update()
    {
        if (m_BooleanAction.stateDown)
        {
            TriggerButtonClicked();
        }
    }

    private void TriggerButtonClicked() {

        if (IsButtonClicked()) {
            return;
        }

        GameObject leftHand = GameObject.Find("LoPoly Rigged Hand Left");
        /*if left hand is null and left hand is active and sphere gravity is disabled then enable gravity*/
        if (leftHand != null && leftHand.activeSelf && !answering)
        {
            if (contentStack.Count == 0) {
                mText.text = "Game Over.";
                return;
            }
            if (firstSphereProcessed)
            {
                ProcessSecondBall();
            }
            else
            {
                ProcessFirstBall();
            }
        }
        else if (leftHand == null)
        {
            mText.text = "Please place your left hand on white paper, and make sure it is detected correctly in the scene. Try to remove and keep the hand again, or spread your fingers";
        }
    }

    private bool IsButtonClicked() {
        if (selected == "BallButton1" || selected == "BallButton2") {
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
            if (contentStack.Count == 0) {
                
                File.AppendAllText(@"answers.json", Valve.Newtonsoft.Json.JsonConvert.SerializeObject(answers) + System.Environment.NewLine);
            }
            return true;
        }

        return false;
    }

    private void ProcessFirstBall() {
        GameObject sphere = GameObject.Find("Sphere");
        currentValues = contentStack.Pop();
        intensity = currentValues[0];
        intensity2 = currentValues[1];

        StartCoroutine(DisableSphereCoroutine(sphere));
        mText.text = "This is the second ball, press the trigger button to make it fall on your hand.";
    }

    private void ProcessSecondBall() {
        intensity = intensity2;
        StartCoroutine(DisableSphereCoroutine(GameObject.Find("Sphere")));
        answering = true;
        mText.text = "Which ball was heavier?";
        buttons.SetActive(true);
    }

    IEnumerator DisableSphereCoroutine(GameObject sphere)
    {
        mText.text = "This ball will be on your hand for 3 seconds, and then it will disappear.";
        sphere.GetComponent<Rigidbody>().useGravity = true;
        yield return new WaitForSeconds(3f);

        sphere.GetComponent<Rigidbody>().useGravity = false;
        sphere.transform.position = new UnityEngine.Vector3(-1.51f, 9.09f, 2.298f);
        
        _emitter.Dispose();
        _emitter = null;
    }


}
