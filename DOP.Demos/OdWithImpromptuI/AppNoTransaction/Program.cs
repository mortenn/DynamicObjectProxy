using System;

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

using DataModel;

namespace AppNoTransaction
{
    class Program
    {
        static void Main(string[] args)
        {
            string connStr = "Integrated Security=true;Data Source=(local);Initial Catalog=AdventureWorks";
            using(IDbConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();

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

                    int iStatus;
                    iStatus = o.InsertOrder();

                    var od = new OrderDetail();
                    od.SalesOrderID = o.OrderID;
                    od.OrderQty = 5;
                    od.ProductID = 708;
                    od.SpecialOfferID = 1;
                    od.UnitPrice = 28.84;
                    od.Command = new SqlCommand();
                    od.Command.Connection = (SqlConnection)conn;

                    iStatus = od.InsertOrderDetail();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
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
