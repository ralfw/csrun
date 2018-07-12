using System;
using System.Threading;
using csrun.adapters.providers;
using NUnit.Framework;

namespace csrun.tests
{
    [TestFixture]
    public class BlockingTimer_tests
    {
        [Test]
        public void Fires_regularly()
        {
            var sut = new BlockingTimer(50);
            var i = 0;
            sut.Start(() => {
                i++;
                if (i==3) sut.Stop();
            });
            sut.Wait();
            Assert.AreEqual(3, i);
        }

        [Test]
        public void Blocks_while_notification_is_handled()
        {
            var sut = new BlockingTimer(50);
            var i = 0;
            sut.Start(() => {
                i++;
                Thread.Sleep(200);
                sut.Stop();
            });
            sut.Wait();
            Assert.AreEqual(1, i);
        }
    }
}