using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using MySql.Data.MySqlClient;
using System.Data;

namespace danhmucVM_windows_service
{
    class ketnoi
    {
        #region khoitao
        private ketnoi()
        {
            string connstring = string.Format("Server=27.72.29.28;port=3306; database=cnf; User Id=kho; password=1234");
            // string connstring = string.Format("Server=localhost;port=3306; database=cnf; User Id=hts; password=1211");
            connection = new MySqlConnection(connstring);
        }

        private MySqlConnection connection = null;

        private static ketnoi _instance = null;
        public static ketnoi Instance()
        {
            if (_instance == null)
                _instance = new ketnoi();
            return _instance;
        }
        public void Open()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public void Close()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }
        #endregion

        #region thao tac tren csdl mysql
        string ngaychen = DateTime.Now.ToString("dd/MM/yyyy");
        //kiem tra xem ma hang day co trong bang mota chua
        public string Kiemtra(string mahang)
        {
            string sql = @"SELECT matong1 FROM mota WHERE matong1='" + mahang + "'";
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            string hh = null;
            Open();
            MySqlDataReader dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                hh = dtr["matong1"].ToString();
            }
            Close();
            return hh;
        }
        public string Kiemtra(string cotcankiem, string tenbangkiem, string giatricantim)
        {
            string sql = string.Format("select {0} from {1} where {0}='{2}'", cotcankiem, tenbangkiem, giatricantim);
            string giatri = null;
            Open();
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader dtr = cmd.ExecuteReader();

            while (dtr.Read())
            {
                giatri = dtr[cotcankiem].ToString();
            }
            Close();
            return giatri;
        }
        public string Kiemtra(string laygiatri, string tubang, string noigiatri, string bang)
        {
            string sql = string.Format("select {0} from {1} where {2}='{3}'", laygiatri, tubang, noigiatri, bang);
            string giatri = null;
            Open();
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            MySqlDataReader dtr = cmd.ExecuteReader();

            while (dtr.Read())
            {
                giatri = dtr[laygiatri].ToString();
            }
            Close();
            return giatri;
        }

        // lay masp tu barcode
        //public string laymasp(string barcode)
        //{
        //    string sql = string.Format("SELECT masp FROM data WHERE barcode='{0}'", barcode);
        //    string h = null;
        //    MySqlCommand cmd = new MySqlCommand(sql, connection);
        //    Open();
        //    MySqlDataReader dtr = cmd.ExecuteReader();
        //    while (dtr.Read())
        //    {
        //        h = dtr["masp"].ToString();
        //    }
        //    Close();
        //    int vitri = h.IndexOf("-");
        //    h = h.Substring(0, vitri);
        //    return h;
        //}

        public void Chenvaobanghangduocban(string maduocban, string ngayduocban, string ghichu, string ngaydangso, string mota, string chude)
        {
            //string sqlchen = @"INSERT INTO hangduocban(matong,ngayban,ghichu,ngaydangso,mota,chude) VALUES('"+maduocban+"','"+ngayduocban+"','"+ghichu+"','"+ngaydangso+",'"+mota+"','"+chude+"')";
            string sqlchen = "insert into hangduocban(matong,ngayban,ghichu,ngaydangso,mota,chude) VALUES(@1,@2,@3,@4,@5,@6)";
            Open();
            //MySqlCommand cmd = new MySqlCommand(sqlchen, connec);
            MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = sqlchen;
            cmd.Parameters.AddWithValue("@1", maduocban);
            cmd.Parameters.AddWithValue("@2", ngayduocban);
            cmd.Parameters.AddWithValue("@3", ghichu);
            cmd.Parameters.AddWithValue("@4", ngaydangso);
            cmd.Parameters.AddWithValue("@5", mota);
            cmd.Parameters.AddWithValue("@6", chude);
            cmd.ExecuteNonQuery();
            Close();
        }
        public void chenvaoFiledanhmuc(string tenFile)
        {
            string sql = "update filedanhmucmoi set tenfile = '" + tenFile + "' , gio = '"+DateTime.Now.ToString()+"'";
            Open();
            MySqlCommand cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
            Close();
        }
        #endregion

    }
}
