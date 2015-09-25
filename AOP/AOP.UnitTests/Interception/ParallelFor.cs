using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AOP.UnitTests.Interception
{
    public class ParallelFor
    {
        [Fact]
        public void FancyTest()
        {
            Parallel.For(0, 10, i =>
            {
                var list = new List<int>();
                int c = 0;
                int sum = 0;
                for (int x = 0; x < 10000; x++)
                {
                    if (c != list.Capacity)
                    {
                        c = list.Capacity;
                        sum += c;
                    }
                    list.Add(sum);
                }
            });
        }
    }
}
