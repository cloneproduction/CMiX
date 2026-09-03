# Redis integration tests

`RedisSyncStoreIntegrationTests` and `SyncPeerRedisIntegrationTests` use a real server. They need a
Redis or a Memurai on `127.0.0.1:6379`.

The fixture connects once when the test run starts. When no server answers in 1 second, all tests
that need the server are skipped. The other tests run as usual.

Each test writes its keys under its own random prefix, `cmix:test:` and 8 hexadecimal characters.
The test deletes these keys when it ends. The tests never touch `cmix:default`, the prefix of the
application.

To run only these tests:

    dotnet test CMiX.Core.Tests/CMiX.Core.Tests.csproj --nologo -v q --filter "FullyQualifiedName~SyncPeerRedisIntegrationTests"
    dotnet test CMiX.Core.Tests/CMiX.Core.Tests.csproj --nologo -v q --filter "FullyQualifiedName~RedisSyncStoreIntegrationTests"

To see the measured message delays, add `--logger "console;verbosity=detailed"`.
