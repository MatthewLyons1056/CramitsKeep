using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    public string objectID;
    public GameObject currentGameObject;

    private void Awake()
    {
        objectID = name + transform.position.ToString() + transform.eulerAngles.ToString();
    }

    private void Start()
    {
        currentGameObject = gameObject;

        for (int i = 0; i < Object.FindObjectsOfType<DontDestroyOnLoad>().Length; i++)
        {
            if (Object.FindObjectsOfType<DontDestroyOnLoad>()[i] != this)
            {
                if (Object.FindObjectsOfType<DontDestroyOnLoad>()[i].objectID == objectID)
                {
                    Destroy(gameObject);
                }
            }
        }
        DontDestroyOnLoad(gameObject);
        PopulatDontDestroyList();
    }
    public void PopulatDontDestroyList()
    {
        GameObject gameControllerOBJ = GameObject.FindGameObjectWithTag("MainSceneController");
        MainSceneManager pauseOBJ = gameControllerOBJ.GetComponent<MainSceneManager>();
        pauseOBJ.dontDestroyOBJs.Add(gameObject);
    }
}
