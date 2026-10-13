using System;
using System.Threading;
using NetMQ.Core;
using NetMQ.Core.Transports.Pgm;
using NetMQ.Monitoring;
using NetMQ.Sockets;
using Xunit;

namespace NetMQ.Tests
{
    public class ForkCompatibilityTests : IClassFixture<CleanupAfterFixture>
    {
        public ForkCompatibilityTests() => NetMQConfig.Cleanup();

        [Fact]
        public void SubscriberConnectedCallbackDoesNotRequireMonitorPoller()
        {
            using var connected = new ManualResetEventSlim();
            using var publisher = new PublisherSocket();
            using var subscriber = new SubscriberSocket();
            int port = publisher.BindRandomPort("tcp://127.0.0.1");
            string endpoint = "tcp://127.0.0.1:" + port;
            MonitorEvent? received = null;

            subscriber.RegisterMonitorCallback(e =>
            {
                received = e;
                connected.Set();
            }, SocketEvents.Connected);
            subscriber.SubscribeToAnyTopic();
            subscriber.Connect(endpoint);

            Assert.True(connected.Wait(TimeSpan.FromSeconds(10)));
            Assert.NotNull(received);
            Assert.Equal(SocketEvents.Connected, received.Event);
            Assert.Equal(endpoint, received.Addr);
        }

        [Fact]
        public void DirectCallbackAndStandardMonitorBothReceiveEvent()
        {
            using var direct = new ManualResetEventSlim();
            using var monitored = new ManualResetEventSlim();
            using var publisher = new PublisherSocket();
            using var poller = new NetMQPoller();
            publisher.RegisterMonitorCallback(_ => direct.Set(), SocketEvents.Listening);
            using var monitor = new NetMQMonitor(publisher, "inproc://fork-monitor-" + Guid.NewGuid(),
                SocketEvents.Listening);
            monitor.Listening += (_, _) => monitored.Set();
            monitor.AttachToPoller(poller);
            poller.RunAsync();

            publisher.BindRandomPort("tcp://127.0.0.1");

            Assert.True(direct.Wait(TimeSpan.FromSeconds(10)));
            Assert.True(monitored.Wait(TimeSpan.FromSeconds(10)));
        }

        [Fact]
        public void CallbackReplacementPreservesFilteringAndSynchronousDelivery()
        {
            using var socket = new SubscriberSocket();
            int firstCalls = 0;
            int callbackThread = -1;
            socket.RegisterMonitorCallback(_ => firstCalls++, SocketEvents.Connected);
            socket.RegisterMonitorCallback(_ => callbackThread = Thread.CurrentThread.ManagedThreadId,
                SocketEvents.Listening);

            socket.SocketHandle.EventConnected("test", null!);
            Assert.Equal(0, firstCalls);
            Assert.Equal(-1, callbackThread);

            socket.SocketHandle.EventListening("test", null!);
            Assert.Equal(Thread.CurrentThread.ManagedThreadId, callbackThread);
        }

        [Fact]
        public void CallbackExceptionsPropagateToEventRaiser()
        {
            using var socket = new SubscriberSocket();
            var expected = new InvalidOperationException("callback failure");
            socket.RegisterMonitorCallback(_ => throw expected, SocketEvents.Connected);

            Assert.Same(expected, Assert.Throws<InvalidOperationException>(
                () => socket.SocketHandle.EventConnected("test", null!)));
        }

        [Fact]
        public void SetSizeRespectsTrimmedBufferBounds()
        {
            var msg = new Msg();
            msg.InitGC(new byte[16], 16);
            try
            {
                msg.TrimPrefix(4);
                msg.SetSize(12);
                Assert.Equal(12, msg.Size);
                Assert.Throws<ArgumentOutOfRangeException>(() => msg.SetSize(13));
                Assert.Throws<ArgumentOutOfRangeException>(() => msg.SetSize(-1));
                msg.SetSize(0);
                Assert.True(msg.IsEmpty);
            }
            finally
            {
                msg.Close();
            }
        }

        [Fact]
        public void SharedMessageMonitoringDoesNotResetOutstandingReferences()
        {
            var msg = new Msg();
            var copy = new Msg();
            msg.InitPool(16);
            copy.InitEmpty();
            try
            {
                Assert.True(msg.TrySetShared(out var counter));
                Assert.True(msg.IsShared);
                Assert.Equal(1, counter.Decrement(0));
                copy.Copy(ref msg);
                Assert.Equal(2, counter.Decrement(0));
                Assert.True(msg.TrySetShared(out var sameCounter));
                Assert.Same(counter, sameCounter);
                Assert.Equal(2, counter.Decrement(0));
                copy.Close();
                Assert.Equal(1, counter.Decrement(0));
            }
            finally
            {
                if (copy.IsInitialised)
                    copy.Close();
                msg.Close();
            }
        }

        [Fact]
        public void PgmStatisticsCallbackOptionRoundTrips()
        {
            using var publisher = new PublisherSocket();
            var callback = new PgmSenderStatsCallback(_ => { }, TimeSpan.FromSeconds(10));
            publisher.Options.PgmSenderStatsCallback = callback;
            Assert.Same(callback, publisher.Options.PgmSenderStatsCallback);
            publisher.Options.PgmSenderStatsCallback = null;
            Assert.Null(publisher.Options.PgmSenderStatsCallback);
        }

        [Fact]
        public void PgmStatisticsDecodeAllNativeCounters()
        {
            var buffer = new byte[13 * sizeof(ulong)];
            for (int i = 0; i < 13; i++)
                BitConverter.GetBytes((ulong)(i + 1)).CopyTo(buffer, i * sizeof(ulong));

            PgmSenderStats.FromBuffer(buffer, out var stats);
            Assert.Equal(new ulong[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                new[] { stats.DataBytesSent, stats.TotalBytesSent, stats.NaksReceived,
                    stats.NaksReceivedTooLate, stats.NumOutstandingNaks, stats.NumNaksAfterRData,
                    stats.RepairPacketsSent, stats.BufferSpaceAvailable, stats.TrailingEdgeSeqId,
                    stats.LeadingEdgeSeqId, stats.RateKBitsPerSecOverall, stats.RateKBitsPerSecLast,
                    stats.TotalODataPacketsSent });
        }
    }
}
