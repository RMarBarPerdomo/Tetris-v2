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
    public float fallTime = 0.8f;
    public static int height = 20;
    public static int width = 10;
    private static Transform[,] grid = new Transform[width, height];
    static int pieceNumber;
    private SpeechOut speechOut;
    private bool gameStarted;
    Vector3 handlePosition;
    float zPosition;
    float lastPosition;
    public int fallmove = 25;
    //public int leftMove = -25;
    //public int rightMove = 25;
    
    //public AudioSource clear;
    //public AudioSource fall;

    // Start is called before the first frame update
    async void Start()
    {
       
    }



    // Update is called once per frame
    async void Update()
    {
        UpperHandle upperHandle =  GameObject.Find("Panto").GetComponent<UpperHandle>();
        await upperHandle.SwitchTo(gameObject,100f); 

        Vector3 handlePosition = upperHandle.GetPosition();
        float zPosition = handlePosition.z;
        float xPosition = handlePosition.x;
        float lastZPosition = 0;
        float lastXPosition = 0;
        
        
        // Movements not needed for this version of level 1, do not delete
        /* if(xPosition - lastXPosition < leftMove)
        {
            transform.position += new Vector3(-1, 0, 0);
            if(!ValidMove())
                transform.position += new Vector3(1, 0, 0);
            lastXPosition = handlePosition;
        }
        else if(xPosition - lastXPosition > rightMove)
        {
            transform.position += new Vector3(1, 0, 0);
            if(!ValidMove())
                transform.position += new Vector3(-1, 0, 0);
            lastXPosition = handlePosition;
        }
        else if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 1, 0), 90);
                if(!ValidMove())
                    transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, -1, 0), 90);
        }*/

        if(Time.time - previousTime > ((zPosition - lastPosition > fallmove) ? fallTime / 10 : fallTime))
        {
            transform.position += new Vector3 (0, 0, -1);
            //fall.Play();
            FindObjectOfType<SFX>().Fall();

            if(!ValidMove())
            {
                transform.position += new Vector3(0, 0, 1);
                this.enabled = false;
                AddToGrid();
                CheckLines();
                //FindObjectOfType<Spawn>().NewPiece(pieceNumber);
            }

            previousTime = Time.time;
            lastPosition = handlePosition.z;
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
