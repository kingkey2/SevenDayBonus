using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OcwSevenDayBonus {
    class Program {
        private static System.Threading.Thread iMainThread;
        private static System.Threading.Thread iQueryThread;
        private static SettingObjectClass SC;

        private static string iRunPath = UnhandleExceptionClass.GetRunPath();
        private static string iDirSplit = "\\";
        private static string iLogPath = iRunPath + iDirSplit + "Log";
        private static string iINIFilename = iRunPath + iDirSplit + "OcwSevenDayBonus.ini";

        static void Main(string[] args) {
            AppDomain.CurrentDomain.UnhandledException += UnhandleExceptionClass.ProcessUnhandleException;

            iMainThread = System.Threading.Thread.CurrentThread;

            if (System.IO.File.Exists(iINIFilename) == false) {
                SC = new SettingObjectClass();

                SettingModuleClass.SaveSetting(iINIFilename, SC);
            }

            SC = (SettingObjectClass)SettingModuleClass.LoadSetting(iINIFilename, typeof(SettingObjectClass));

            QueryThread_Monitor();

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }

        static void QueryThread_Monitor() {
            try {
                QueryThread_Loop();
            } catch (Exception ex) {
                UnhandleExceptionClass.LogToFile(iLogPath, ex.ToString());
            }
        }

        static void QueryThread_Loop() {
            MgmtAPI.MgmtAPI MgmtAPI = new MgmtAPI.MgmtAPI();
            MgmtAPI.Url = SC.ServerAPI;

            while (true) {
                MgmtAPI.APIResult ret = new MgmtAPI.APIResult();
                string hash;
                DateTime dateTime = DateTime.Now;
                DateTime targetTime = RoundUp(dateTime, TimeSpan.FromMinutes(15));
                string key = "";

                if (SC.IsTest) {
                    key = "5e93ebf6-c4ff-4f2a-862d-d4d9b755f173";
                } else {
                    key = "fdb40b9c-1082-4541-8f08-0a696c00b4ce";
                }

                hash = dateTime.ToString("yyyy/MM/dd HH:mm:01") + "_" + GetMD5(targetTime.ToString("yyyy/MM/dd HH:mm:ss") + key, false).ToLower();

                ret = MgmtAPI.SetSevenDateBonusForConsole(hash);

                if (ret != null) {
                    if (ret.Result == OcwSevenDayBonus.MgmtAPI.enumResult.OK) {
                        Environment.Exit(0);
                    } else {
                        UnhandleExceptionClass.LogToFile(iLogPath, "Exception Msg:" + ret.Message);
                        Environment.Exit(0);
                    }
                }

            }
        }

        public static DateTime RoundUp(DateTime dt, TimeSpan d) {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }

        public static string GetMD5(string DataString, bool Base64Encoding = true) {
            return GetMD5(System.Text.Encoding.UTF8.GetBytes(DataString), Base64Encoding);
        }

        public static string GetMD5(byte[] Data, bool Base64Encoding = true) {
            System.Security.Cryptography.MD5CryptoServiceProvider MD5Provider = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hash;
            System.Text.StringBuilder RetValue = new System.Text.StringBuilder();

            hash = MD5Provider.ComputeHash(Data);
            MD5Provider = null;

            if (Base64Encoding) {
                RetValue.Append(System.Convert.ToBase64String(hash));
            } else {
                foreach (byte EachByte in hash) {
                    string ByteStr = EachByte.ToString("x");

                    ByteStr = new string('0', 2 - ByteStr.Length) + ByteStr;
                    RetValue.Append(ByteStr);
                }
            }


            return RetValue.ToString();
        }
    }


    public class APIResult {
        public enum enumResultState {
            OK,
            ERR
        }

        public enumResultState ResultState;
        public string Message;
    }
}
