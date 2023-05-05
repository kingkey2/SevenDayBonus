using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class UnhandleExceptionClass {
    private static System.Collections.ArrayList iSyncRoot = new System.Collections.ArrayList();

    public static void ProcessUnhandleException(object sender, System.UnhandledExceptionEventArgs e) {
        LogToFile(GetRunPath() + "\\Log", e.ExceptionObject.ToString());
    }

    public static string GetRunPath() {
        return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    }

    public static void LogToFile(string LogPath, string Msg) {
        Console.WriteLine(System.DateTime.Now.ToString() + "  " + Msg);

        lock (iSyncRoot) {
            if (System.IO.Directory.Exists(LogPath) == false) {
                try { System.IO.Directory.CreateDirectory(LogPath); } catch (Exception ex) { }
            }

            if (System.IO.Directory.Exists(LogPath)) {
                string OutputContent = "-------------------------------------------------\r\n" + System.DateTime.Now.ToString() + "\r\n" + Msg + "\r\n";

                try { System.IO.File.AppendAllText(LogPath + "\\" + System.DateTime.Now.ToString("yyyy-MM-dd") + ".log", OutputContent); } catch (Exception ex) { }
            }
        }
    }
}
