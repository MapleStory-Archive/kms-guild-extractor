using System.Threading;
using System.Threading.Tasks;
using KMSGuildExtractor.Core;
using KMSGuildExtractor.Core.Info;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class WebParsingTest
    {

        [TestMethod]
        public void GuildSearchTest1()
        {
            var tokenSource = new CancellationTokenSource();
            Task<(bool, int)> task = Guild.TryFindGuildID("���׸޸�ȣ", WorldID.Reboot, tokenSource.Token);
            Task.WaitAll(task);

            Assert.AreEqual(task.Result, (true, 2210));
        }

        [TestMethod]
        public void GuildSearchTest2()
        {
            var tokenSource = new CancellationTokenSource();
            Task<(bool, int)> task = Guild.TryFindGuildID("���׸�", WorldID.Reboot, tokenSource.Token);
            Task.WaitAll(task);

            Assert.AreEqual(task.Result, (false, -1));
        }

        [TestMethod]
        public void GuildSearchTest3()
        {
            var tokenSource = new CancellationTokenSource();
            Task<(bool, int)> task = Guild.TryFindGuildID("���屳", WorldID.Scania, tokenSource.Token);
            Task.WaitAll(task);

            Assert.AreEqual(task.Result, (true, 241077));
        }

        [TestMethod]
        public void GuildSearchTest4()
        {
            var tokenSource = new CancellationTokenSource();
            Task<(bool, int)> task = Guild.TryFindGuildID("���", WorldID.Burning, tokenSource.Token);
            Task.WaitAll(task);

            Assert.AreEqual(task.Result, (true, 2));
        }
    }
}
