using Fusion;

public struct NetworkPlayerState : INetworkStruct

{
    public int Level;
    public Direction UpDirection;
}

public enum PlayerRole : byte
{
    Host,
    Client,
    Waiting
}