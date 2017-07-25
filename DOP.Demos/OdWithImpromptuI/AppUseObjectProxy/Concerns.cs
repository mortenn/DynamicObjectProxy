using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CBOExtender;
using System.Runtime.Remoting.Messaging;


namespace Concerns
{
    class AppConcerns
    {
        public static void JoinSqlTransaction(AspectContext2 ctx, dynamic parameter)
        {
            try
            {
                ctx.Target.Command.Transaction = parameter;
                return;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to join transaction!", ex);
            }
        }

        public static void EnterLog(AspectContext2 ctx, dynamic parameters)
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

        public static void ExitLog(AspectContext2 ctx, dynamic parameters)
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

        public static void SecurityCheck(AspectContext2 ctx, dynamic parameter)
        {
            if (parameter.IsInRole("BUILTIN\\" + "Administrators"))
                return;

            throw new Exception("No right to call!");
        }

    }
}
