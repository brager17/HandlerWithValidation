using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ValidationHandlerTemplate
{
    public abstract class AsyncValidationHandlerV3 : IAsyncHandler<TIn, TOut>, IAsyncValidator<TIn>,IDisposable
    {
        private TIn _previousInput;
        private IEnumerator<(Func<TIn, CancellationToken, Task>, Func<bool>, ValidationResult)> _enumerator;

        private IEnumerator<(Func<TIn, CancellationToken, Task> Initialize, Func<bool> Check, ValidationResult
            Result)> GetEnumerator(TIn input)
        {
            if (_previousInput == null || _previousInput != input)
            {
                _previousInput = input;
                return _enumerator = CreateEnumerator();
            }

            if (_previousInput == input)
            {
                return _enumerator;
            }

            throw new ArgumentException();
        }

        private IEnumerator<(Func<TIn, CancellationToken, Task> initialize, Func<bool> check, ValidationResult result)>
            CreateEnumerator()
        {
            return Initializing()
                .Zip(Validators(), (a, c) => (initialize: a, check: c.Item1, result: c.Item2))
                .GetEnumerator();
        }

        protected abstract IEnumerable<Func<TIn, CancellationToken, Task>> Initializing();

        protected abstract IEnumerable<(Func<bool>, ValidationResult)> Validators();

        public async Task<TOut> AsyncHandle(TIn input, CancellationToken ct)
        {
            //initialize other
            var ve = GetEnumerator(input);
            while (ve.MoveNext())
            {
                await ve.Current.Item1(input, ct);
            }

            return await Handle(input);
        }

        protected abstract Task<TOut> Handle(TIn input);

        public virtual async Task<IEnumerable<ValidationResult>> Validate(TIn obj, CancellationToken ct)
        {
            var ve = GetEnumerator(obj);
            while (ve.MoveNext())
            {
                await ve.Current.Initialize(obj, ct);
                if (ve.Current.Check())
                {
                    return await Task.FromResult(new[] {ve.Current.Result});
                }
            }

            return null;
        }

        public void Dispose()
        {
            _enumerator?.Dispose();
        }
    }
}