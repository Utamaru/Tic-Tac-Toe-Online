using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameplayController : Photon.MonoBehaviour
{
    #region Singleton
    public static GameplayController get
    {
        get
        {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<GameplayController>();
            return _instance;
        }
    }
    private static GameplayController _instance;
    #endregion

    public bool AutoConnect = true;
    public byte Version = 1;
    public Text StatusLabel;

    public GameState State
    {
        get
        {
            return _state;
        }
        set
        {
            if (_state != value)
            {
                _state = value;
                string message;
                if (_state == GameState.WaitingForPlayers)
                {
                    message = "Waiting for another player to join your game";
                }
                else if (_state == GameState.WaitingForRestart)
                {
                    message = "Waiting for your opponent's response";
                }
                else if (_state == GameState.YouWon)
                {
                    message = "You won\nPress \"spacebar\" to try to win again";
                }
                else if (_state == GameState.YouLost)
                {
                    message = "You lost\nPress \"spacebar\" to take revenge";
                }
                else if (_state == GameState.Tie)
                {
                    message = "It's a tie\nPress \"spacebar\" to rematch";
                }
                else if (_state == GameState.WaitingForTurnByX && PhotonNetwork.player.GetTeam() == PunTeams.Team.X ||
                    _state == GameState.WaitingForTurnByO && PhotonNetwork.player.GetTeam() == PunTeams.Team.O)
                {
                    message = "It is your turn";
                }
                else
                {
                    message = "It is your opponent's turn";
                }
                StatusLabel.text = message;
            }
        }
    }

    private GameState _state;
    private PhotonView photonView;
    private bool connectInUpdate = true;

    private bool isPlayerOneReady;
    private bool isPlayerTwoReady;

    private void Awake()
    {
        _state = GameState.None;
        State = GameState.WaitingForPlayers;
        photonView = GetComponent<PhotonView>();
    }

    public virtual void Start()
    {
        PhotonNetwork.autoJoinLobby = false;
    }

    public virtual void Update()
    {
        if (connectInUpdate && AutoConnect && !PhotonNetwork.connected)
        {
            connectInUpdate = false;
            PhotonNetwork.ConnectUsingSettings(Version + "." + Application.loadedLevel);
        }

        if (State == GameState.YouLost || State == GameState.YouWon || State == GameState.Tie)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                if (!PhotonNetwork.player.isMasterClient)
                {
                    photonView.RPC("InformMasterClientToRestart", PhotonTargets.Others);
                }
                else
                {
                    isPlayerOneReady = true;
                }

                State = GameState.WaitingForRestart;
            }

        }

        if (State == GameState.WaitingForRestart && isPlayerOneReady && isPlayerTwoReady && PhotonNetwork.player.isMasterClient)
        {
            InitializeGame();
            photonView.RPC("StartGame", PhotonTargets.AllViaServer);
        }
    }

    #region PhotonEvents

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    public virtual void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public virtual void OnPhotonRandomJoinFailed()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = 2 }, null);
    }

    public virtual void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.LogError("Cause: " + cause);
    }

    public void OnJoinedRoom()
    {
        if (PhotonNetwork.playerList.Length == 2)
        {
            if (PhotonNetwork.offlineMode)
            {

            }
            else
            {
                photonView.RPC("StartGame", PhotonTargets.AllViaServer);
            }
        }
    }

    public virtual void OnPhotonPlayerDisconnected()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        Grid.get.Clear();
        State = GameState.WaitingForPlayers;
        isPlayerOneReady = isPlayerTwoReady = false;
    }

    #endregion

    [RPC]
    public void StartGame()
    {
        InitializeGame();
        PhotonNetwork.player.SetTeam(PhotonNetwork.player.isMasterClient ? PunTeams.Team.X : PunTeams.Team.O);
        State = GameState.WaitingForTurnByX;
    }

    public void OnClickTile(PunTeams.Team team, IntVector2 coordinates)
    {
        photonView.RPC("PerformTurn", PhotonTargets.AllViaServer, new object[] { team.ToString(), coordinates.x, coordinates.y });
    }

    [RPC]
    public void PerformTurn(string teamName, int x, int y)
    {
        if (State == GameState.YouLost | State == GameState.YouWon | State == GameState.Tie)
        {
            return;
        }

        IntVector2 coordinates = new IntVector2(x, y);
        PunTeams.Team team = Helpers.ParseEnum<PunTeams.Team>(teamName);

        Debug.Log(State + ", " + team + " : " + coordinates.ToString());

        if (team == PunTeams.Team.none | State == GameState.WaitingForRestart) return;

        if (team == PunTeams.Team.X && State == GameState.WaitingForTurnByX ||
            team == PunTeams.Team.O && State == GameState.WaitingForTurnByO)
        {
            if (Grid.get.PerformTurn(coordinates, team))
            {
                if (State == GameState.WaitingForTurnByX) State = GameState.WaitingForTurnByO;
                else if (State == GameState.WaitingForTurnByO) State = GameState.WaitingForTurnByX;
            }
        }
    }

    [RPC]
    public void RestartWithCurrentPlayer()
    {

    }

    private void OnGameOver(PunTeams.Team team)
    {
        photonView.RPC("OnGameOverRPC", PhotonTargets.AllViaServer, new object[] { team.ToString() });
    }

    [RPC]
    private void OnGameOverRPC(string teamName)
    {
        PunTeams.Team team = Helpers.ParseEnum<PunTeams.Team>(teamName);

        Debug.Log("winner: " + team.ToString());

        if (team == PunTeams.Team.none)
        {
            State = GameState.Tie;
        }
        else if (team == PhotonNetwork.player.GetTeam())
        {
            State = GameState.YouWon;
        }
        else
        {
            State = GameState.YouLost;
        }
    }

    [RPC]
    private void InformMasterClientToRestart()
    {
        isPlayerTwoReady = true;
    }

    public void ReadyToRestart()
    {

    }

    private void OnGUI()
    {
#if UNITY_EDITOR
        GUI.contentColor = Color.black;
        GUI.Label(new Rect(10, 10, 500, 500),
            string.Format("Network state: {0}\nGame state: {1}\nIs Master Client: {2}\nTeam: {3}",
            PhotonNetwork.connectionState, State,
            PhotonNetwork.player.
            isMasterClient,
            PhotonNetwork.player.GetTeam()));
#endif
    }

    private void OnEnable()
    {
        Grid.get.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        Grid.get.OnGameOver -= OnGameOver;
    }
}
