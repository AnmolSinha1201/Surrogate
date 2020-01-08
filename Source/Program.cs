using System;
using System.Linq.Expressions;
using Surrogate.Samples;

namespace Surrogate
{
    class Program
    {
        static void Main(string[] args)
        {
            // var asd = Activator.CreateInstance(typeof(MethodSurrogateInfo), new object[] { 12345 });
            var member = typeof(Foo).GetMethod(nameof(Foo.ActualMethod));
            Expression<Func<Attribute>> f = () => Attribute.GetCustomAttribute(member, typeof(Foo));
            // Expression<Action> f = () => this.foobar();

            var retType = SurrogateBuilder.Build(typeof(Foo));
            var instance = (Foo)Activator.CreateInstance(retType);
            // var instance = new Foo();
            // retType.GetMethod("NewM").Invoke(instance, null);
            var num = 456;
            var retVal = instance.ActualMethod("foobar is real", ref num);
            // instance.foo();
        }

        void foobar()
        {

        }
    }

    public class Foo
    {
        [MethodSurrogate]
        public virtual int ActualMethod(string InputText, ref int InputNum)
        {
            Console.WriteLine("Actual Method");
            Console.WriteLine($"Received : {InputText}");
            InputNum = 123;
            return InputNum + 1;
        }
    }
}
