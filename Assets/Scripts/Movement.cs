using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using SpeechIO;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    public Vector3 rotationPoint;
    private float previousTime = 0;
    public float fallTime = 5.0f;
    public static int height = 20;
    public static int width = 10;
    private static Transform[,] grid = new Transform[width, height];
    private SpeechOut speechOut;
    SpeechIn speechIn;
    Vector3 handlePosition;
    public static int fallMove = -1;
    public static int leftMove = -1;
    public static int rightMove = 1;
    public static bool feel_finished;
    float lastRotation = 0;
    float lastZPosition = -12;
    float lastXPosition = 0;
    LowerHandle lowerHandle;


    private Scene scene;
    public bool rotateFinished = true;
    public int rotateIndex;
    public int rotateTo;
    private GameObject pos;
    private int count = 0;
    bool is_level_finished = false;
    bool ready = false;
    int enough = 0;
    bool valuesSet = false;

    // Start is called before the first frame update
    async void Start()
    {

        UpperHandle upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        LowerHandle lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        speechOut = new SpeechOut();
        FindObjectOfType<Feel>().SetTag(gameObject.tag);
        feel_finished = FindObjectOfType<Feel>().GetFeel();
        speechOut.SetLanguage(SpeechBase.LANGUAGE.GERMAN);

        // if speechIn active the game stops working when finishing feel handle doesn't move afterwards
        speechIn = new SpeechIn(onSpeechRecognized);
        //speechIn.StartListening();

        rotateIndex = 0;
        ready = false;
        scene = SceneManager.GetActiveScene();
        pos = new GameObject();
        is_level_finished = false;
    }



    // Update is called once per frame
    async void Update()
    {
        // feel outline of block
        if (!feel_finished)
        {
            await FindObjectOfType<Feel>().feel_outline();
            feel_finished = FindObjectOfType<Feel>().GetFeel();
        }
        
        // Level_1
        if (scene.name == "Level_1" && !is_level_finished && feel_finished)
            await Level_1();

        // Level_2
        if (scene.name == "Level_2" && !is_level_finished && feel_finished)
            await Level_2();

        // Level_3
        if (scene.name == "Level_3" && !is_level_finished && feel_finished)
            await Level_3();

        if (scene.name == "Level_4" && !is_level_finished && feel_finished)
            await Level_4();
        

        // main movement
       
        if (feel_finished && ready)
        {
            if (!valuesSet)
                setValues();
            await movement();
        }

    }

    public void setValues()
    {
        UpperHandle upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        LowerHandle lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        Vector3 handlePosition = upperHandle.GetPosition();
        lastXPosition = handlePosition.x;
        lastZPosition = handlePosition.z;
        valuesSet = true;
    }
    async Task Level_1()
    {
        
        is_level_finished = true;
        UpperHandle upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        LowerHandle lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        Vector3 handlePosition = upperHandle.GetPosition();
        Vector3 ItPosition = new Vector3 (-1, 0, -28.5f);
        pos.transform.position = new Vector3(handlePosition.x,0, handlePosition.z);
        await upperHandle.SwitchTo(pos);
        await speechOut.Speak("Bewege den Block nach unten so");
        pos.transform.position = new Vector3(handlePosition.x, 0, -28);
        await upperHandle.SwitchTo(pos);
        pos.transform.position = new Vector3(handlePosition.x, 0, -10);
        await speechOut.Speak("um die linien zu löschen");
        await lowerHandle.MoveToPosition(ItPosition, 5.0f, false);
        await upperHandle.SwitchTo(pos);
        ready = true;
        upperHandle.Free();

    }

    async Task Level_2()
    {
        UpperHandle upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        LowerHandle lowerHandle = GameObject.Find("Panto").GetComponent<LowerHandle>();
        Vector3 handlePosition = upperHandle.GetPosition();
        Vector3 ItPosition = new Vector3 (4, 0, -28.5f);
        is_level_finished = true;
        pos.transform.position = handlePosition + Vector3.left + Vector3.left;
        await speechOut.Speak("Bewege den Block nach Links so");
        await upperHandle.SwitchTo(pos, 5);
        transform.position += new Vector3(-1, 0, 0);
        pos.transform.position += Vector3.right + Vector3.right;
        await upperHandle.SwitchTo(pos, 5);
        transform.position += new Vector3(1, 0, 0);

        await speechOut.Speak("Bewege den Block nach Rechts so.");
        pos.transform.position = handlePosition + Vector3.right;
        transform.position += new Vector3(1, 0, 0);
        await upperHandle.SwitchTo(pos, 5);
        pos.transform.position += Vector3.left + Vector3.left;
        await upperHandle.SwitchTo(pos, 5);
        transform.position += new Vector3(-1, 0, 0);
        await speechOut.Speak("Bewege den Block zur Lücke");
        await lowerHandle.MoveToPosition(ItPosition, 5.0f, false);
        ready = true;
        upperHandle.Free();
    }

    async Task Level_3()
    {
        is_level_finished = true;
        UpperHandle upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        float meRotation = upperHandle.GetRotation();
        rotateFinished = false;
        rotateTo = 1;
        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 1, 0), -90);
        await speechOut.Speak("Rotiere den Block nach Rechts so");

        rotateTo = -1;
        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 1, 0), 90);
        await speechOut.Speak("Rotiere den Block nach Links so");

        ready = true;
        upperHandle.Free();
    }

    async Task Level_4()
    {
        await speechOut.Speak("Fühle die Blocke auf dem Boden mit dem unterem Handle, nachdem du sie plaziert hast. Versuche lange zu überleben.");
        ready = true;
    }

    async Task movement()
    {
        UpperHandle upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
        Vector3 handlePosition = upperHandle.GetPosition();
        float zPosition = handlePosition.z;
        float xPosition = handlePosition.x;
        float meRotation = upperHandle.GetRotation();






        if (xPosition - lastXPosition < leftMove)
        {
            transform.position += new Vector3(-1, 0, 0);
            FindObjectOfType<SFX>().Fall();
            if (!ValidMove())
                transform.position += new Vector3(1, 0, 0);
            lastXPosition = xPosition;
        }
        else if (xPosition - lastXPosition > rightMove)
        {
            transform.position += new Vector3(1, 0, 0);
            FindObjectOfType<SFX>().Fall();
            if (!ValidMove())
                transform.position += new Vector3(-1, 0, 0);
            lastXPosition = xPosition;
        }
        else if (meRotation - lastRotation > 90)
        {

            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 1, 0), 90);
            FindObjectOfType<SFX>().Fall();
            if (!ValidMove())
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, -1, 0), 90);
            lastRotation = meRotation;
        }
        else if (meRotation - lastRotation < -90)
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 1, 0), -90);
            FindObjectOfType<SFX>().Fall();
            if (!ValidMove())
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, -1, 0), -90);
            lastRotation = meRotation;
        }

        if (Time.time - previousTime > ((zPosition - lastZPosition < fallMove) ? fallTime / 10 : fallTime))
        {
            transform.position += new Vector3(0, 0, -1);
            FindObjectOfType<SFX>().Fall();

            if (!ValidMove())
            {
                transform.position += new Vector3(0, 0, 1);
                this.enabled = false;
                AddToGrid();
                CheckLines();
                
                if(scene.name == "Level_1")
                    FindObjectOfType<SFX>().Clear();
                if(scene.name == "Level_2")
                    FindObjectOfType<SFX>().Clear();

                RemoveChildren();
                if(scene.name == "Level_1")
                    SceneManager.LoadScene("Level_2", LoadSceneMode.Single);

                if(scene.name == "Levle_2")
                    SceneManager.LoadScene("Level_3", LoadSceneMode.Single);
                
                if(scene.name == "Levle_3")
                    SceneManager.LoadScene("Level_4", LoadSceneMode.Single);

                FindObjectOfType<Spawn>().NewPiece();
            }

            lastZPosition = zPosition;
            previousTime = Time.time;
        }
    }

    void FixedUpdate()
    {
        if (!rotateFinished)
        {
            UpperHandle upperHandle = GameObject.Find("Panto").GetComponent<UpperHandle>();
            upperHandle.Rotate(rotateIndex);
            if (rotateTo == 1 && rotateIndex < 100)
                rotateIndex += 10;
            if (rotateTo == -1 && rotateIndex > 0)
                rotateIndex -= 10;
        }
        
    }

    void RemoveChildren(){
        foreach (Transform child in transform)
        {
            if(child.tag == tag + "_child")
                Destroy(child.gameObject);
        }
    }

    bool HasLine(int i)
    {
        for(int j = 0; j < width; j++)
        {
            if(grid[j, i] == null)
                return false;
        }

        FindObjectOfType<SFX>().Clear();
        return true;
    }

    void DeleteLine(int i)
    {
        for(int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);
            grid[j, i] = null;
        }
    }

    void MoveDown(int i)
    {
        for(int h = i; h < height; h++)
        {
            for(int j = 0; j < width; j++)
            {
                if(grid[j, h] != null)
                {
                    grid[j, h-1] = grid[j, h];
                    grid[j, h] = null;
                    grid[j, h-1].transform.position = new Vector3(0, 0, 1);
                }
            }
        }
    }

    void CheckLines()
    {
        for(int i = height-1; i >= 0; i--) 
        {
            if(HasLine(i))
            {
                DeleteLine(i);
                MoveDown(i);
            }
        }
    }

    void AddToGrid()
    {
        foreach(Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedZ = Mathf.RoundToInt(children.transform.position.z - 0.5f);

            grid[roundedX + 5, roundedZ + 29] = children;
        }
    }

    async void onSpeechRecognized(string command)
    {
        if (command == "Feel")
        {
            FindObjectOfType<Feel>().GetFeel();
            FindObjectOfType<Feel>().feel_outline();
            FindObjectOfType<Feel>().GetFeel();
        }
    }

    bool ValidMove()
    {
        foreach(Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedZ = Mathf.RoundToInt(children.transform.position.z);

            if(roundedX < -5 || roundedZ < -29 || roundedX >= 5 || roundedZ >= -8)
            {
                return false;
            }

            if(grid[roundedX + 5, roundedZ + 29] != null)
                return false;

        }

        return true;
    }
}
