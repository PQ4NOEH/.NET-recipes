using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Compresion.UnitTests
{
    public class DeflateSpec
    {
        public byte[] Compress(byte[] data)
        {
            using (MemoryStream destination = new MemoryStream())
            using (MemoryStream source = new MemoryStream(data))
            {
                DeflateStream deflate = new DeflateStream(destination, CompressionMode.Compress);
                source.CopyTo(deflate);
                return destination.GetBuffer();
            }
        }

        public byte[] Decompress(byte[] data)
        {
            using (MemoryStream destination = new MemoryStream())
            using (MemoryStream source = new MemoryStream(data))
            {
                DeflateStream deflate = new DeflateStream(source, CompressionMode.Decompress);
                deflate.CopyTo(destination);
                return destination.GetBuffer();
            }
        }

        [Fact]
        public void Can_do_compresion()
        {
            Byte[] initial = File.ReadAllBytes("ØMQ.pdf");
            byte[] afterCompression = Compress(initial);
            Assert.True(initial.Length > afterCompression.Length);
        }

        [Fact]
        public void Can_do_decompresion()
        {
            Byte[] initial = File.ReadAllBytes("ØMQ.pdf");
            byte[] afterCompression = Compress(initial);
            byte[] afterDecompression = Decompress(afterCompression);
            Assert.True(initial.Length <=  afterDecompression.Length);
        }
    }
}
