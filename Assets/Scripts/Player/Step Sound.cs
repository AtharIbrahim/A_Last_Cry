using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSound : MonoBehaviour
{
    public AudioSource step;
    public AudioClip footstep;

    public AudioSource idle;
    public AudioClip idleBreath;

    void footsound(){
        step.clip = footstep;
        step.Play();
    }



    void IdleSound(){
        idle.clip = idleBreath;
        idle.Play();
    }
}
