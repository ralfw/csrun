using System;
using System.IO;
using System.Threading;
using csrun.adapters.providers;
using NUnit.Framework;

namespace csrun.tests
{
    [TestFixture]
    public class Filesystem_tests
    {
        [Test]
        public void Detect_file_change()
        {
            const string FILENAME = "test.txt";
            File.WriteAllText(FILENAME, "a");
            var sut = new Filesystem("template.cs");
            
            var timestamp = File.GetLastWriteTime(FILENAME);
            var result = sut.FileHasChanged(FILENAME, ref timestamp);
            Assert.IsFalse(result);

            timestamp = timestamp.Subtract(TimeSpan.FromSeconds(5));
            result = sut.FileHasChanged(FILENAME, ref timestamp);
            Assert.IsTrue(result);
        }
    }
}