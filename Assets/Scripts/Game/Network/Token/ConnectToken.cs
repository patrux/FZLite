using UnityEngine;
using System.Collections;

public class ConnectToken : Bolt.IProtocolToken
{
    public string playerName;
    public bool readyStatus;
    byte ability0;
    byte ability1;

    public ConnectToken()
    { }
    public ConnectToken(string _playerName, bool _readyStatus)
    {
        playerName = _playerName;
        readyStatus = _readyStatus;
        ability0 = 0;
        ability1 = 1;
    }

    public void Write(UdpKit.UdpPacket _packet)
    {
        _packet.WriteString(playerName);
        _packet.WriteBool(readyStatus);
        _packet.WriteByte(ability0);
        _packet.WriteByte(ability1);
    }

    public void Read(UdpKit.UdpPacket _packet)
    {
        playerName = _packet.ReadString();
        readyStatus = _packet.ReadBool();
        ability0 = _packet.ReadByte();
        ability1 = _packet.ReadByte();
    }
}
