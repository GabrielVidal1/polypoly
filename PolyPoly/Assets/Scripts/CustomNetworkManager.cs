using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        Vector3 startPos = GameManager.instance.Circuit.sections[0].CurrentPoint;
        GameObject player = Instantiate(playerPrefab, startPos,
            Quaternion.LookRotation(GameManager.instance.Circuit.sections[0].Direction));
        NetworkServer.AddPlayerForConnection(conn, player);
    }
    
    
}
