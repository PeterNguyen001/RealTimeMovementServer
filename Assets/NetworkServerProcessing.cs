using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class NetworkServerProcessing
{
    #region Send and Receive Data Functions
    static public void ReceivedMessageFromClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        Debug.Log("Network msg received =  " + msg + ", from connection id = " + clientConnectionID + ", from pipeline = " + pipeline);

        string[] csv = msg.Split(',');
        int signifier = int.Parse(csv[0]);

        if (signifier == ClientToServerSignifiers.PlayerInput)
        {
            const int playerInput = 1;
            gameLogic.HandlePlayerInput(clientConnectionID,int.Parse(csv[playerInput]));
        }
        else if (signifier == ClientToServerSignifiers.updateHeartbeat)
        {
            networkServer.UpdateHeartbeatTime(clientConnectionID);
        }
    }
    static public void SendMessageToClient(string msg, int clientConnectionID, TransportPipeline pipeline)
    {
        networkServer.SendMessageToClient(msg, clientConnectionID, pipeline);
    }

    #endregion

    #region Connection Events

    static public void ConnectionEvent(int clientConnectionID)
    {
        gameLogic.AddPlayer(clientConnectionID);
        networkServer.AddPlayerToLastHeartbeat(clientConnectionID);
        Debug.Log("Client connection, ID == " + clientConnectionID);
    }
    static public void DisconnectionEvent(int clientConnectionID)
    {
        gameLogic.RemovePlayer(clientConnectionID);
        Debug.Log("Client disconnection, ID == " + clientConnectionID);
    }

    #endregion

    #region Setup
    static NetworkServer networkServer;
    static GameLogic gameLogic;

    static public void SetNetworkServer(NetworkServer NetworkServer)
    {
        networkServer = NetworkServer;
    }
    static public NetworkServer GetNetworkServer()
    {
        return networkServer;
    }
    static public void SetGameLogic(GameLogic GameLogic)
    {
        gameLogic = GameLogic;
    }

    #endregion
}

#region Protocol Signifiers
static public class ClientToServerSignifiers
{
    public const int PlayerInput = 2;
    public const int updateHeartbeat = 5;
}

static public class ServerToClientSignifiers
{
    public const int PlayerVelocity = 2;
    public const int OtherPlayersVelocity = 3;
    public const int CreateNewPlayer = 4;
    public const int RemovePlayer = 5;
}

#endregion

