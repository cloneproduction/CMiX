namespace CMiX.Core.Networking.Messages
{
    // The state-hash/push-pull handshake has to keep working even while everything else is
    // deliberately blocked because the two sides are not in sync - otherwise the mismatch could
    // never be resolved. Both the outgoing block (ControlMessenger) and the incoming block
    // (Server.MessageReceived) check this so a message never has to be special-cased in two
    // different ways.
    public static class SyncProtocolMessages
    {
        public static bool IsSyncProtocol(IMessage message) =>
            message is MessageStateHash or MessageProjectSnapshot;
    }
}
