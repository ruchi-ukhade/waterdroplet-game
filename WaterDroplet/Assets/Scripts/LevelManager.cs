using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    // List of resettable objects
    private List<IResettable> resettableObjects = new List<IResettable>();


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
        RegisterAllResettableObjects();
    }


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

}
