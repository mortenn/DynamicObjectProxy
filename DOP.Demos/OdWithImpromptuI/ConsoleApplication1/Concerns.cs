using System;
using System.Runtime.Remoting.Messaging;
using DynamicObjectProxy;

namespace ConsoleApplication1
{
    class AppConcerns
    {
        public static void ThrowException(AspectContext ctx)
        {
            throw new Exception("Expected exception!!!");
        }

        public static void JoinSqlTransaction(AspectContext ctx)
        {
            try
            {
                ctx.Target.Command.Transaction = ctx.Parameters.Transaction;
                return;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to join transaction!", ex);
            }
        }

        public static void EnterLog(AspectContext ctx)
        {
            IMethodCallMessage method = ctx.CallCtx;
            string str = "Entering " + ((object)ctx.Target).GetType().ToString() + "." + method.MethodName +
                "(";
            int i = 0;
            foreach (object o in method.Args)
            {
                if (i > 0)
                    str = str + ", ";
                str = str + o.ToString();
            }
            str = str + ")";

            Console.WriteLine(str);
            Console.Out.Flush();

        }

        public static void ExitLog(AspectContext ctx)
        {
            IMethodCallMessage method = ctx.CallCtx;
            string str = ((object)ctx.Target).GetType().ToString() + "." + method.MethodName +
                "(";
            int i = 0;
            foreach (object o in method.Args)
            {
                if (i > 0)
                    str = str + ", ";
                str = str + o.ToString();
            }
            str = str + ") exited";

            Console.WriteLine(str);
            Console.Out.Flush();
        }

        public static void SecurityCheck(AspectContext ctx)
        {
            //if (ctx.Parameters.CurrentPrincipal.IsInRole("BUILTIN\\" + "Administrators"))
            //    return;

            //throw new Exception("No right to call!");
        }

    }
}
