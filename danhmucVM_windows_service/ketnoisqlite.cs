﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Data.SQLite;
using System.Data;
using System.IO;
namespace danhmucVM_windows_service
{
    class ketnoisqlite
    {
        #region khoitao
        public static SQLiteConnection connec = null;
        public ketnoisqlite()
        {
            string chuoiketnoi = "Data Source=dbhangmoi.db;version=3;new=false";
            connec = new SQLiteConnection(chuoiketnoi);
        }

        private static ketnoisqlite _khoitao = null;
        public static ketnoisqlite khoitao()
        {
            if (_khoitao == null)
            {
                _khoitao = new ketnoisqlite();
            }
            return _khoitao;
        }

        public void Open()
        {
            if (connec.State != ConnectionState.Open)
            {
                connec.Open();
            }
        }
        public void Close()
        {
            if (connec.State != ConnectionState.Closed)
            {
                connec.Close();
            }
        }
        #endregion
        
        string ngaychen = DateTime.Now.ToString("dd/MM/yyyy");
        public string Kiemtrafile(string tenfile)
        {
            string sql = string.Format("select name from filedanhmuc where name='{0}'", tenfile);
            string giatri = null;
            Open();
            SQLiteCommand cmd = new SQLiteCommand(sql, connec);
            SQLiteDataReader dtr = cmd.ExecuteReader();

            while (dtr.Read())
            {
                giatri = dtr["name"].ToString();
            }
            Close();
            return giatri;
        }
        public void Chenvaobangfiledanhmuc(string tenfile)
        {
            string sqlchen = string.Format(@"INSERT INTO filedanhmuc VALUES('{0}','{1}','Not')", tenfile, ngaychen);
            Open();
            SQLiteCommand cmd = new SQLiteCommand(sqlchen, connec);
            cmd.ExecuteNonQuery();
            Close();
        }
        public List<string> layfilechuaxuly()
        {
            List<string> ds = new List<string>();
            string sql = "select name from filedanhmuc where tinhtrang='Not'";
            Open();
            SQLiteCommand cmd = new SQLiteCommand(sql, connec);
            SQLiteDataReader dtr = cmd.ExecuteReader();
            while (dtr.Read())
            {
                ds.Add(dtr["name"].ToString());
            }
            Close();
            return ds;
        }
        public void thaydoitrangthaidakiemtra(string tenfile)
        {
            string sql = string.Format("UPDATE filedanhmuc SET tinhtrang='{0}' WHERE name='{1}'", "OK", tenfile);
            SQLiteCommand cmd = new SQLiteCommand(sql, connec);
            Open();
            cmd.ExecuteNonQuery();
            Close();
        }
        public string Kiemtra(string cotcankiem, string tenbangkiem, string giatricantim)
        {
            string sql = string.Format("select {0} from {1} where {0}='{2}'", cotcankiem, tenbangkiem, giatricantim);
            string giatri = null;
            Open();
            SQLiteCommand cmd = new SQLiteCommand(sql, connec);
            SQLiteDataReader dtr = cmd.ExecuteReader();

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
            SQLiteCommand cmd = new SQLiteCommand(sql, connec);
            SQLiteDataReader dtr = cmd.ExecuteReader();

            while (dtr.Read())
            {
                giatri = dtr[laygiatri].ToString();
            }
            Close();
            return giatri;
        }
    }
}