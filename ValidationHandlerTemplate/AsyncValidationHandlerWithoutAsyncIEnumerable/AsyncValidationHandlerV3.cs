using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ValidationHandlerTemplate
{
    public abstract class AsyncValidationHandlerV3 : IAsyncHandler<TIn, TOut>, IAsyncValidator<TIn>
    {
        private TIn _handlerInput;
        private TIn _validationInput;
        private IEnumerator<(Func<TIn, CancellationToken, Task>, Func<bool>, ValidationResult)> _enumerator;

        private IEnumerator<(Func<TIn, CancellationToken, Task> Initialize, Func<bool> Check, ValidationResult Result)>
            ValidationEnumerator
        {
            get
            {
                return _handlerInput == _validationInput
                    ? _enumerator
                    : _enumerator = Initializing()
                        .Zip(Validators(), (a, c) => (initialize: a, check: c.Item1, result: c.Item2)).GetEnumerator();
            }
        }

        protected abstract IEnumerable<Func<TIn, CancellationToken, Task>> Initializing();

        protected abstract IEnumerable<(Func<bool>, ValidationResult)> Validators();

        public async Task<TOut> AsyncHandle(TIn input, CancellationToken ct)
        {
            _handlerInput = input;
            //initialize other
            while (_enumerator.MoveNext())
            {
                await _enumerator.Current.Item1(input, ct);
            }

            return await Handle(input);
        }

        protected abstract Task<TOut> Handle(TIn input);

        public virtual async Task<IEnumerable<ValidationResult>> Validate(TIn obj, CancellationToken ct)
        {
            _validationInput = obj;
            while (ValidationEnumerator.MoveNext())
            {
                await ValidationEnumerator.Current.Initialize(obj, ct);
                if (ValidationEnumerator.Current.Check())
                {
                    return await Task.FromResult(new[] {ValidationEnumerator.Current.Result});
                }
            }

            return null;
        }
    }
}