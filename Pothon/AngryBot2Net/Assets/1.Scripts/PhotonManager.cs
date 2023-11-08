using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0";
    private string userID = "KimBH";

    public TMP_InputField userIF;
    public TMP_InputField roomNameIF;

    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    private GameObject roomItemPrefab;
    public Transform scrollContent;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;
        //PhotonNetwork.NickName = userID;

        roomItemPrefab = Resources.Load<GameObject>("RoomItem");
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Start()
    {
        userID = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(1, 21):00}");
        userIF.text = userID;

        PhotonNetwork.NickName = userID;
    }

    public void SetUserID()
    {
        if (string.IsNullOrEmpty(userIF.text))
        {
            userID = $"USER_{Random.Range(1, 21):00}";
        }
        else
        {
            userID = userIF.text;
        }

        PlayerPrefs.SetString("USER_ID", userID);
        PhotonNetwork.NickName = userID;
    }

    string SetRoomName()
    {
        if (string.IsNullOrEmpty(roomNameIF.text))
        {
            roomNameIF.text = $"ROOM_{Random.Range(1, 101):000}";
        }

        return roomNameIF.text;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master!");
        Debug.Log($"PhotonNetwork.InLobby : {PhotonNetwork.InLobby}");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        //PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"JoinRandom Failed {returnCode} : {message}");

        OnMakeRoomClick();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            Debug.Log($"{player.NickName}, {player.ActorNumber}");
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleField");
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList)
            {
                if (rooms.TryGetValue(roomInfo.Name, out GameObject tempRoom))
                {
                    Destroy(tempRoom);
                    rooms.Remove(roomInfo.Name);
                }
            }
            else
            {
                if (!rooms.ContainsKey(roomInfo.Name))
                {
                    GameObject roomPrefab = Instantiate(roomItemPrefab, scrollContent);
                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else
                {
                    rooms.TryGetValue(roomInfo.Name, out GameObject tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = roomInfo;
                }
            }
            Debug.Log($"Room = {roomInfo.Name} ({roomInfo.PlayerCount} / {roomInfo.MaxPlayers})");
        }
    }

    #region UI_BUTTON_EVENT
    public void OnLoginClick()
    {
        SetUserID();
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnMakeRoomClick()
    {
        SetUserID();

        RoomOptions room = new()
        {
            MaxPlayers = 20,
            IsOpen = true,
            IsVisible = true
        };

        PhotonNetwork.CreateRoom(SetRoomName(), room);
    }
    #endregion
}
