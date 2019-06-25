using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ValidationHandlerTemplate
{
    public class TIn
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int? Age { get; set; }
    }

    public class TOut
    {
        public string TypeName;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    public interface IHandler<in TIn, out TOut>
    {
        TOut Handle(TIn input);
    }

    public interface IValidator<T>
    {
        IEnumerable<ValidationResult> Validate(T obj);
    }

    public interface IAsyncHandler<T, T1>
    {
        Task<T1> AsyncHandle(T input);
    }

    public interface IAsyncEnumerabeValidator<T>
    {
        IAsyncEnumerable<ValidationResult> Validate(T obj);
    }

    public interface IAsyncValidator<T>
    {
        Task<IEnumerable<ValidationResult>> Validate(T obj);
    }
}