using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingFR : MonoBehaviour
{
    public GameObject[] waypoints;
    int current = 0;
    float rotSpeed;
    public float speed= 0.4f;
    public float WPRadius= 0.09f;
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(waypoints[current].transform.position, transform.position) < WPRadius) {
            current++;
            if (current >= waypoints.Length) {
                current = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, waypoints[current].transform.position, Time.deltaTime * speed);
    }

    public void increaseSpeed() {
        speed = speed + 0.04f;
    }

    public void decreaseSpeed()
    {
        speed = speed - 0.04f;
    }

    public void resetSpeed()
    {
        speed = 0.4f;
    }
}
