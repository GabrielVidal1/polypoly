using MatchMaking;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public static Player localPlayer;
    [SyncVar] public string matchID;
    [SyncVar] public int playerIndex;

    NetworkMatchChecker networkMatchChecker;

    [SyncVar] public Match currentMatch;

    [SerializeField] GameObject playerLobbyUI;

    void Awake () {
        networkMatchChecker = GetComponent<NetworkMatchChecker> ();
    }

    public override void OnStartClient () {
        if (isLocalPlayer) {
            localPlayer = this;
        } else {
            Debug.Log ($"Spawning other player UI Prefab");
            playerLobbyUI = UILobby.instance.SpawnPlayerUIPrefab (this);
        }
    }

    public override void OnStopClient () {
        Debug.Log ($"Client Stopped");
        ClientDisconnect ();
    }
    
    
    public override void OnStopServer () {
        Debug.Log ($"Client Stopped on Server");
        ServerDisconnect ();
    }

    /* 
        HOST MATCH
    */

    public void HostGame (bool publicMatch) {
        string matchID = MatchMaker.GetRandomMatchID ();
        CmdHostGame (matchID, publicMatch);
    }

    [Command]
    void CmdHostGame (string _matchID, bool publicMatch) {
        matchID = _matchID;
        if (MatchMaker.instance.HostGame (_matchID, gameObject, publicMatch, out playerIndex)) {
            Debug.Log ($"<color=green>Game hosted successfully</color>");
            networkMatchChecker.matchId = _matchID.ToGuid ();
            TargetHostGame (true, _matchID, playerIndex);
        } else {
            Debug.Log ($"<color=red>Game hosted failed</color>");
            TargetHostGame (false, _matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetHostGame (bool success, string _matchID, int _playerIndex) {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log ($"MatchID: {matchID} == {_matchID}");
        UILobby.instance.HostSuccess (success, _matchID);
    }

    /* 
        JOIN MATCH
    */

    public void JoinGame (string _inputID) {
        CmdJoinGame (_inputID);
    }

    [Command]
    void CmdJoinGame (string _matchID) {
        matchID = _matchID;
        if (MatchMaker.instance.JoinGame (_matchID, gameObject, out playerIndex)) {
            Debug.Log ($"<color=green>Game Joined successfully</color>");
            networkMatchChecker.matchId = _matchID.ToGuid ();
            TargetJoinGame (true, _matchID, playerIndex);
        } else {
            Debug.Log ($"<color=red>Game Joined failed</color>");
            TargetJoinGame (false, _matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetJoinGame (bool success, string _matchID, int _playerIndex) {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log ($"MatchID: {matchID} == {_matchID}");
        UILobby.instance.JoinSuccess (success, _matchID);
    }

    /* 
        DISCONNECT
    */

    public void DisconnectGame () {
        CmdDisconnectGame ();
    }

    [Command]
    void CmdDisconnectGame () {
        ServerDisconnect ();
    }

    void ServerDisconnect () {
        MatchMaker.instance.PlayerDisconnected (this, matchID);
        RpcDisconnectGame ();
        networkMatchChecker.matchId = string.Empty.ToGuid ();
    }

    [ClientRpc]
    void RpcDisconnectGame () {
        ClientDisconnect ();
    }

    void ClientDisconnect () {
        if (playerLobbyUI != null) {
            Destroy (playerLobbyUI);
        }
    }

    /* 
        SEARCH MATCH
    */

    public void SearchGame () {
        CmdSearchGame ();
    }

    [Command]
    void CmdSearchGame () {
        if (MatchMaker.instance.SearchGame (gameObject, out playerIndex, out matchID)) {
            Debug.Log ($"<color=green>Game Found Successfully</color>");
            networkMatchChecker.matchId = matchID.ToGuid ();
            TargetSearchGame (true, matchID, playerIndex);
        } else {
            Debug.Log ($"<color=red>Game Search Failed</color>");
            TargetSearchGame (false, matchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetSearchGame (bool success, string _matchID, int _playerIndex) {
        playerIndex = _playerIndex;
        matchID = _matchID;
        Debug.Log ($"MatchID: {matchID} == {_matchID} | {success}");
        UILobby.instance.SearchGameSuccess (success, _matchID);
    }

    /* 
        BEGIN MATCH
    */

    public void BeginGame () {
        CmdBeginGame ();
    }

    [Command]
    void CmdBeginGame () {
        MatchMaker.instance.BeginGame (matchID);
        Debug.Log ($"<color=red>Game Beginning</color>");
    }

    public void StartGame () { //Server
        TargetBeginGame ();
    }

    [TargetRpc]
    void TargetBeginGame () {
        Debug.Log ($"MatchID: {matchID} | Beginning");
        //Additively load game scene
        //SceneManager.LoadScene (2, LoadSceneMode.Additive);
    }

    
    
    
    
    
    
    
    
    
    
    
    

    [Header("Other")]
    
    [SerializeField] private float speed;
    [SerializeField] private float steeringSpeed;
    
    private Circuit circuit;

    private Vector3 targetDirection;
    
    private Rigidbody rb;

    public Section CurrentSection = null;


    [SyncVar]
    public int money = 1000;

    private bool stopped;
    
    [SerializeField] private float fuel = 3f;
    
    private void Start()
    {
        GameManager.instance.AddPlayer(this);
        if (isLocalPlayer)
            GameManager.instance.localPlayer = this;

        rb = GetComponent<Rigidbody>();
        
        if (!isLocalPlayer)    
        {
            Destroy(rb);
            tag = "Undefined";
        }
    }

    [Server]
    public void Go(Circuit circuit)
    {
        this.circuit = circuit;
        CurrentSection = circuit.sections[0];
        stopped = false;
    }

    
    private void FixedUpdate()
    {
        if (isLocalPlayer)
            Move(Input.GetKey(KeyCode.LeftArrow), Input.GetKey(KeyCode.RightArrow));
    }
    
    
    [Command]
    private void Move(bool left, bool right)
    {
        Vector3 acceleration = transform.forward * speed;

        float torque = 0f;
        if (left)
        {
            torque = -steeringSpeed;
            acceleration += -transform.right * steeringSpeed;
        }
        if (right)
        {
            torque = steeringSpeed;
            acceleration += transform.right * steeringSpeed;
        }
        rb.AddTorque(transform.up * torque);

        if (fuel >= 0f)
        {
            rb.AddForce(acceleration);
            fuel -= Time.fixedDeltaTime;
        }
        else
        {
            if (!stopped)
                Stop();
        }
    }

    [Command]
    public void Refuel()
    {
        fuel = Random.value * 10f;
        stopped = false;
    }

    [ClientRpc]
    void Stop()
    {
        stopped = true;
        GameManager.instance.mainCanvas.StopScreen.Open();
    }
}
