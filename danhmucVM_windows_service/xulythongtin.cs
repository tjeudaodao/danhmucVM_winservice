using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spire.Xls;
using excel = Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;

namespace danhmucVM_windows_service
{
    class xulythongtin
    {
        #region khoitao class
        public xulythongtin()
        {

        }
        private static xulythongtin _khoitao = null;
        public static xulythongtin Khoitao()
        {
            if (_khoitao == null)
            {
                _khoitao = new xulythongtin();
            }
            return _khoitao;
        }
        #endregion

        #region danhmuc
        string maungay = @"\d{2}/\d{2}/\d{4}";
        static List<laythongtin> luuthongtin = new List<laythongtin>();
        static List<string> danhsachfilechuaxuly = new List<string>();
        string duongdanapp = AppDomain.CurrentDomain.BaseDirectory;

        // ham chuyen doi dinh dang ngay tu string sang dang so co the + -
        public string chuyendoingayvedangso(string ngaydangDDMMYYYY)
        {
            try
            {
                DateTime dt = DateTime.ParseExact(ngaydangDDMMYYYY, "dd/MM/yyyy", null);
                return dt.ToString("yyyyMMdd");
            }
            catch (Exception)
            {

                return "Loi";
            }

        }

        public void luudanhmuchangmoi()
        {

            var con = ketnoisqlite.khoitao();
            string[] danhsachfile = Directory.GetFiles(duongdanapp + @"\filedanhmuc\");

            for (int i = 0; i < danhsachfile.Length; i++)
            {
                if (con.Kiemtrafile(danhsachfile[i]) == null)
                {

                    con.Chenvaobangfiledanhmuc(danhsachfile[i]);
                }

            }
        }
        public void xulyanh()
        {
            var con = ketnoisqlite.khoitao();
            danhsachfilechuaxuly = con.layfilechuaxuly();
            string mau = @"^KH tung hang";
            foreach (string file in danhsachfilechuaxuly)
            {
                try
                {
                    if (Path.GetExtension(file) == ".xlsx")
                    {
                        // Console.WriteLine(file);
                        ExcelPackage filechon = new ExcelPackage(new FileInfo(file));
                        ExcelWorksheet ws = filechon.Workbook.Worksheets[1];
                        var sodong = ws.Dimension.End.Row;

                        string ngayduocban = null;
                        string ngaydangso = null;
                        MatchCollection mat = Regex.Matches(Convert.ToString(ws.Cells[7, 1].Text) ?? "", maungay);
                        //Console.WriteLine(ws.Cells[7,1].value);
                        foreach (Match m in mat)
                        {
                            ngayduocban = m.Value.ToString();
                        }
                        string mahang, mota, bst, ghichu;
                        ngaydangso = chuyendoingayvedangso(ngayduocban);
                        for (int i = 10; i < sodong; i++)
                        {
                            if (ws.Cells[i, 5].Value == null)
                            {
                                continue;
                            }
                            mahang = ws.Cells[i, 5].Value.ToString();
                            mota = ws.Cells[i, 6].Value.ToString();
                            bst = Convert.ToString(ws.Cells[i, 10].Value);
                            ghichu = Convert.ToString(ws.Cells[i, 11].Value);
                            luuthongtin.Add(new laythongtin(ngayduocban, mahang, mota, bst, ghichu, ngaydangso));
                        }
                        filechon.Dispose();
                    }
                    else if (Path.GetExtension(file) == ".xls")
                    {
                        if (Regex.IsMatch(Path.GetFileName(file), mau))
                        {
                            copyanhKHtunghang(file);
                        }
                        else copyanhvathongtin(file);
                    }
                }
                catch (Exception)
                {

                    continue;
                }

            }

        }
        public void xulymahang()
        {
            var con = ketnoisqlite.khoitao();
            var conmysql = ketnoi.Instance();
            try
            {
                foreach (laythongtin mahang in luuthongtin)
                {
                    if (conmysql.Kiemtra("matong", "hangduocban", mahang.Maduocban) == null)
                    {
                        try
                        {
                            conmysql.Chenvaobanghangduocban(mahang.Maduocban, mahang.Ngayduocban, mahang.Ghichu, mahang.Ngaydangso, mahang.Motamaban, mahang.Chudemaban);
                        }
                        catch (Exception)
                        {

                            continue;
                        }
                    }


                }
                luuthongtin.Clear();
            }
            catch (Exception)
            {

                throw;
            }

            foreach (string file in danhsachfilechuaxuly)
            {
                con.thaydoitrangthaidakiemtra(file);
            }
            danhsachfilechuaxuly.Clear();
        }
        public void copyanhvathongtin(string filecanlay)
        {
            var excelApp = new excel.Application();
            var wb = excelApp.Workbooks.Open(filecanlay);
            var ws = (excel.Worksheet)wb.Worksheets[2];
            string duongdanluuanh = duongdanapp + @"\luuanh";
            int hangbatdau = 0;
            //lay ngay tu file excel roi chuyen doi sang dinh dang khac truoc khi insert vao database
            string ngayduocban = null;
            string ngaydangso = null;
            MatchCollection mat = Regex.Matches(Convert.ToString(ws.Cells[7, 1].value2) ?? "", maungay);
            foreach (Match m in mat)
            {
                ngayduocban = m.Value.ToString();
            }
            ngaydangso = chuyendoingayvedangso(ngayduocban);

            List<string> tenanh = new List<string>();
            string mahang, mota, bst, ghichu;
            foreach (var pic in ws.Pictures())
            {
                hangbatdau = pic.TopLeftCell.Row;

                tenanh.Add(ws.Cells[hangbatdau, 5].value);

            }
            int lastRow = ws.Cells[ws.Rows.Count, 5].End(excel.XlDirection.xlUp).Row;
            for (int i = 10; i < (lastRow + 2); i++)
            {
                if (ws.Cells[i, 5].value == null)
                {
                    continue;
                }
                mahang = ws.Cells[i, 5].value2.ToString();
                mota = ws.Cells[i, 6].value2.ToString();
                bst = Convert.ToString(ws.Cells[i, 10].value2);
                ghichu = Convert.ToString(ws.Cells[i, 11].value2);

                luuthongtin.Add(new laythongtin(ngayduocban, mahang, mota, bst, ghichu, ngaydangso));
            }

            string[] manganh = tenanh.ToArray();
            
            wb.Close();
            excelApp.Quit();
            Marshal.FinalReleaseComObject(excelApp);
            Marshal.FinalReleaseComObject(wb);


            Workbook workbook = new Workbook();
            workbook.LoadFromFile(filecanlay);

            Worksheet sheet = workbook.Worksheets[1];
            if (!Directory.Exists(duongdanluuanh))
            {
                Directory.CreateDirectory(duongdanluuanh);
            }

            for (int i = 1; i < manganh.Length; i++)
            {
                Spire.Xls.ExcelPicture picture = sheet.Pictures[i];
                if (!File.Exists(duongdanluuanh + @"\" + manganh[i] + ".png"))
                {
                    picture.Picture.Save(duongdanluuanh + @"\" + manganh[i] + ".png", ImageFormat.Png);
                }

            }
            workbook.Dispose();
        }
        public void copyanhKHtunghang(string filecanlay)
        {
            var excelApp = new excel.Application();
            var wb = excelApp.Workbooks.Open(filecanlay);
            var ws = (excel.Worksheet)wb.Worksheets[1];
            string duongdanluuanh =duongdanapp + @"\luuanh";
            int hangbatdau = 0;

            List<string> tenanh = new List<string>();
            foreach (var pic in ws.Pictures())
            {
                hangbatdau = pic.TopLeftCell.Row;

                tenanh.Add(ws.Cells[hangbatdau, 2].value);
            }

            string[] manganh = tenanh.ToArray();

            wb.Close();
            excelApp.Quit();
            Marshal.FinalReleaseComObject(excelApp);
            Marshal.FinalReleaseComObject(wb);


            Workbook workbook = new Workbook();
            workbook.LoadFromFile(filecanlay);

            Worksheet sheet = workbook.Worksheets[0];
            if (!Directory.Exists(duongdanluuanh))
            {
                Directory.CreateDirectory(duongdanluuanh);
            }

            for (int i = 1; i < manganh.Length; i++)
            {
                Spire.Xls.ExcelPicture picture = sheet.Pictures[i];
                if (!File.Exists(duongdanluuanh + @"\" + manganh[i] + ".png"))
                {
                    picture.Picture.Save(duongdanluuanh + @"\" + manganh[i] + ".png", ImageFormat.Png);
                }

            }
            workbook.Dispose();
        }
       
        #endregion
    }
}
