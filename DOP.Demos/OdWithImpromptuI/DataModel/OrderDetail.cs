using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;

namespace DataModel
{
    public class OrderDetail
    {
        public int SalesOrderID { get; set; }
        public int OrderQty { get; set; }
        public int ProductID { get; set; }
        public int SpecialOfferID { get; set; }
        public double UnitPrice { get; set; }

        private SqlCommand commd;
        public SqlCommand Command
        {
            get { return commd; }
            set { commd = value; }
        }

        public int InsertOrderDetail()
        {
            string sqlStr = @"INSERT INTO [Sales].[SalesOrderDetail] 
([SalesOrderID], [OrderQty], [ProductID], [SpecialOfferID], [UnitPrice]) values
(@orderID, @OrderQty, @ProductID, @SpecialOfferID, @UnitPrice)";

            commd.CommandText = sqlStr;
            commd.CommandType = CommandType.Text;

            SqlParameter orderIDParameter = new SqlParameter("@orderID", SqlDbType.Int);
            orderIDParameter.Direction = ParameterDirection.Input;
            orderIDParameter.Value = SalesOrderID;
            commd.Parameters.Add(orderIDParameter);

            SqlParameter OrderQtyParameter = new SqlParameter("@OrderQty", SqlDbType.Int);
            OrderQtyParameter.Direction = ParameterDirection.Input;
            OrderQtyParameter.Value = OrderQty;
            commd.Parameters.Add(OrderQtyParameter);

            SqlParameter ProductIDParameter = new SqlParameter("@ProductID", SqlDbType.Int);
            ProductIDParameter.Direction = ParameterDirection.Input;
            ProductIDParameter.Value = ProductID;
            commd.Parameters.Add(ProductIDParameter);

            SqlParameter SpecialOfferIDParameter = new SqlParameter("@SpecialOfferID", SqlDbType.Int);
            SpecialOfferIDParameter.Direction = ParameterDirection.Input;
            SpecialOfferIDParameter.Value = SpecialOfferID;
            commd.Parameters.Add(SpecialOfferIDParameter);

            SqlParameter UnitPriceParameter = new SqlParameter("@UnitPrice", SqlDbType.Float);
            UnitPriceParameter.Direction = ParameterDirection.Input;
            UnitPriceParameter.Value = UnitPrice;
            commd.Parameters.Add(UnitPriceParameter);

            return commd.ExecuteNonQuery();
        }
    }
}