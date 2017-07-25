// Criado por Jone Polvora
// 17 05 2012

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicObjectProxy.Incubatory
{
    public static class DynamicProxyGenerator
    {
        public static T GetInstanceFor<T>()
        {
            var typeOfT = typeof(T);
            var methodInfos = typeOfT.GetMethods();
            var assName = new AssemblyName("testAssembly");
            var assBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assBuilder.DefineDynamicModule("testModule", "test.dll");
            var typeBuilder = moduleBuilder.DefineType(typeOfT.Name + "Proxy", TypeAttributes.Public);
            //typeBuilder.
            typeBuilder.AddInterfaceImplementation(typeOfT);
            var ctorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[] { });
            var ilGenerator = ctorBuilder.GetILGenerator();
            ilGenerator.EmitWriteLine("Creating Proxy instance");
            ilGenerator.Emit(OpCodes.Ret);

            foreach (var methodInfo in methodInfos)
            {
                var methodBuilder = typeBuilder.DefineMethod(
                    methodInfo.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    methodInfo.ReturnType,
                    methodInfo.GetParameters().Select(p => p.GetType()).ToArray()
                    );
                var methodILGen = methodBuilder.GetILGenerator();
                if (methodInfo.ReturnType == typeof(void))
                {
                    methodILGen.Emit(OpCodes.Ret);
                }
                else
                {
                    if (methodInfo.ReturnType.IsValueType || methodInfo.ReturnType.IsEnum)
                    {
                        MethodInfo getMethod = typeof(Activator).GetMethod("CreateInstance", new Type[] { typeof(Type) });
                        LocalBuilder lb = methodILGen.DeclareLocal(methodInfo.ReturnType);
                        methodILGen.Emit(OpCodes.Ldtoken, lb.LocalType);
                        methodILGen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
                        methodILGen.Emit(OpCodes.Callvirt, getMethod);
                        methodILGen.Emit(OpCodes.Unbox_Any, lb.LocalType);

                    }
                    else
                    {
                        methodILGen.Emit(OpCodes.Ldnull);
                    }
                    methodILGen.Emit(OpCodes.Ret);
                }
                typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
            }

            Type constructedType = typeBuilder.CreateType();
            var instance = Activator.CreateInstance(constructedType);
            return (T)instance;
        }
    }
}