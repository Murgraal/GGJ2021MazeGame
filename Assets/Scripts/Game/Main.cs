using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;
using Photon.Pun.Demo.Cockpit;

public class Main : MonoBehaviourPunCallbacks
{

    private static Main instance;

    public static GameData data = new GameData();
    public static Player otherPlayer;
    public Text debugData;
    private static string debugDataContent;
    private static Action onDebugDataUpdated;

    [SerializeField] private List<GameObject> EnvironmentPrefabs; 





    [SerializeField] private GameObject clientPlayerPrefab;


    public override void OnEnable()
    {
        base.OnEnable();
        onDebugDataUpdated += UpdateDebugData;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        onDebugDataUpdated -= UpdateDebugData;
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this; 
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //InstantiateWorld(30);
        InitGame();
    }

    public static void AddToDebugDataContent(string toAdd)
    {
        debugDataContent += "\n" + toAdd;
        onDebugDataUpdated?.Invoke();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }
    public void InitilizeOtherPlayer()
    {
        var players = FindObjectsOfType<Player>();
        foreach (var player in players)
        {
            Debug.Log(player.gameObject.name);
            if (player != Player.LocalPlayer)
            {
                otherPlayer = player;
                otherPlayer.gameObject.name = "OtherPlayer";
            }
        }
    }

    public void UpdateDebugData()
    {
        debugData.text = debugDataContent;
    }

    public void InitGame()
    {
        Debug.Log("Initing game");
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = "1";
        }
    }

    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log("I am connected");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("RoomCreationFailed");
        AddToDebugDataContent(message);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        AddToDebugDataContent(PhotonNetwork.CloudRegion);
        Debug.Log("ConnectedToMaster");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        AddToDebugDataContent(message);
        CreateRoom();
    }

    public void CreateRoom()
    {
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("JoinedRoom");
        
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Main");
        }
        if(Player.LocalPlayer == null)
        {
            InstantiatePlayer();
        }

        AddToDebugDataContent($" Count of players: {PhotonNetwork.CountOfPlayers}");
        AddToDebugDataContent($"Players in rooms: {PhotonNetwork.CountOfPlayersInRooms}");
        AddToDebugDataContent($" Count of rooms: {PhotonNetwork.CountOfRooms}");
        AddToDebugDataContent($" My room is called: {PhotonNetwork.CurrentRoom.Name}");
        AddToDebugDataContent($" My localPlayerInstance: {Player.LocalPlayer.gameObject.name}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public void InstantiatePlayer()
    {
        var randomSpawnPosition = GetRandomPositionInWorld();
        var p1 = PhotonNetwork.Instantiate(clientPlayerPrefab.name,randomSpawnPosition,Quaternion.identity);
        
    }

    public void InstantiateWorld(int amountOfObjects)
    {
        var parent = new GameObject();
        parent.name = "World";
        for(int i = 0; i < amountOfObjects; i++)
        {
            var randomObject = EnvironmentPrefabs[UnityEngine.Random.Range(0, EnvironmentPrefabs.Count)];
            Instantiate(randomObject, GetRandomPositionInWorld(),Quaternion.identity,parent.transform);
        }
    }

    private Vector2 GetRandomPositionInWorld()
    {
        return new Vector2(UnityEngine.Random.Range(-30, 30), UnityEngine.Random.Range(-30, 30));
    }

}

public class GameData
{

}

public enum GameState
{

}
