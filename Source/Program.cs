using System;
using System.Linq.Expressions;
using Surrogate.Samples;

namespace Surrogate
{
    class Program
    {
        static void Main(string[] args)
        {
            // SurrogateBuilder.FindAttributes(123456);
            // var asd = Activator.CreateInstance(typeof(MethodSurrogateInfo), new object[] { 12345 });
            // var member = typeof(Foo).GetMethod(nameof(Foo.ActualMethod));
            // Expression<Func<Attribute>> f = () => Attribute.GetCustomAttribute(member, typeof(Foo));
            // Expression<Action> f = () => this.foobar();

            var instance = (Foo)SurrogateBuilder.Build<Foo>();
            // var instance = new Foo();
            // retType.GetMethod("NewM").Invoke(instance, null);
            var num = 456;
            var strings = "Foobar is real";
            var retVal = instance.ActualMethod(ref strings, ref num);
            // instance.foo();
        }

        void foobar()
        {

        }
    }

    public class Foo
    {
        [return: Clamp(5, 10)]
        [MethodSurrogate]
        public virtual int ActualMethod([NotNull] ref string InputText, ref int InputNum)
        {
            Console.WriteLine("Actual Method");
            Console.WriteLine($"Received : {InputText}");
            InputNum = 123;
            return InputNum + 1;
        }
    }
}
