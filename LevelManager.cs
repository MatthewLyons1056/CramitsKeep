using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    /*The goal of the level manager is to spawn a series of rooms based off of the level
     * The rooms need to be spread out throughout the level, and also level stream only relevent levels
     */
    [SerializeField]
    //Reference Player
    public PlayerControls pControlsScript;
    public ItemManager itemManagerScript;
    public LoadProgress lProgressScript;
    public MainSceneManager mSceneScript;
    private GameStateController gameStateScript;
    // private LevelManager lManagerScript;
    //collection of lists for room spawning and managing
    public List<GameObject> roomPrefabs;
    public GameObject bossRoom;
    public List<GameObject> ActiveRoomPrefabs = new List<GameObject>();//this list is a collection of roomMechanic scripts in spawn order
    public List<GameObject> roomSpawnPos;
    public List<GameObject> entranceList = new List<GameObject>();
    public List<GameObject> fullRoomOBJs = new List<GameObject>();
    //spawn cost influence value that scales upward based on each rooms difficulty
    public int groupCostIncreaseVal, roomCostIncreaseVal;
    public float enemyHealthIncrease, enemyDamageIncrease;
    private float bossBonusMultiplier;//special mulitplier that will greatly boost scaling based on boss kill count

    //data for when a power upgrade should spawn instead of a stat booster
    public int roomsUntilPowerItem, roomsUntilAbilityItem;
    private int roomsCleared;
    [SerializeField]
    public int spawnIndex = 0;

    //Entrance Transition Logic
    public int currentRoomIndex;

    //Finished with spawning rooms
    

    void Start()
    {
        //Reference Player
        GameObject player = GameObject.FindWithTag("Player");

        if(player != null)
        {
            pControlsScript = player.GetComponent<PlayerControls>();
        }
        
        //Reference ItemManager
        GameObject itemManager = GameObject.Find("ItemManager");
        if (itemManager != null)
        {
            itemManagerScript = itemManager.GetComponent<ItemManager>();
        }
        

        GameObject loadManager = GameObject.Find("LoadManager");
        if (loadManager != null)
        {
            lProgressScript = loadManager.GetComponent<LoadProgress>();
        }
        

        GameObject mainSceneManager = GameObject.FindGameObjectWithTag("MainSceneController");
        if (mainSceneManager != null)
        {
            mSceneScript = mainSceneManager.GetComponent<MainSceneManager>();
            gameStateScript = mainSceneManager.GetComponentInChildren<GameStateController>();
            bossBonusMultiplier = gameStateScript.bossKillCounter * .2f + 1;
            enemyHealthIncrease*= bossBonusMultiplier;
            enemyDamageIncrease*= bossBonusMultiplier * .75f;
            groupCostIncreaseVal *= Mathf.RoundToInt(bossBonusMultiplier * 2 - 1);
            roomCostIncreaseVal *= Mathf.RoundToInt(bossBonusMultiplier * 2 - 1);
        }
      
        //search for all available room locations
        for (int i = 0; i < roomSpawnPos.Count - 1; i++) //spawn each room - the first one
        {
            int randomRoomIndex = Random.Range(0, roomPrefabs.Count);//Index for random rooms
            Instantiate(roomPrefabs[randomRoomIndex], roomSpawnPos[spawnIndex].transform.position, transform.rotation);
            roomPrefabs.RemoveAt(randomRoomIndex);
            spawnIndex += 1;
        }
        Instantiate(bossRoom, roomSpawnPos[spawnIndex].transform.position, transform.rotation);
        GameObject[] roomVisuals = GameObject.FindGameObjectsWithTag("RoomVisualOBJ");
        foreach(GameObject room in roomVisuals)
        {
            fullRoomOBJs.Add(room);
        }
        lProgressScript.StartCoroutine(lProgressScript.SetLoadSequence());
        
    }
    

    public IEnumerator SearchRooms()
    {
        GameObject[] rooms;
        rooms = GameObject.FindGameObjectsWithTag("Room");
        foreach(GameObject room in rooms)
        {
            ActiveRoomPrefabs.Add(room);
        }
        yield return new WaitForSeconds(.5f);
        lProgressScript.entranceCheck = true;
        foreach (GameObject room in ActiveRoomPrefabs)
        {
            //add roomMechanic, then reference the entrance   
            RoomMechanic roomMechanicOBJ = room.GetComponent<RoomMechanic>();
            entranceList.Add(roomMechanicOBJ.entrance);
            //entranceList.Add(room.GetComponent<RoomMechanic>().entrance.gameObject);
        }
        DeactivateRooms(0);
    }
    public void TransitionPlayer(GameObject nextUpgrade)
    {
        //increase counter on rooms cleared and the spawn modifier values
        roomsCleared += 1;
        gameStateScript.roomsCleared += 1;
        gameStateScript.groupCostModifier += groupCostIncreaseVal;
        gameStateScript.roomCostModifier += roomCostIncreaseVal;
        gameStateScript.enemyHealthBonus += enemyHealthIncrease;
        gameStateScript.enemyDamageBonus += enemyDamageIncrease;
        itemManagerScript.DesignateUpgrade(nextUpgrade, currentRoomIndex);
        Destroy(ActiveRoomPrefabs[currentRoomIndex - 1].GetComponent<RoomMechanic>().lootPointerInScene);
        //if true this will cause the next room to choose a power upgrade instead of stat boosters
        if (roomsCleared == roomsUntilPowerItem)
        {
            roomsUntilPowerItem += 5;
            ActiveRoomPrefabs[currentRoomIndex].GetComponent<RoomMechanic>().upgradeType = "Power";
        }
        //if true this will cause the next room to choose a ability upgrade instead of a stat booster or power
        if (roomsCleared == roomsUntilAbilityItem)
        {
            roomsUntilAbilityItem += 5;
            ActiveRoomPrefabs[currentRoomIndex].GetComponent<RoomMechanic>().upgradeType = "Ability";
        }
        //moves the player to the next room and designates what upgrade the next room will provide
        pControlsScript.transform.position = entranceList[currentRoomIndex].transform.position;

        GameObject nextRoom = ActiveRoomPrefabs[currentRoomIndex];
        nextRoom.GetComponent<RoomMechanic>().maxGroupCost += gameStateScript.groupCostModifier;
        nextRoom.GetComponent<RoomMechanic>().maxRoomCost += gameStateScript.roomCostModifier;
        //sets the TP position to the next room
        DeactivateRooms(currentRoomIndex);
        currentRoomIndex += 1;
    }
    public void DeactivateRooms(int currentRoomNum)
    {
        fullRoomOBJs[currentRoomNum].SetActive(true);
        for(int i = 0; i < fullRoomOBJs.Count; i++)
        {
            if(i != currentRoomNum)
            {
                fullRoomOBJs[i].SetActive(false);
            }
        }
    }
    public void MovePlayer(int targetRoom)
    {
        pControlsScript.transform.position = entranceList[targetRoom].transform.position;
    }
    public void ScaleRoom(int targetRoom)
    {
        GameObject nextRoom = ActiveRoomPrefabs[targetRoom];
        nextRoom.GetComponent<RoomMechanic>().maxGroupCost += gameStateScript.groupCostModifier;
        nextRoom.GetComponent<RoomMechanic>().maxRoomCost += gameStateScript.roomCostModifier;
    }

   
}

