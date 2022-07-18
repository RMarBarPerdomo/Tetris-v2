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
    PantoCollider[] pantoColliders;

    // Start is called before the first frame update
    async void Start()
    {   
        /*pantoColliders = GameObject.FindObjectsOfType<PantoCollider>();
        foreach (PantoCollider collider in pantoColliders)
        {
            collider.CreateObstacle();
            collider.Enable();
        }*/

        speechOut = new SpeechOut();
        LowerHandle lowerHandle;
        //await GameObject.Find("Panto").GetComponent<LowerHandle>().SwitchTo(itPosition);
        //await speechOut.Speak("Move the upper handle towards yourself to fill the hole and clear the line.");
        NewPiece();
    }

    // Update is called once per frame
    public void NewPiece()
    {   
            FindObjectOfType<Feel>().SetFeelOutLine_false();
            Instantiate(Pieces[0], transform.position, Quaternion.identity);
            //Random.Range(0, Pieces.Length)
    }

}