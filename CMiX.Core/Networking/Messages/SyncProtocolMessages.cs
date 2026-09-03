namespace CMiX.Core.Networking.Messages
{
    // Sync protocol messages pass the messenger even while sending is blocked.
    public static class SyncProtocolMessages
    {
        public static bool IsSyncProtocol(IMessage message) =>
            message is MessageProjectSnapshot;
    }
}
