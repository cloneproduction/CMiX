using Xunit;

// ValueInteraction is a process wide ambient scope, so a test class that opens one would change
// what every other test class observes while xUnit runs them in parallel.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
