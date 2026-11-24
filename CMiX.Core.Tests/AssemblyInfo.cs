using Xunit;

// Some statics remain, so a test class that changes one would change what the other test classes see in a parallel run.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
