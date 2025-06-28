using Fusion;

public struct NetworkPlayerState : INetworkStruct

{
    public int Level;
    public Direction upDirection;
}

public enum PlayerRole : byte
{
    Host,
    Client
}