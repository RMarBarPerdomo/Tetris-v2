using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DualPantoFramework;
using SpeechIO;
using System.Threading.Tasks;

public class Movement : MonoBehaviour
{
    public Vector3 rotationPoint;
    private float previousTime;
    public float fallTime = 1.0f;
    public static int height = 20;
    public static int width = 10;
    private static Transform[,] grid = new Transform[width, height];
    static int pieceNumber;
    private SpeechOut speechOut;
    private bool gameStarted;
    Vector3 handlePosition;
    float zPosition;
    float lastPosition;
    public static int fallMove = 1;
    public static int leftMove = -1;
    public static int rightMove = 1;
    public static bool feel_finished;

    // Start is called before the first frame update
    async void Start()
    {
        UpperHandle upperHandle =  GameObject.Find("Panto").GetComponent<UpperHandle>();
        //await upperHandle.SwitchTo(gameObject,100f); 
        FindObjectOfType<Feel>().SetTag(tag);
        feel_finished = FindObjectOfType<Feel>().GetFeel();
       
    }



    // Update is called once per frame
    async void Update()
    {
        if (!feel_finished)
        {
            await FindObjectOfType<Feel>().feel_outline();
        }

        if(feel_finished)
        {
            
            UpperHandle upperHandle =  GameObject.Find("Panto").GetComponent<UpperHandle>();

            Vector3 handlePosition = upperHandle.GetPosition();
            float zPosition = handlePosition.z;
            float xPosition = handlePosition.x;
            float meRotation = upperHandle.GetRotation();
            float lastRotation = 0;
            float lastZPosition = 0;
            float lastXPosition = 0;

            
            
            // Movements not needed for this version of level 1, do not delete
            if(xPosition - lastXPosition < leftMove)
            {
                transform.position += new Vector3(-1, 0, 0);
                if(!ValidMove())
                    transform.position += new Vector3(1, 0, 0);
                lastXPosition = xPosition;
            }
            else if(xPosition - lastXPosition > rightMove)
            {
                transform.position += new Vector3(1, 0, 0);
                if(!ValidMove())
                    transform.position += new Vector3(-1, 0, 0);
                lastXPosition = xPosition;
            }
            //else if(Input.GetKeyDown(KeyCode.UpArrow))
            else if(meRotation - lastRotation > 90)
            {
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 1, 0), 90);
                    if(!ValidMove())
                        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, -1, 0), 90);
                lastRotation = meRotation;
            }
            else if(meRotation - lastRotation < -90)
            {
                transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 1, 0), -90);
                    if(!ValidMove())
                        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, -1, 0), -90);
                lastRotation = meRotation;
            }

            
            //if(Time.time - previousTime > (Input.GetKey(KeyCode.DownArrow) ? fallTime / 10 : fallTime))
            //if(Time.time - previousTime > ((zPosition - lastZPosition > fallmove) ? fallTime / 10 : fallTime))
            //else if(zPosition - lastZPosition > fallMove)
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.position += new Vector3 (0, 0, -1);
                FindObjectOfType<SFX>().Fall();

                if(!ValidMove())
                {
                    transform.position += new Vector3(0, 0, 1);
                    this.enabled = false;
                    AddToGrid();
                    CheckLines();
                    FindObjectOfType<Spawn>().NewPiece();
                }

                previousTime = Time.time;
                lastZPosition = zPosition;
            }
        }
        

    }

    bool HasLine(int i)
    {
        for(int j = 0; j < width; j++)
        {
            if(grid[j, i] == null)
                return false;
        }

        //clear.Play();
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
            int roundedZ = Mathf.RoundToInt(children.transform.position.z);

            grid[roundedX, roundedZ] = children;
        }
    }

    bool ValidMove()
    {
        foreach(Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedZ = Mathf.RoundToInt(children.transform.position.z);

            if(roundedX < 0 || roundedZ < 0 || roundedX >= width || roundedZ >= height)
            {
                return false;
            }

            if(grid[roundedX, roundedZ] != null)
                return false;

        }

        return true;
    }
}
