using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Ultrahaptics;
using Valve.VR;
using Valve.VR.Extras;
using System.IO;

public class MotionWP_FR : MonoBehaviour
{
    public SteamVR_ActionSet m_ActionSet;
    public SteamVR_Action_Boolean m_BooleanAction;
    public float frequency = 200f;
    public bool experiment_over = false;
    public bool enableGravity = false;
    bool gameStarted = false;

    TextMeshPro mText;
    TextMeshPro WeightIncreaseInst;
    TextMeshPro WeightDecreaseInst;
    AmplitudeModulationEmitter _emitter;

    List<string> currentCollisions = new List<string>();

    private float intensity;
    private float intensity2;
    private bool firstSphereProcessed = false;
    public SteamVR_LaserPointer laserPointer;

    Stack<float[]> contentStack = new Stack<float[]>();
    float[] intensities = new float[] {0.6f, 0.7f, 0.8f, 0.9f, 1f};


    GameObject buttons;
    GameObject startButton;
    GameObject dynamicObject;
    GameObject dynamicObjectBlue;
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
        mText.text = "Welcome to the Haptics User Study. Press trigger buttin to star the experiment.";
        buttons = GameObject.Find("Buttons");
        startButton = GameObject.Find("StartButton");
        dynamicObject = GameObject.Find("DynamicObject");
        dynamicObjectBlue = GameObject.Find("DynamicObjectBlue");

        buttons.SetActive(false);
        startButton.SetActive(false);
        dynamicObject.SetActive(false);
        dynamicObjectBlue.SetActive(false);

        WeightIncreaseInst = GameObject.Find("WeightIncreaseInst").GetComponent<TextMeshPro>();
        WeightDecreaseInst = GameObject.Find("WeightDecreaseInst").GetComponent<TextMeshPro>();

        laserPointer.PointerIn += PointerInside;
    }



    // Update is called once per frame
    private void Update()
    {
        GameObject leftHand = GameObject.Find("LoPoly Rigged Hand Left");
        if (gameStarted && leftHand != null && leftHand.activeSelf)
        {
            sphere.GetComponent<Rigidbody>().useGravity = true;
        }
        if (!sphere.GetComponent<Rigidbody>().useGravity) {
            if (!experiment_over)
            {
                sphere.transform.position = new UnityEngine.Vector3(-1.20899999f, 1.27199996f, 1421);
            }
            else if (experiment_over) {
                sphere.transform.position = new UnityEngine.Vector3(-1.20899999f, 0.927100003f, 1421f);
            }

        }

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
                mText.text = "Experiment is over. Please wait we will play a game now.";

                sphere.transform.position = new UnityEngine.Vector3(-1.20899999f, 0.927100003f, 1421f);

                intensity = 0.8f;
                dynamicObject.SetActive(true);
                dynamicObjectBlue.SetActive(true);
                //startButton.SetActive(true);
            }

            return true;
        }
        if (selected == "StartButton") {
            
            startButton.SetActive(false);
            gameStarted = true;
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
        print("gravity false 2");

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
        print("gravity false 3");
        //Vector3(-1.20899999, 1.27199996, 1421)
        _emitter.stop();
        answering = true;
        //print("Which ball was heavy");
        mText.text = "Which ball was heavier?";
        buttons.SetActive(true);
    }


    void OnCollisionEnter(Collision collision)
    {
        currentCollisions.Add(collision.gameObject.name);

        if (collision.gameObject.name == "ComputerDesk" || collision.gameObject.name == "PaperTray") {
            if (experiment_over) {
                sphere.transform.position = new UnityEngine.Vector3(-1.20899999f, 0.927100003f, 1421f);
                sphere.GetComponent<Rigidbody>().useGravity = false;
            }
        }
        

        if ((collision.gameObject.name == "DynamicObject" || collision.gameObject.name == "DynamicObjectBlue") && experiment_over) {
            ChnageSphereWeight(collision.gameObject.name);
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
    

    private void FillIntensityValues() {
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
        
    }
    
    private void ChnageSphereWeight(string collisionName) {
        if (collisionName == "DynamicObject") {
            if (intensity == 1f){
                
                mText.text = "Hurray! you have won the game. To play the game again press start button.";
                sphere.transform.position = new UnityEngine.Vector3(-1.20899999f, 0.927100003f, 1421f);
                gameStarted = false;
                startButton.SetActive(true);
                intensity = 0.8f;
                return;
            }
                intensity = intensity + 0.1f;
            WeightIncreaseInst.text = $"Weight +100Gms";
            WeightDecreaseInst.text = $"";
            StartCoroutine(ClearGameInst(WeightIncreaseInst));

        } else {
            if (intensity == 0.6f) {
                mText.text = "You have lost the game. The ball reached its minimum weight. To restart the game press start button.";
                sphere.transform.position = new UnityEngine.Vector3(-1.20899999f, 0.927100003f, 1421f);
                gameStarted = false;
                startButton.SetActive(true);
                gameStarted = false;
                intensity = 0.8f;
                return;
            }
            intensity = intensity - 0.1f;
            WeightIncreaseInst.text = "";
            WeightDecreaseInst.text = $"Weight -100gms";
            StartCoroutine(ClearGameInst(WeightDecreaseInst));
        }
    }

    IEnumerator ClearGameInst(TextMeshPro TMPObject) {
        yield return new WaitForSeconds(2f);
        TMPObject.text = "";
    }


}
