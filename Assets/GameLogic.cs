using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameLogic : MonoBehaviour
{
    
    const float CharacterSpeed = 0.25f;
    float DiagonalCharacterSpeed;

    Dictionary<int, Character> players = new Dictionary<int, Character>();
    void Start()
    {
        DiagonalCharacterSpeed = Mathf.Sqrt(CharacterSpeed * CharacterSpeed + CharacterSpeed * CharacterSpeed) / 2f;
        NetworkServerProcessing.SetGameLogic(this);
    }

    private void Update()
    {
        List<int> playerIds = new List<int>(players.Keys);
        foreach (int id in playerIds)
        {
            Character character = players[id];
            character.position += character.velocity * Time.deltaTime;
            players[id] = character; // Reassign the updated struct back to the dictionary
        }
    }
    public void HandlePlayerInput(int clientId, int input)
    {
        if (!players.ContainsKey(clientId)) return;

        Character character = players[clientId];
        character.velocity = CalculateVelocity(input);
        players[clientId] = character;

        NotifyPlayersOfVelocityChange(clientId);

    }

    private Vector2 CalculateVelocity(int input)
    {
        Vector2 characterVelocityInPercent;
        // Your existing logic to calculate velocity...
        characterVelocityInPercent = Vector2.zero;

        if (input == InputSendToSever.wdKey)
        {
            characterVelocityInPercent.x = DiagonalCharacterSpeed;
            characterVelocityInPercent.y = DiagonalCharacterSpeed;
        }
        else if (input == InputSendToSever.waKey)
        {
            characterVelocityInPercent.x = -DiagonalCharacterSpeed;
            characterVelocityInPercent.y = DiagonalCharacterSpeed;
        }
        else if (input == InputSendToSever.sdKey)
        {
            characterVelocityInPercent.x = DiagonalCharacterSpeed;
            characterVelocityInPercent.y = -DiagonalCharacterSpeed;
        }
        else if (input == InputSendToSever.saKey)
        {
            characterVelocityInPercent.x = -DiagonalCharacterSpeed;
            characterVelocityInPercent.y = -DiagonalCharacterSpeed;
        }
        else if (input == InputSendToSever.dKey)
            characterVelocityInPercent.x = CharacterSpeed;
        else if (input == InputSendToSever.aKey)
            characterVelocityInPercent.x = -CharacterSpeed;
        else if (input == InputSendToSever.wKey)
            characterVelocityInPercent.y = CharacterSpeed;
        else if (input == InputSendToSever.sKey)
            characterVelocityInPercent.y = -CharacterSpeed;
        return characterVelocityInPercent;
    }

    private void NotifyPlayersOfVelocityChange(int clientId)
    {
        Vector2 velocity = players[clientId].velocity;
        foreach (int id in players.Keys)
        {
            NetworkServerProcessing.SendMessageToClient($"{ServerToClientSignifiers.PlayerVelocity},{clientId},{velocity.x},{velocity.y}", id, TransportPipeline.ReliableAndInOrder);
        }
    }
    public void AddPlayer(int playerId)
    {
        Character newPlayer = new Character(Vector2.zero, Vector2.zero);
        players.Add(playerId, newPlayer);

        // Notify all players about the new player
        foreach (int id in players.Keys)
        {
            NetworkServerProcessing.SendMessageToClient($"{ServerToClientSignifiers.CreateNewPlayer},{playerId},{newPlayer.position.x},{newPlayer.position.y}", id, TransportPipeline.ReliableAndInOrder);

            // If it's not the new player, notify the new player about the existing players
            if (id != playerId)
            {
                NetworkServerProcessing.SendMessageToClient($"{ServerToClientSignifiers.CreateNewPlayer},{id},{players[id].position.x},{players[id].position.y}", playerId, TransportPipeline.ReliableAndInOrder);
            }
        }
    }
    public void RemovePlayer(int playerId) 
    { 
        if(players.ContainsKey(playerId))
        {
            players.Remove(playerId);
            foreach (int id in players.Keys)
                NetworkServerProcessing.SendMessageToClient($"{ServerToClientSignifiers.RemovePlayer},{playerId}", playerId, TransportPipeline.ReliableAndInOrder);
        }
            
    }
}

public static class InputSendToSever
{
    public const int noKey = 0;
    public const int wdKey = 1;
    public const int waKey = 2;
    public const int sdKey = 3;
    public const int saKey = 4;
    public const int dKey  = 5;
    public const int aKey  = 6;
    public const int wKey  = 7;
    public const int sKey  = 8;
}

public struct Character
{
    public Vector2 position;
    public Vector2 velocity;
    public Character(Vector2 pos, Vector2 vel)
    {
        position = pos;
        velocity = vel;
    }
}
