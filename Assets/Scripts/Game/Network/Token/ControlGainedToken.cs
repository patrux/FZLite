using UnityEngine;
using System.Collections;

public class ControlGainedToken : Bolt.IProtocolToken
{
    public string gameObjectName;

    public ControlGainedToken() { }

    public ControlGainedToken(string _gameObjectName)
    {
        gameObjectName = _gameObjectName;
    }

    public void Write(UdpKit.UdpPacket _packet)
    {
        _packet.WriteString(gameObjectName);
    }

    public void Read(UdpKit.UdpPacket _packet)
    {
        gameObjectName = _packet.ReadString();
    }
}
