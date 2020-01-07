using System;
using System.Linq.Expressions;
using Surrogate.Base;
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
            // retType.GetMethod("NewM").Invoke(instance, null);
            var retVal = instance.ActualMethod(456);
            // instance.foo();
        }

        void foobar()
        {

        }
    }

    public class Foo
    {
        [MethodSurrogate]
        public virtual int ActualMethod(int input = 0)
        {
            Console.WriteLine("Actual Method");
            return input + 1;
        }
    }
}
