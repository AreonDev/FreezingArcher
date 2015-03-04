namespace FreezingArcher.Messaging
{
    /// <summary>
    /// Enum which holds all known MessageId's 
    /// MessageId's must be unique for each type of IMessage
    /// </summary>
    public enum MessageId
    {
        EntityMoved = 1,
        RegisterGraphicsResource = 2,
        GenerateContent = 3,
        GenerateTerrainRequest = 4,
    }
}