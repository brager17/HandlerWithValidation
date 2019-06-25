using System;
using System.Threading.Tasks;
using Xunit;

namespace ValidationHandlerTemplate.Tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task<object> Test1()
        {
            var handler = new AsyncEnumerabeValidationHandlerV1();
            await foreach (var validationResult in handler.Validate(new TIn()))
            {
            }

            var s = await handler.AsyncHandle(new TIn());


            return s;
        }
    }
}