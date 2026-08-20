namespace CMiX.Core.Networking.Messages
{
    // Sync-protocol messages must always get through, even while unsynced, or the mismatch could
    // never be resolved.
    public static class SyncProtocolMessages
    {
        public static bool IsSyncProtocol(IMessage message) =>
            message is MessageStateHash or MessageProjectSnapshot or MessageRequestSnapshot;
    }
}
