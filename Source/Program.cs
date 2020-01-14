using System;
using System.Linq.Expressions;
using Surrogate.Samples;

namespace Surrogate
{
    class Program
    {
        static void Main(string[] args)
        {
            var instance = (Foo)SurrogateBuilder.Build<Foo>();
            var num = 456;
            var strings = "Foobar is real";
            var retVal = instance.ActualMethod(ref strings, ref num);
        }

        void foobar()
        {

        }
    }

    public class Foo
    {
        [return: Clamp(5, 10)]
        // [MethodSurrogate]
        [Bypass]
        public virtual int ActualMethod([NotNull] ref string InputText, ref int InputNum)
        {
            Console.WriteLine("Actual Method");
            Console.WriteLine($"Received : {InputText}");
            InputNum = 123;
            return InputNum + 1;
        }
    }
}
