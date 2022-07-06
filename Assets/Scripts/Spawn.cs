using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using SpeechIO;
using System.Threading.Tasks;

public class Spawn : MonoBehaviour
{
    public GameObject[] Pieces;
    private SpeechOut speechOut;
    public GameObject itPosition;

    // Start is called before the first frame update
    async void Start()
    {   
        speechOut = new SpeechOut();
        LowerHandle lowerHandle;
        NewPiece(0);
        await GameObject.Find("Panto").GetComponent<LowerHandle>().SwitchTo(itPosition);
        speechOut.Speak("Move the handle towards yourself to fill the hole and clear the line.");
    }

    // Update is called once per frame
    public void NewPiece(int i)
    {   
            Instantiate(Pieces[i], transform.position, Quaternion.identity);
    }

}