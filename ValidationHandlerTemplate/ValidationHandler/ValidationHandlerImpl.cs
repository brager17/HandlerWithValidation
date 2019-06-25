using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ValidationHandlerTemplate
{
    public class ValidationHandlerImpl : ValidationHandler
    {
        private string Name { get; set; }
        private string Surname { get; set; }
        private int? Age { get; set; }

        protected override IEnumerable<Action<TIn>> Initializing()
        {
            yield return o => Name = GetName(o);
            yield return o => Surname = GetSurname(o);
            yield return o => Age = GetAge(o);
        }

        protected override IEnumerable<(Func<bool>, ValidationResult)> Validators()
        {
            yield return (() => Name != null, new ValidationResult("1"));
            yield return (() => Surname != null, new ValidationResult("2"));
            yield return (() => Age != null, new ValidationResult("3"));
        }


        private string GetName(TIn input) => input.Name;

        private string GetSurname(TIn input) => input.Surname;

        private int? GetAge(TIn input) => input.Age;



        protected override TOut _Handle(TIn input)
        {
            throw new NotImplementedException();
        }
    }
}