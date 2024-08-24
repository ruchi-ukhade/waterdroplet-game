using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    // List of resettable objects
    private List<IResettable> resettableObjects = new List<IResettable>();

    private Vector2 lastCheckpoint;
    private int playerSize; // record player size when it reaches a checkpoint

    public float startTimelineTime = 5f;
    public GameObject dialogueController;
    private ForestSpiritDialog dialogueControllerScript;

    // player stats
    private int retries = 0;
    private int jumps = 0;
    private int sizeChanges = 0;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(showDialogueAfterTimeline());

        dialogueControllerScript = dialogueController.GetComponent<ForestSpiritDialog>();
        dialogueControllerScript.StartDialogue();
        RegisterAllResettableObjects();
    }


    #region Reset Level
    // loop thru all objs with the type of IResettable, and register each one
    private void RegisterAllResettableObjects()
    {
        // Get an array of objects with the type of IResettable
        IResettable[] objects = FindObjectsOfType<MonoBehaviour>().OfType<IResettable>().ToArray();
        // Resgister each object to be a resettable object
        foreach (IResettable obj in objects)
        {
            RegisterResettableObject(obj);
        }
    }

    // Append obj to the resettableObjects list, and Save its initial state
    public void RegisterResettableObject(IResettable obj)
    {
        if (!resettableObjects.Contains(obj))
        {
            resettableObjects.Add(obj); // Append this obj to the resettableObjects list
            obj.SaveInitialState(); // Save obj's initial state
        }
    }

    // Remove obj from the resettableObjects list
    public void UnregisterResettableObject(IResettable obj)
    {
        resettableObjects.Remove(obj);
    }

    // Loop thru all objs in resettableObjects list and reset each
    public void ResetLevel()
    {
        foreach (IResettable obj in resettableObjects)
        {
            obj.ResetToInitialState();
        }
    }
    #endregion


    #region Respawn player
    // Setter and Getter for check point position
    public void SetCheckpoint(Vector2 position)
    {
        lastCheckpoint = position;
    }

    public Vector2 GetLastCheckpoint()
    {
        return lastCheckpoint;
    }


    // Setter and Getter for player size when hit check point 
    public void SetPlayerSize(int size)
    {
        playerSize = size;
    }
    public int GetPlayerSize()
    {
        return playerSize;
    }
    #endregion


    // After "start timeline" played, enable the dialogue box
    /*private IEnumerator showDialogueAfterTimeline()
    {
        Debug.Log("start");
        yield return new WaitForSecondsRealtime(startTimelineTime);        
        Debug.Log("text");
        //DialogueController.SetActive(true);

        
    }*/


    public int getRetries() { return retries; }
    public void incrementRetries() { retries++; }

    public int getJumps() { return jumps;}
    public void incrementJumps() { jumps++; }

    public int getSizeChanges() { return sizeChanges;}
    public void incrementSizeChanges() { sizeChanges++; }
}
