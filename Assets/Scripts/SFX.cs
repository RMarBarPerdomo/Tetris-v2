using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public AudioSource fall;
    public AudioSource clear;
    
    public void Fall()
    {
        fall.Play();
    }

    public void Clear()
    {
        clear.Play();
    }

}
