using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using SpeechIO;
using System.Threading.Tasks;

public class Feel : MonoBehaviour
{
    private SpeechOut speechOut;
    int CurrentNode = 0;
    public static GameObject[] outline;
    public static GameObject L_Shape;
    public static GameObject lehandle;
    float threshold = 0.1f;
    static GameObject CurrentPositionHolder;
    static bool feel_finished = false;
    public static int fallmove = 25;
    public static UpperHandle upperHandle;
    string tag_string;

    //public AudioSource clear;
    //public AudioSource fall;

    // Start is called before the first frame update
    async void Start()
    {
        outline = GameObject.FindGameObjectsWithTag(tag_string + "_child");
        lehandle = GameObject.FindGameObjectWithTag("MeHandle");
        await upperHandle.SwitchTo(outline[0]);
    }


    // Update is called once per frame

    async void OnTriggerEnter()
    {
        await speechOut.Speak("now feel how the block is moving");
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(4);
    }

    static async public Task feel_outline()
    {

        float dist = Vector3.Distance(outline[Feel.CurrentNode].transform.position, lehandle.transform.position);

        if (dist > threshold)
        {
            await upperHandle.SwitchTo(outline[Feel.CurrentNode]); //= Vector3.Lerp(startPosition, CurrentPositionHolder, Timer);
        }
        else
        {
            if (CurrentNode < outline.Length - 1)
            {
                CurrentNode++;
            }
            else
            {
                feel_finished = true;
                OnTriggerEnter();
            }

        }


    }

    async static public void SetFeelOutLine_true()
    {
        feel_finished = true;
    }
    async static public void SetFeelOutLine_false()
    {
        feel_finished = false;
    }

    static public void SetTag(string t)
    {
        tag_string = t;
    }

    static public bool  GetFeel(){
        return feel_finished;
    }

}

