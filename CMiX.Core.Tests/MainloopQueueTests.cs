using CMiX.Core.Networking;
using Xunit;

namespace CMiX.Core.Tests
{
    public class MainloopQueueTests
    {
        [Fact]
        public async Task Post_ThenDrain_RunsTheActionsInOrderOnTheDrainingThread()
        {
            var queue = new MainloopQueue();
            var order = new List<int>();
            var threads = new List<int>();

            await Task.Run(() =>
            {
                for (var i = 1; i <= 3; i++)
                {
                    var index = i;
                    queue.Post(() =>
                    {
                        order.Add(index);
                        threads.Add(Environment.CurrentManagedThreadId);
                    });
                }
            });

            Assert.Empty(order);

            var ran = queue.Drain();

            Assert.Equal(3, ran);
            Assert.Equal(new[] { 1, 2, 3 }, order);
            Assert.All(threads, id => Assert.Equal(Environment.CurrentManagedThreadId, id));
        }

        [Fact]
        public void Drain_RunsOnlyTheActionsThatWereQueuedBeforeIt()
        {
            var queue = new MainloopQueue();
            var secondRan = false;
            queue.Post(() => queue.Post(() => secondRan = true));

            Assert.Equal(1, queue.Drain());
            Assert.False(secondRan);
            Assert.Equal(1, queue.Count);

            Assert.Equal(1, queue.Drain());
            Assert.True(secondRan);
        }

        [Fact]
        public void AnActionThatThrows_DoesNotStopTheNextOne()
        {
            var queue = new MainloopQueue();
            var thirdRan = false;
            queue.Post(() => { });
            queue.Post(() => throw new InvalidOperationException("test"));
            queue.Post(() => thirdRan = true);

            Assert.Equal(3, queue.Drain());
            Assert.True(thirdRan);
        }

        [Fact]
        public void Post_AfterClose_RunsAtOnce()
        {
            var queue = new MainloopQueue();
            var firstRan = false;
            var secondRan = false;
            queue.Post(() => firstRan = true);

            queue.Close();

            Assert.True(firstRan);

            queue.Post(() => secondRan = true);

            Assert.True(secondRan);
            Assert.Equal(0, queue.Count);
        }

        [Fact]
        public void Post_WithNull_DoesNothing()
        {
            var queue = new MainloopQueue();

            queue.Post(null);

            Assert.Equal(0, queue.Count);
            Assert.Equal(0, queue.Drain());
        }
    }
}
