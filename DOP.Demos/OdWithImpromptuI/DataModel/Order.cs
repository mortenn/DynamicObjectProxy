using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;

namespace DataModel
{
    public class Order
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateTime DueDate { get; set; }
        public string AccountNumber { get; set; }
        public int ContactID { get; set; }
        public int BillToAddressID { get; set; }
        public int ShipToAddressID { get; set; }
        public int ShipMethodID { get; set; }
        public double SubTotal { get; set; }
        public double TaxAmt { get; set; }

        private SqlCommand commd;
        public SqlCommand Command
        {
            get { return commd; }
            set { commd = value; }
        }

        public int InsertOrder()
        {
            string sqlStr = @"INSERT [Sales].[SalesOrderHeader] 
([CustomerID], [DueDate], [AccountNumber], [ContactID], [BillToAddressID], 
[ShipToAddressID], [ShipMethodID], [SubTotal], [TaxAmt]) values
(@CustomerID, @DueDate, @AccountNumber, @ContactID, @BillToAddressID,
@ShipToAddressID, @ShipMethodID, @SubTotal, @TaxAmt); SET @scopeId = SCOPE_IDENTITY()";

            commd.CommandText = sqlStr;
            commd.CommandType = CommandType.Text;

            SqlParameter CustomerIDParameter = new SqlParameter("@CustomerID", SqlDbType.Int);
            CustomerIDParameter.Direction = ParameterDirection.Input;
            CustomerIDParameter.Value = CustomerID;
            commd.Parameters.Add(CustomerIDParameter);

            SqlParameter DueDateParameter = new SqlParameter("@DueDate", SqlDbType.DateTime);
            DueDateParameter.Direction = ParameterDirection.Input;
            DueDateParameter.Value = DueDate;
            commd.Parameters.Add(DueDateParameter);

            SqlParameter AccountNumberParameter = new SqlParameter("@AccountNumber", SqlDbType.Text);
            AccountNumberParameter.Direction = ParameterDirection.Input;
            AccountNumberParameter.Value = AccountNumber;
            commd.Parameters.Add(AccountNumberParameter);

            SqlParameter ContactIDParameter = new SqlParameter("@ContactID", SqlDbType.Int);
            ContactIDParameter.Direction = ParameterDirection.Input;
            ContactIDParameter.Value = ContactID;
            commd.Parameters.Add(ContactIDParameter);

            SqlParameter BillToAddressIDParameter = new SqlParameter("@BillToAddressID", SqlDbType.Int);
            BillToAddressIDParameter.Direction = ParameterDirection.Input;
            BillToAddressIDParameter.Value = BillToAddressID;
            commd.Parameters.Add(BillToAddressIDParameter);

            SqlParameter ShipToAddressIDParameter = new SqlParameter("@ShipToAddressID", SqlDbType.Int);
            ShipToAddressIDParameter.Direction = ParameterDirection.Input;
            ShipToAddressIDParameter.Value = ShipToAddressID;
            commd.Parameters.Add(ShipToAddressIDParameter);

            SqlParameter ShipMethodIDParameter = new SqlParameter("@ShipMethodID", SqlDbType.Int);
            ShipMethodIDParameter.Direction = ParameterDirection.Input;
            ShipMethodIDParameter.Value = ShipMethodID;
            commd.Parameters.Add(ShipMethodIDParameter);

            SqlParameter SubTotalParameter = new SqlParameter("@SubTotal", SqlDbType.Float);
            SubTotalParameter.Direction = ParameterDirection.Input;
            SubTotalParameter.Value = SubTotal;
            commd.Parameters.Add(SubTotalParameter);

            SqlParameter TaxAmtParameter = new SqlParameter("@TaxAmt", SqlDbType.Int);
            TaxAmtParameter.Direction = ParameterDirection.Input;
            TaxAmtParameter.Value = TaxAmt;
            commd.Parameters.Add(TaxAmtParameter);

            SqlParameter scopeIDParameter = new SqlParameter("@scopeId", SqlDbType.Int);
            scopeIDParameter.Direction = ParameterDirection.Output;
            commd.Parameters.Add(scopeIDParameter);

            int i = commd.ExecuteNonQuery();

            OrderID = (int)scopeIDParameter.Value;

            return i;
        }
    }
}