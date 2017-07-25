using System;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Threading;
using DataModel;
using DynamicObjectProxy;

namespace ConsoleApplication1
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
            //Thread.GetDomain().SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

            const string connStr = "Integrated Security=true;Data Source=.\\SQLEXPRESS;Initial Catalog=AdvWorks";
            using (IDbConnection conn = new SqlConnection(connStr))
            {
                IDbTransaction transaction = null;

                try
                {
                    conn.Open();
                    var transactionObj = conn.BeginTransaction();
                    transaction = ObjectProxyFactory
                        .Configure<IDbTransaction>(transactionObj) //init fluency building
                        .FilterMethods(trans => trans.Commit(), trans => trans.Rollback()) //strongly typed method names
                        .AddPostDecoration(AppConcerns.ExitLog) //add post aspect
                        .CreateProxy(); //finish fluent interface, returning the proxy object


                    var order = new Order();
                    order.CustomerID = 18759;
                    order.DueDate = DateTime.Now.AddDays(1);
                    order.AccountNumber = "10-4030-018759";

                    order.BillToAddressID = 14024;
                    order.ShipToAddressID = 14024;

                    order.SubTotal = 174.20;
                    order.TaxAmt = 10;
                    order.Command = new SqlCommand();
                    order.Command.Connection = (SqlConnection)conn;


                    /* Setting up parameters */
                    dynamic parameters = new ExpandoObject();
                    parameters.Transaction = transactionObj;
                    parameters.CurrentPrincipal = Thread.CurrentPrincipal;

                    var proxiedOrder = ObjectProxyFactory
                        .Configure<IOrder>(order)
                        .FilterMethods(o => o.InsertOrder())
                        .AddPreDecoration(AppConcerns.JoinSqlTransaction)
                        .AddPreDecoration(AppConcerns.EnterLog)
                        .AddPostDecoration(AppConcerns.ExitLog)
                        .SetParameters((ExpandoObject)parameters)
                        .AddPreDecoration(AppConcerns.ThrowException)
                        .SetCallBack(ex => ex.Exceptions.ForEach(Console.WriteLine))
                        .CreateProxy();

                    var result = proxiedOrder.InsertOrder();

                    //throw new Exception();

                    var orderDetail = new OrderDetail();

                    orderDetail.SalesOrderID = order.OrderID;
                    orderDetail.OrderQty = 5;
                    orderDetail.ProductID = 708;
                    orderDetail.SpecialOfferID = 1;
                    orderDetail.UnitPrice = 28.84;
                    orderDetail.Command = new SqlCommand();
                    orderDetail.Command.Connection = (SqlConnection)conn;

                    var proxiedOrderDetail = ObjectProxyFactory
                        .Configure<IOrderDetail>(orderDetail)
                        .FilterMethods(oDet => oDet.InsertOrderDetail())
                        .AddPreDecoration(AppConcerns.JoinSqlTransaction)
                        .AddPreDecoration(AppConcerns.EnterLog)
                        .AddPostDecoration(AppConcerns.ExitLog)
                        .SetParameters(parameters)
                        .CreateProxy();

                    proxiedOrderDetail.InsertOrderDetail();

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
