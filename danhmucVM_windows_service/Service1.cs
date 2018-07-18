using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;


using System.Runtime.InteropServices;
using System.Threading;

namespace danhmucVM_windows_service
{
    public partial class Service1 : ServiceBase
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr handle, int nCmdShow);

        private const int KEYEVENTF_EXTENDEDKEY = 0x1;
        private const int KEYEVENTF_KEYUP = 0x2;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private static void PressKey(byte keyCode)
        {
            keybd_event(keyCode, 0x45, KEYEVENTF_EXTENDEDKEY, 0);
            keybd_event(keyCode, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        Thread luongmail;
        Thread xulyanh;
        Thread chenmahang;
        Thread loadLandau;

        static string filedanhmucmoi = null;

        #region ham thuc thi

        void loadkhikhoidong()
        {
            while (true)
            {
                luongmail = new Thread(hamcapnhat);
                luongmail.IsBackground = true;
                luongmail.Start();

                xulyanh = new Thread(hamxulyanh);
                xulyanh.IsBackground = true;
                xulyanh.Start();

                chenmahang = new Thread(chenma);
                chenmahang.IsBackground = true;
                chenmahang.Start();

                chenmahang.Join();
                Thread.Sleep(300000);
            }
        }
        void hamcapnhat() // 1
        {
            try
            {
                ghiloi.WriteLogError("Bắt đầu cập nhật");
                var xulyOL = xulyoutlook.Instance();
                var ham = xulythongtin.Khoitao();
                xulyOL.xuly();
                filedanhmucmoi = xulyOL.tenfile(); // chinh sua luu file moi vao 1 bang trong database cnf, de fia client se biet duoc moi cap nhat file nao

                ham.luudanhmuchangmoi();
            }
            catch (Exception e)
            {
                ghiloi.WriteLogError(e);
                throw;
            }
            IntPtr hWnd = FindWindow(null, "Internet Security Warning"); // Window Titel
            if (hWnd != IntPtr.Zero)
            {
                ShowWindow(hWnd, 9);
                SetForegroundWindow(hWnd);
                PressKey(0x09);
                PressKey(0x0D);
            }
        }
        void hamxulyanh() // 2
        {
            try
            {
                luongmail.Join(); // ham xulyanh se doi cho thread luongmail chay xong moi bat dau chay
                var ham = xulythongtin.Khoitao();
                ham.xulyanh();
            }
            catch (Exception e)
            {
                ghiloi.WriteLogError(e);
                throw;
            }
            
        }
        void chenma() // 3
        {
            try
            {
                xulyanh.Join(); //ham chenma(thread chenmahang) se doi cho ham xulyanh chay xong moi chay
                var ham = xulythongtin.Khoitao();
                var con = ketnoi.Instance();
                ham.xulymahang();

                IntPtr hWnd = FindWindow(null, "Internet Security Warning"); // Window Titel
                if (hWnd != IntPtr.Zero)
                {
                    ShowWindow(hWnd, 9);
                    SetForegroundWindow(hWnd);
                    PressKey(0x09);
                    PressKey(0x0D);
                }
                con.chenvaoFiledanhmuc(filedanhmucmoi);
                
                ghiloi.WriteLogError("Đã cập nhật xong");
            }
            catch (Exception e)
            {
                ghiloi.WriteLogError(e);
                throw;
            }
            
        }

        #endregion
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            loadLandau = new Thread(loadkhikhoidong);
            loadLandau.IsBackground = true;
            loadLandau.Start();
        }

        protected override void OnStop()
        {
            ghiloi.WriteLogError("Dung !");
        }
    }
}
