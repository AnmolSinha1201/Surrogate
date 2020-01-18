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
            var instance = (Foo)SurrogateBuilder.Build<Foo>();
            var num = 456;
            var inputText = "Foobar is real";
            var retVal = instance.ActualMethod(null, ref num);
        }

        void foobar()
        {

        }
    }

    public class Foo
    {
        [return: ReturnSurrogate]
        [return: Clamp(5, 10)]
        [MethodSurrogate]
        // [Bypass]
        public virtual int ActualMethod([NotNull] string InputText, ref int InputNum)
        {
            Console.WriteLine("Actual Method");
            Console.WriteLine($"Received : {InputText}");
            InputNum = 123;
            return InputNum + 1;
        }
    }
}
