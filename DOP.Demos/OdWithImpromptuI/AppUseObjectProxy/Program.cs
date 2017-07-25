using System;
using CBOExtender;

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using DataModel;
using Concerns;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Security.Principal;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;  


namespace AppUseObjectProxy
{
    public interface IOrder
    {
        int OrderID { get; set; }
        int CustomerID { get; set; }
        DateTime DueDate { get; set; }
        string AccountNumber { get; set; }
        int ContactID { get; set; }
        int BillToAddressID { get; set; }
        int ShipToAddressID { get; set; }
        int ShipMethodID { get; set; }
        double SubTotal { get; set; }
        double TaxAmt { get; set; }
        SqlCommand Command { get; set; }

        int InsertOrder();
    }

    public interface IOrderDetail
    {
        int SalesOrderID { get; set; }
        int OrderQty { get; set; }
        int ProductID { get; set; }
        int SpecialOfferID { get; set; }
        double UnitPrice { get; set; }
        SqlCommand Command { get; set; }

        int InsertOrderDetail();
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Commenting out this line, the security check aspect will throw out an exception 
            Thread.GetDomain().SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

            string connStr = "Integrated Security=true;Data Source=(local);Initial Catalog=AdventureWorks";
            using (IDbConnection conn = new SqlConnection(connStr))
            {
                IDbTransaction transaction = null;

                try
                {
                    conn.Open();
                    IDbTransaction transactionObj = conn.BeginTransaction();
                    transaction = ObjectProxyFactory.CreateProxy2<IDbTransaction>(
                        transactionObj,
                        new string[] { "Commit", "Rollback" },
                        null,
                        new Decoration2(AppConcerns.ExitLog, null)
                    );

                    var o = new Order();
                    o.CustomerID = 18759;
                    o.DueDate = DateTime.Now.AddDays(1);
                    o.AccountNumber = "10-4030-018759";
                    o.ContactID = 4189;
                    o.BillToAddressID = 14024;
                    o.ShipToAddressID = 14024;
                    o.ShipMethodID = 1;
                    o.SubTotal = 174.20;
                    o.TaxAmt = 10;
                    o.Command = new SqlCommand();
                    o.Command.Connection = (SqlConnection)conn;

                    var iOrder = o.ActLike<IOrder>();

                    iOrder = ObjectProxyFactory.CreateProxy2<IOrder>(
                        iOrder,
                        new string[] { "InsertOrder" },
                        new Decoration2(AppConcerns.JoinSqlTransaction, transactionObj),
                        null
                    );

                    iOrder = ObjectProxyFactory.CreateProxy2<IOrder>(
                        iOrder,
                        new string[] { "InsertOrder" },
                        new Decoration2(AppConcerns.EnterLog, null),
                        new Decoration2(AppConcerns.ExitLog, null)
                    );

                    iOrder = ObjectProxyFactory.CreateProxy2<IOrder>(
                        iOrder,
                        new string[] { "InsertOrder" },
                        new Decoration2(AppConcerns.SecurityCheck, Thread.CurrentPrincipal),
                        null
                    );

                    int iStatus;
                    iStatus = iOrder.InsertOrder();

                    //throw new Exception();

                    var od = new OrderDetail();

                    od.SalesOrderID = o.OrderID;
                    od.OrderQty = 5;
                    od.ProductID = 708;
                    od.SpecialOfferID = 1;
                    od.UnitPrice = 28.84;
                    od.Command = new SqlCommand();
                    od.Command.Connection = (SqlConnection)conn;

                    var iOrderDetail = od.ActLike<IOrderDetail>();

                    iOrderDetail = ObjectProxyFactory.CreateProxy2<IOrderDetail>(
                        iOrderDetail,
                        new string[] { "InsertOrderDetail" },
                        new Decoration2(AppConcerns.JoinSqlTransaction, transactionObj),
                        null
                    );

                    iOrderDetail = ObjectProxyFactory.CreateProxy2<IOrderDetail>(
                        iOrderDetail,
                        new string[] { "InsertOrderDetail" },
                        new Decoration2(AppConcerns.EnterLog, null),
                        new Decoration2(AppConcerns.ExitLog, null)
                    );

                    iOrderDetail = ObjectProxyFactory.CreateProxy2<IOrderDetail>(
                        iOrderDetail,
                        new string[] { "InsertOrderDetail" },
                        new Decoration2(AppConcerns.SecurityCheck, Thread.CurrentPrincipal),
                        null
                    );

                    iStatus = iOrderDetail.InsertOrderDetail();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    if (transaction != null)
                        transaction.Rollback();
                }
                finally
                {
                    conn.Close();
                }

                Console.ReadLine();
            }
        }
    }
}
