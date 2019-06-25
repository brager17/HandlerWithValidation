using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ValidationHandlerTemplate
{
    public abstract class ValidationHandler : IHandler<TIn, TOut>, IValidator<TIn>
    {
        private TIn _handlerInput;
        private TIn _validationInput;
        private IEnumerator<(Action<TIn>, Func<bool>, ValidationResult)> _enumerator;

        private IEnumerator<(Action<TIn> Initialize, Func<bool> Check, ValidationResult Result)>
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

        protected abstract IEnumerable<Action<TIn>> Initializing();

        protected abstract IEnumerable<(Func<bool>, ValidationResult)> Validators();

        public TOut Handle(TIn input)
        {
            _handlerInput = input;
            //initialize other
            while (_enumerator.MoveNext())
            {
                ValidationEnumerator.Current.Item1(input);
            }

            return _Handle(input);
        }

        protected abstract TOut _Handle(TIn input);

        public virtual IEnumerable<ValidationResult> Validate(TIn obj)
        {
            _validationInput = obj;
            while (ValidationEnumerator.MoveNext())
            {
                ValidationEnumerator.Current.Initialize(obj);
                if (ValidationEnumerator.Current.Check())
                {
                    return new[] {ValidationEnumerator.Current.Result};
                }
            }

            return null;
        }
    }
}