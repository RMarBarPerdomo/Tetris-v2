using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using SpeechIO;
using System.Threading.Tasks;
using System;


public class Feel : MonoBehaviour
{
    private SpeechOut speechOut = new SpeechOut();
    static int CurrentNode = 0;
    public static GameObject[] outline;
    //public static GameObject lehandle;
    float threshold = 0.1f;
    static GameObject CurrentPositionHolder;
    static bool feel_finished = false;
    public static int fallmove = 25;
    public static UpperHandle upperHandle;
    string tag_string;
    bool nowSet = true;

    async void OnTriggerEnter()
    {
        
        CurrentNode = 0;
    }


    async public Task feel_outline()
    {



        outline = new GameObject[20];
        UpperHandle upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        outline = GameObject.FindGameObjectsWithTag(tag_string + "_child");
        //lehandle = GameObject.FindGameObjectWithTag("MeHandle");
        Vector3 handlePosition = upperHandle.GetPosition();
        print(outline.Length);


        Array.Sort(outline, CompareObNames);


        

        float dist = Vector3.Distance(outline[CurrentNode].transform.position, handlePosition);
        if (dist > threshold)
        {
            await upperHandle.SwitchTo(outline[CurrentNode]); 
        }
        else
        {
            if (CurrentNode < outline.Length - 1)
            {
                CurrentNode = CurrentNode + 1;
                
            }
            else
            {
                if(!feel_finished) 
                    OnTriggerEnter();
                feel_finished = true;
            }

        }
        upperHandle.Free();
    }

    public void SetFeelOutLine_true()
    {
        feel_finished = true;
    }
      public void SetFeelOutLine_false()
    {
        feel_finished = false;
    }

    public void SetTag(string t)
    {
        tag_string = t;
    }

    public bool GetFeel(){
        return feel_finished;
    }
    int CompareObNames(GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }

}