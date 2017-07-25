using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicObjectProxy.Tests
{
    [TestClass]
    public class UnitTestWithNoRegistration
    {
        [TestMethod]
        public void ShoulInterceptOnlyPrivateMethod()
        {
            var privateMethodName =
                ObjectProxyHelper.GetMethodNames<IPrivateMethod>(i => i.PrivateMethod())[0];

            var proxy = ObjectProxyFactory
                .Configure<IAllMethods>(new TargetClass()) //initialize fluent config, with given interface and instance
                .FilterMethods(privateMethodName) //only intercept methods with the given name
                .AddPreDecoration(ctx => Assert.IsTrue(ctx.CallCtx.MethodName == privateMethodName))
                .AddPostDecoration(ctx => Assert.IsTrue(ctx.CallCtx.MethodName == privateMethodName))
                .SetParameters(new object())
                .CreateProxy(); //finally create and return the proxy


            proxy.PrivateMethod(); // will assert is true
            proxy.ProtectedMethod(); //will not be intercepted
            proxy.PublicMethod(1); //will not be intercepted
        }

        [TestMethod]
        public void ShoulNotInterceptOnlyPrivateMethod()
        {
            var methodNames = ObjectProxyHelper
                .GetMethodNames<IAllMethods>(i => i.ProtectedMethod(), i => i.PublicMethod(0)).ToList();

            var proxy = ObjectProxyFactory.Configure<IAllMethods>(new TargetClass())
                .FilterMethods(methodNames.ToArray())
                .AddPostDecoration(ctx => Assert.IsTrue(methodNames.Contains(ctx.CallCtx.MethodName)))
                .CreateProxy();

            proxy.PrivateMethod();
            proxy.ProtectedMethod();
            proxy.PublicMethod(1);
        }

        [TestMethod]
        public void ShouldInterceptGetters()
        {
            var getters = ObjectProxyHelper
                .GetMethodNamesFromLambdaProperty<IAllMethods>(true, false, t => t.Counter)
                .ToList();

            var proxy = ObjectProxyFactory.Configure<IAllMethods>(new TargetClass())
                .FilterMethods(getters.ToArray())
                .AddPostDecoration(ctx => Assert.IsTrue(getters.Contains(ctx.CallCtx.MethodName)))
                .CreateProxy();

            var c = proxy.Counter; //will access the get_Counter method
            proxy.PrivateMethod();
            proxy.ProtectedMethod();
            proxy.PublicMethod(1);
        }

        [TestMethod]
        public void ProxyExtension()
        {
            var @object = new TargetClass().GetProxy<IAllMethods>();
            @object.PrivateMethod(); //set Counter++ (1)
            var y = @object.Counter;
            Assert.IsTrue(y == 1);
        }

        [TestMethod]
        public void ExpectedExceptionTest()
        {

            var proxy = ObjectProxyFactory
            	.Configure<IAllMethods>(new TargetClass())
             .FilterMethods(t => t.PrivateMethod())
             .AddPostDecoration(ThrowExceptionAspect)
             .SetCallBack(CheckExceptionCallback)
             .CreateProxy();

            //test exception
            proxy.PrivateMethod();

        }

        void ThrowExceptionAspect(AspectContext<IAllMethods> ctx)
        {
            throw new System.Exception();
        }

        void CheckExceptionCallback(AspectException exception)
        {
            Assert.IsNotNull(exception);
            Assert.IsTrue(exception.Exceptions.Count == 1);
        }

    }
}