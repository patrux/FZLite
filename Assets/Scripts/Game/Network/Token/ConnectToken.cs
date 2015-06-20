using UnityEngine;
using System.Collections;

/// <summary>
/// ConnectToken stores information about a player from the moment when he connected to the server.
/// Only contains information about a single player.
/// </summary>
public class ConnectToken : Bolt.IProtocolToken
{
    // The name of the player
    public string playerName;

    // If the player is ready or not
    public bool isReady;

    // What "player slot" the player has in the game lobby
    public byte slotID;

    // Client only, track other clients by their id
    public uint playerID;

    public ConnectToken(){ }

    public ConnectToken(string _playerName, bool _readyStatus, byte _slotID, uint _playerID)
    {
        playerName = _playerName;
        isReady = _readyStatus;
        slotID = _slotID;
        playerID = _playerID;
    }

    public void Write(UdpKit.UdpPacket _packet)
    {
        _packet.WriteString(playerName);
        _packet.WriteBool(isReady);
        _packet.WriteByte(slotID);
        _packet.WriteUInt(playerID);
    }

    public void Read(UdpKit.UdpPacket _packet)
    {
        playerName = _packet.ReadString();
        isReady = _packet.ReadBool();
        slotID = _packet.ReadByte();
        playerID = _packet.ReadUInt();
    }

    public string ToString()
    {
        return "playerName[" + playerName + "] isReady[" + isReady + "] slotID[" + slotID + "] playerID[" + playerID + "]";
    }
}
