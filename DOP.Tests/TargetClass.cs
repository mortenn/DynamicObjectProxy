// Criado por Jone Polvora
// 01 06 2012

namespace DynamicObjectProxy.Tests
{
    public interface IPrivateMethod
    {
        void PrivateMethod();
    }

    public interface IProtectedMethod
    {
        void ProtectedMethod();
    }

    public interface IArgMethods
    {
        int PublicMethod(int increment);
    }

    public interface IPublicMethod : IArgMethods
    {
        int Counter { get; }
    }

    public interface IAllMethods : IPublicMethod, IProtectedMethod, IPrivateMethod, IArgMethods
    {
    }

    public class TargetClass : IPublicMethod
    {
        public TargetClass()
        {
            PrivateMethod();
            ProtectedMethod();

        }

        public int Counter { get; private set; }

        void PrivateMethod()
        {
            Counter++;
        }

        protected void ProtectedMethod()
        {
            Counter--;
        }

        public int PublicMethod(int increment)
        {
            return (Counter = Counter + increment);
        }
    }
}