using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ValidationHandlerTemplate.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task<object> Test1()
        {
            var handler = new AsyncEnumerableValidationHandlerV1();
            await foreach (var unused in handler.Validate(new TIn()))
            {
            }

            var s = await handler.AsyncHandle(new TIn(), CancellationToken.None);
            return s;
        }
    }
}