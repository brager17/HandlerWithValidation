using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ValidationHandlerTemplate
{
    public abstract class AsyncEnumerabeValidationHandlerV2 : IAsyncHandler<TIn, TOut>, IAsyncEnumerabeValidator<TIn>
    {
        private TIn _handlerInput;
        private TIn _validationInput;
        private IEnumerator<(Func<TIn, Task>, Func<bool>, ValidationResult)> _enumerator;

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

        protected abstract IEnumerable<Func<TIn, Task>> Initializing();

        protected abstract IEnumerable<(Func<bool>, ValidationResult)> Validators();

        public async Task<TOut> AsyncHandle(TIn input)
        {
            _handlerInput = input;
            //initialize other
            while (_enumerator.MoveNext())
            {
                await _enumerator.Current.Item1(input);
            }

            return await Handle(input);
        }

        protected abstract Task<TOut> Handle(TIn input);

        public virtual async IAsyncEnumerable<ValidationResult> Validate(TIn obj)
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
    }
}