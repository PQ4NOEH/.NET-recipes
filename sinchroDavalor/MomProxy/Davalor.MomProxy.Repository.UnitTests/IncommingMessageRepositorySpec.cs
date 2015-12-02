using Davalor.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Davalor.MomProxy.Repository.UnitTests
{
    public class IncommingMessageRepositorySpec
    {
        [Fact]
        public void If_the_file_PendingMessages_does_not_exist_creates_it()
        {
            try
            {
                Assert.False(File.Exists("PendingMessages"));
                var sut = new IncommingMessageRepository();
                Assert.True(File.Exists("PendingMessages"));
            }
            finally
            {
                File.Delete("PendingMessages");
            }
        }
        [Fact]
        public void On_save_appends_a_new_line()
        {
            try
            {
                var sut = new IncommingMessageRepository();
                List<string> newLines = new List<string>(StringExtension.RandomStrings(4, 20));
                newLines.ForEach(s => sut.Save(s));
                var fileLines = File.ReadAllLines("PendingMessages");
                Assert.Equal(fileLines.Count(), newLines.Count);
                for(int i =0; i < newLines.Count; i++) Assert.Equal(fileLines.ElementAt(i), newLines[i]);
            }
            finally
            {
                File.Delete("PendingMessages");
            }
        }

        [Fact]
        public void On_Delete_remove_the_line_keeping_the_file_order()
        {
            try
            {
                var sut = new IncommingMessageRepository();
                List<string> newLines = new List<string>(StringExtension.RandomStrings(10, 20));

                newLines.ForEach(s => sut.Save(s));
                var lineToDelete = newLines[new Random().Next(0, 9)];
                sut.Delete(lineToDelete);
                newLines.Remove(lineToDelete);
                var fileLines = File.ReadAllLines("PendingMessages");

                Assert.Equal(fileLines.Count(), newLines.Count);
                for (int i = 0; i < newLines.Count; i++) Assert.Equal(fileLines.ElementAt(i), newLines[i]);
            }
            finally
            {
                File.Delete("PendingMessages");
            }
        }

        [Fact]
        public void GetPending_returns_all_messages()
        {
            try
            {
                var sut = new IncommingMessageRepository();
                List<string> newLines = new List<string>(StringExtension.RandomStrings(10, 20));
                newLines.ForEach(s => sut.Save(s));

                var fileLines = sut.GetPending().Value;

                Assert.Equal(fileLines.Count(), newLines.Count);
                for (int i = 0; i < newLines.Count; i++) Assert.Equal(fileLines.ElementAt(i), newLines[i]);
            }
            finally
            {
                File.Delete("PendingMessages");
            }
        }
    }
}
