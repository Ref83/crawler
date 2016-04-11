using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace WebCrawler.Tests.Helpers
{
    public static class AssertExtensions
    {
        public static void AreListEqual<T>(T[] expected, IEnumerable<T> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count());

            var index = 0;
            foreach (var item in actual)
            {
                Assert.AreEqual(expected[index], item);
                index++;
            }
        }

        public static void IsException(Action action, Type exceptionType)
        {
            try
            {
                action();
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, exceptionType);
            }
        }

        public static async Task IsExceptionAsync(Func<Task> awaitable, Type exceptionType)
        {
            try
            {
                await awaitable();
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, exceptionType);
            }
        }
    }
}
