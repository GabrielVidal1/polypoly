using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    const int MINIMUM_PLAYER_NUMBER = 1;
    
    public List<Player> players;
    public Player LocalPlayer { get; private set; }

    public void AddPlayer(Player player)
    {
        players.Add(player);
        LocalPlayer = player;
        if (players.Count >= MINIMUM_PLAYER_NUMBER)
            StartGame();
    }

    void StartGame()
    {
        
    }
}
