using System;
using System.Linq.Expressions;
using Surrogate.Internal.BaseSurrogates;
using Surrogate.Samples;

namespace Surrogate
{
    class Program
    {
        static void Main(string[] args)
        {
            // var instance = new Foo("Foobar is real");
            var instance = (Foo)SurrogateBuilder.Build<Foo>("Foobar is real");
            var num = 456;
            var inputText = "Foobar is real";
            var retVal = instance.ActualMethod(inputText, num);
        }
    }

    public class Foo
    {
        public Foo() {}
        public Foo(string foobar)
        {

        }
        [return: ReturnSurrogate]
        // [return: Clamp(5, 10)]
        [MethodSurrogate]
        // [Bypass]
        public virtual int ActualMethod(string InputText, int InputNum)
        {
            Console.WriteLine("Actual Method");
            Console.WriteLine($"Received : {InputText}");
            InputNum = 123;
            return InputNum + 1;
        }
    }
}
