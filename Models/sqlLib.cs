using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Upload.Models
{
    public class sqlLib
    {
        # region Variables
        private SqlConnection Cn = null;
        private SqlDataAdapter Da = null;
        private SqlCommand Cmd = null;
        private string cnstr = "", _procedure = "";

        private proLib plib = new proLib();
        # endregion

        #region Constructor
        public sqlLib()
        {
        }
        #endregion

        # region Properties
        public string ConnectionString
        {
            get
            {
                try { if (cnstr == "") { cnstr = ConfigurationManager.ConnectionStrings["connectionstring"].ToString(); } }
                catch { }
                return cnstr;
            }
            //set
            //{
            //    try { cnstr = String.Format("Server={0}; Uid={1}; Pwd={2}; Database={3}; Connection TimeOut=600; Pooling=true; Min Pool Size=1;", value); }
            //    catch { cnstr = ""; }
            //}
        }

        public string ProcedureName
        {
            get { return _procedure; }
            set
            {
                _procedure = value;

                Cn = new SqlConnection(ConnectionString);
                Da = new SqlDataAdapter(_procedure, Cn);
                Cmd = Da.SelectCommand;
                Cmd.CommandType = CommandType.StoredProcedure;
                Cmd.CommandTimeout = 0; // ramya added on 12 sep 22 to avoid timeout exception  
                Cmd.CommandTimeout = 3000;
            }
        }

        public DataSet ExecuteProcedure
        {
            get
            {
                DataSet Ds = new DataSet();
                try
                {
                    if (Cn.State == ConnectionState.Closed) Cn.Open();
                    Da.Fill(Ds); return Ds;
                }
                catch (Exception ex)
                {
                    //plib.CreateLog("SqlLib", ProcedureName, ex.Message);

                    string s = ex.Message;
                    if (s.IndexOf("'") > -1) s = s.Replace("'", " ");
                    s = s.Replace("\n", " "); s = s.Replace("\r", " ");

                    DataTable dt = new DataTable();
                    dt.Columns.Add("Message"); dt.Columns.Add("Reset");
                    DataRow dr = dt.NewRow();
                    dr[0] = s; dr[1] = "0";
                    dt.Rows.Add(dr); Ds.Tables.Add(dt);
                    return Ds;
                }
                finally
                {
                    Cn.Close(); Da.Dispose(); Cmd.Dispose(); Ds.Dispose(); _procedure = "";
                }
            }
        }
        #endregion

        #region Methods
        public void AddParameter(string Name, string Value)
        {
            if (Name.IndexOf("@") == -1)
            {
                Name = "@" + Name;
            }

            Cmd.Parameters.AddWithValue(Name, Value);
        }

        public void AddParameter(string Name, DataTable Value)
        {
            if (Name.IndexOf("@") == -1)
            {
                Name = "@" + Name;
            }
            Cmd.Parameters.AddWithValue(Name, Value);
        }

        public DataTable Execute(string Query)
        {
            if (string.IsNullOrEmpty(Query)) return null;

            DataTable dt = new DataTable();
            try
            {
                Cn = new SqlConnection(ConnectionString);
                Da = new SqlDataAdapter(Query, Cn);
                Cmd = Da.SelectCommand;
                Cmd.CommandType = CommandType.Text;
                Cmd.CommandTimeout = 3000;

                if (Cn.State == ConnectionState.Closed) Cn.Open();
                Da.Fill(dt); return dt;
            }
            catch (Exception)
            {
                //plib.CreateLog("SqlLib", Query, ex.Message);
                return null;
            }
            finally
            {
                Cn.Close(); Da.Dispose(); Cmd.Dispose(); dt.Dispose();
            }
        }
        #endregion
    }
}