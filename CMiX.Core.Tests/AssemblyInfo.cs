using Xunit;

// WeakReferenceMessenger.Default is shared by every control, so the tests run one at a time.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
