using System;
using System.Linq.Expressions;
using System.Reflection;
using Surrogate.Internal.BaseSurrogates;
using Surrogate.Samples;

namespace Surrogate
{
    class Program
    {
        static void Main(string[] args)
        {
            // var instance = new Foo("Foobar is real");
            // var instance = (Foo)SurrogateBuilder.Build(typeof(Foo), "qweqweqwe");
            var instance = SurrogateBuilder.Build<Foo>("qweqweqwe", "asdasd");

            var num = 456;
            var inputText = "Foobar is real";
            // instance.ActualMethod(inputText, num);
            var retVal = (int)instance.ActualMethod(inputText, ref num);
            // instance.GetType().GetMethod("ActualMethod").GetMethodBody().GetILAsByteArray()
        }
    }

    public class Foo
    {
        public Foo() 
        {

        }
        public Foo(params string[] foobar)
        {

        }
        [return: ReturnSurrogate]
        // [return: Clamp(5, 10)]
        [MethodSurrogate]
        // [Bypass]
        public virtual int ActualMethod([ParameterSurrogate] string InputText, ref int InputNum)
        {
            Console.WriteLine("Actual Method");
            Console.WriteLine($"Received : {InputText}");
            InputNum = 123;
            return InputNum + 1;
        }
    }
}
