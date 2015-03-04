namespace FreezingArcher.Messaging
{
    /// <summary>
    /// Enum which holds all known MessageId's 
    /// MessageId's must be unique for each type of IMessage
    /// </summary>
    public enum MessageId
    {
        /// <summary>
        /// The entity moved.
        /// </summary>
        EntityMoved = 1,
        /// <summary>
        /// The register graphics resource.
        /// </summary>
        RegisterGraphicsResource = 2,
        /// <summary>
        /// The content of the generate.
        /// </summary>
        GenerateContent = 3,
        /// <summary>
        /// The generate terrain request.
        /// </summary>
        GenerateTerrainRequest = 4,
    }
}