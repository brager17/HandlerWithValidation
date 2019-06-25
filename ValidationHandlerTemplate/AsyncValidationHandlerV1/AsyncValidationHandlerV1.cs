using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ValidationHandlerTemplate
{
    public class AsyncEnumerabeValidationHandlerV1 : IAsyncHandler<TIn, TOut>, IAsyncEnumerabeValidator<TIn>, IDisposable
    {
        private IEnumerator<(Func<TIn, Task>, Func<bool>, ValidationResult)> _enumerator;
        private string Name { get; set; }
        private string Surname { get; set; }
        private int? Age { get; set; }
        private TIn _handlerInput;
        private TIn _validationInput;

        private IEnumerable<Func<TIn, Task>> Initializing()
        {
            yield return async o => Name = await GetName(o);
            yield return async o => Surname = await GetSurname(o);
            yield return async o => Age = await GetAge(o);
        }

        private IEnumerable<(Func<bool>, ValidationResult)> Validators()
        {
            yield return (() => Name != null, new ValidationResult("1"));
            yield return (() => Surname != null, new ValidationResult("2"));
            yield return (() => Age != null, new ValidationResult("3"));
        }

        private IEnumerator<(Func<TIn, Task> Initialize, Func<bool> Check, ValidationResult Result)>
            ValidationEnumerator
        {
            get
            {
                return _handlerInput == _validationInput
                    ? _enumerator
                    : _enumerator??=Initializing()
                        .Zip(Validators(), (a, c) => (initialize: a, check: c.Item1, result: c.Item2)).GetEnumerator();
            }
        }

        public async Task<TOut> AsyncHandle(TIn input)
        {
            _handlerInput = input;
            //initialize other
            while (_enumerator.MoveNext())
            {
                await _enumerator.Current.Item1(input);
            }

            // do actions
            // return value
            return null;
        }

        public async IAsyncEnumerable<ValidationResult> Validate(TIn obj)
        {
            _validationInput = obj;
            while (ValidationEnumerator.MoveNext())
            {
                await ValidationEnumerator.Current.Initialize(obj);
                if (ValidationEnumerator.Current.Check())
                {
                    yield return ValidationEnumerator.Current.Result;
                    yield break;
                }
            }
        }

        private async Task<string> GetName(TIn input) =>
            await Task.Delay(1000).ContinueWith(x => input.Name);

        private async Task<string> GetSurname(TIn input) =>
            await Task.Delay(1000).ContinueWith(x => input.Surname);

        private async Task<int?> GetAge(TIn input) => await
            Task.Delay(1000).ContinueWith(x => input.Age);

        public void Dispose()
        {
            _enumerator?.Dispose();
        }
    }
}