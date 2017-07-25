using System;
using System.Linq;
using System.Security.AccessControl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicObjectProxy.Tests
{
    [TestClass]
    public class UnitTestInterception
    {


        [TestMethod]
        public void ShouldNotMutateArgs()
        {
            var argMethods = ObjectProxyHelper
                .GetMethodNamesFromLambdaProperty<IArgMethods>(true, false)
                .ToList();

            var proxy = ObjectProxyFactory.Configure<IArgMethods>(new TargetClass(), supressErrors: false)
                .FilterMethods(argMethods.ToArray())
                .AddPostDecoration(ctx => { /* no activity */ })
                .CreateProxy();

            var value = proxy.PublicMethod(1);

            Assert.AreEqual(value, 1);
        }


        [TestMethod]
        public void ShouldMutateArgs()
        {
            var argMethods = ObjectProxyHelper
                .GetMethodNamesFromLambdaProperty<IArgMethods>(true, false)
                .ToList();

            var proxy = ObjectProxyFactory.Configure<IArgMethods>(new TargetClass(), supressErrors: false)
                .FilterMethods(argMethods.ToArray())
                .AddPreDecoration(ctx => ctx.TargetArgs = new Object[] { 99 })
                .CreateProxy();

            int value = proxy.PublicMethod(1);
            Assert.AreEqual(99, value);

        }

        [TestMethod]
        public void ShouldMutateMethodReturnValue()
        {
            var argMethods = ObjectProxyHelper
                .GetMethodNamesFromLambdaProperty<IArgMethods>(true, false)
                .ToList();

            var proxy = ObjectProxyFactory.Configure<IArgMethods>(new TargetClass(), supressErrors: false)
                .FilterMethods(argMethods.ToArray())
                .AddPostDecoration(ctx => ctx.TargetReturnValue = 100)
                .CreateProxy();

            int value = proxy.PublicMethod(1);
            Assert.AreEqual(100, value);
        }

        [TestMethod]
        public void ShouldMutateGetterReturnValue()
        {
            var argMethods = ObjectProxyHelper
                .GetMethodNamesFromLambdaProperty<IPublicMethod>(true, false)
                .ToList();

            var proxy = ObjectProxyFactory.Configure<IPublicMethod>(new TargetClass(), supressErrors: false)
                .FilterMethods(argMethods.ToArray())
                .AddPostDecoration(ctx => ctx.TargetReturnValue = 100)
                .CreateProxy();

            int value = proxy.Counter;
            Assert.AreEqual(100, value);
        }

    }
}
