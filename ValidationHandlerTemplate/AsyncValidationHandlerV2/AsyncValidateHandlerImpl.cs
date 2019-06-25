using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ValidationHandlerTemplate
{
    public class AsyncEnumerabeValidateHandlerImpl : AsyncEnumerabeValidationHandlerV2
    {
        private string Name { get; set; }
        private string Surname { get; set; }
        private int? Age { get; set; }

        protected override IEnumerable<Func<TIn, Task>> Initializing()
        {
            yield return async o => Name = await GetName(o);
            yield return async o => Surname = await GetSurname(o);
            yield return async o => Age = await GetAge(o);
        }

        protected override IEnumerable<(Func<bool>, ValidationResult)> Validators()
        {
            yield return (() => Name != null, new ValidationResult("1"));
            yield return (() => Surname != null, new ValidationResult("2"));
            yield return (() => Age != null, new ValidationResult("3"));
        }

        protected override Task<TOut> Handle(TIn input)
        {
            throw new NotImplementedException();
        }

        private async Task<string> GetName(TIn input) =>
            await Task.Delay(1000).ContinueWith(x => input.Name);

        private async Task<string> GetSurname(TIn input) =>
            await Task.Delay(1000).ContinueWith(x => input.Surname);

        private async Task<int?> GetAge(TIn input) => await
            Task.Delay(1000).ContinueWith(x => input.Age);
    }
}