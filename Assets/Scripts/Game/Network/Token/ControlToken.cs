using UnityEngine;
using System.Collections;

public class ControlToken : Bolt.IProtocolToken
{
    public uint playerID;

    public ControlToken() { }

    public ControlToken(uint _playerID)
    {
        playerID = _playerID;
    }

    public void Write(UdpKit.UdpPacket _packet)
    {
        _packet.WriteUInt(playerID);
    }

    public void Read(UdpKit.UdpPacket _packet)
    {
        playerID = _packet.ReadUInt();
    }
}
