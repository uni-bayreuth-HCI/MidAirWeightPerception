using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCubeController : MonoBehaviour
{
    public Animation youranimation;
    void OnCollisionEnter()
    {
        youranimation.Play();
    }
}
