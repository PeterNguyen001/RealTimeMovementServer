using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    Vector2 characterVelocityInPercent;
    const float CharacterSpeed = 0.25f;
    float DiagonalCharacterSpeed;

    Dictionary<int, Character> players = new Dictionary<int, Character>();
    void Start()
    {
        DiagonalCharacterSpeed = Mathf.Sqrt(CharacterSpeed * CharacterSpeed + CharacterSpeed * CharacterSpeed) / 2f;
        NetworkServerProcessing.SetGameLogic(this);
    }

    public void HandlePlayerInput(int clientId, int input)
    {
        Character character = players[clientId];
        characterVelocityInPercent = Vector2.zero;

        if(input == InputSendToSever.wdKey ) 
        {
            characterVelocityInPercent.x = DiagonalCharacterSpeed;
            characterVelocityInPercent.y = DiagonalCharacterSpeed;
        }
        else if (input == InputSendToSever.waKey)
        {
            characterVelocityInPercent.x = -DiagonalCharacterSpeed;
            characterVelocityInPercent.y = DiagonalCharacterSpeed;
        }
        else if( input == InputSendToSever.sdKey ) 
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
        else if(input == InputSendToSever.wKey)
            characterVelocityInPercent.y = CharacterSpeed;
        else if (input == InputSendToSever.sKey)
            characterVelocityInPercent.y = -CharacterSpeed;

        character.velocityX = characterVelocityInPercent.x;
        character.velocityY = characterVelocityInPercent.y;

        players[clientId] = character;

        foreach( int id in players.Keys ) 
        {          
            NetworkServerProcessing.SendMessageToClient($"{ServerToClientSignifiers.PlayerVelocity},{characterVelocityInPercent.x},{characterVelocityInPercent.y}", clientId, TransportPipeline.ReliableAndInOrder);
        }
        
    }

    public void AddPlayer(int  id) 
    { 
        if(players.Count == 0)
        {
            Character character = new Character();
            players.Add(id,character);
        }
    }
    public void RemovePlayer(int id) 
    { 
        if(players.ContainsKey(id))
        {
            players.Remove(id);
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
    public float positionX;
    public float positionY;
    public float velocityX;
    public float velocityY;
    public Character(float posX,float posY, float velX, float velY)
    {
        this.positionX = posX;
        this.positionY = posY;
        this.velocityX = velX;
        this.velocityY = velY;
    }
}
