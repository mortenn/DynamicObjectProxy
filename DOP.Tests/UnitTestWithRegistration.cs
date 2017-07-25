using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicObjectProxy.Tests
{
    [TestClass]
    public class UnitTestWithRegistration
    {
        [TestInitialize]
        public void Init()
        {
            var getters = ObjectProxyHelper
              .GetMethodNamesFromLambdaProperty<IAllMethods>(true, false, t => t.Counter)
              .ToList();

            ObjectProxyFactory.Configure<IAllMethods>(new TargetClass())
                .FilterMethods(getters.ToArray())
                .AddPostDecoration(ctx => Assert.IsTrue(getters.Contains(ctx.CallCtx.MethodName)))
                .Save();
        }

        [TestCleanup]
        public void CleanUp()
        {
            ObjectProxyFactory.CleanUp();
        }



        [TestMethod]
        public void TestConfigBuilder()
        {
            var proxy = ObjectProxyFactory.CreateProxy<IAllMethods>();
            var x = proxy.Counter;
        }
    }
}
