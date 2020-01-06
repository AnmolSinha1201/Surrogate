using System;
using System.Linq.Expressions;
using Surrogate.Base;

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
            instance.ActualMethod();
        }

        void foobar()
        {

        }
    }

    public class Foo
    {
        [MethodSurrogate]
        public virtual void ActualMethod()
        {
            Console.WriteLine("Actual Method");
        }

        public void CallMethod1(string Name, object[] Items)
        {
            
        }

        public void CallMethod2(string Name)
        {

        }

        public void CallMethod3(object[] Items)
        {

        }
    }
}
