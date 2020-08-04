using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KMSGuildExtractor.Core;
using KMSGuildExtractor.Core.Info;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class GuildDetailParsingTest
    {

        [TestMethod]
        public void GuildDetailParsingTest1()
        {
            var tokenSource = new CancellationTokenSource();
            Task<GuildInfo> task = Guild.GetGuildDetailAsync(new GuildInfo("���׸޸�ȣ", WorldID.Reboot, 2210), tokenSource.Token);
            Task.WaitAll(task);

            //Assert.IsTrue(task.Result.Users.Any(u => u.Name == "ĸƾ�̸�����"));
            Assert.AreEqual(task.Result.Users.Count, 171);
        }

        [TestMethod]
        public void GuildDetailParsingTest2()
        {
            var tokenSource = new CancellationTokenSource();
            Task<GuildInfo> task = Guild.GetGuildDetailAsync(new GuildInfo("���屳", WorldID.Scania, 241077), tokenSource.Token);
            Task.WaitAll(task);

            //Assert.IsTrue(task.Result.Users.Any(u => u.Name == "�ų�" && u.Position == GuildPosition.Master));
            Assert.AreEqual(task.Result.Users.Count, 174);
        }
    }
}
