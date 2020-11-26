using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    #endregion
    
    const int MINIMUM_PLAYER_NUMBER = 1;
    public const float ROAD_WIDTH = 10f;
    
    public List<Player> players;
    public Player localPlayer;

    public Circuit Circuit;
    public MainCanvas mainCanvas;
    public NetworkManager NetworkManager;

    public Property[] properties;
    
    private void Start()
    {
        Circuit.CreateCircuit();
    }
    
    public void AddPlayer(Player player)
    {
        players.Add(player);
        if (players.Count >= MINIMUM_PLAYER_NUMBER && isLocalPlayer)
            StartGame();
    }
    
    [Command]
    void StartGame()
    {
        foreach (Player player in players)
        {
            player.Go(Circuit);
        }
    }
}
