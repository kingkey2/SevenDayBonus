using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class SettingModuleClass {
    public static object LoadSetting(string Filename, Type objType) {
        string XMLContent;
        object RetValue = null;

        if (System.IO.File.Exists(Filename)) {
            XMLContent = System.IO.File.ReadAllText(Filename, System.Text.Encoding.UTF8);

            try { RetValue = XMLDeserial(XMLContent, objType); } catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }

        return RetValue;
    }

    public static void SaveSetting(string SaveFilename, object Obj) {
        string XMLContent;

        XMLContent = XMLSerial(Obj, System.Text.Encoding.UTF8);

        System.IO.File.WriteAllText(SaveFilename, XMLContent, System.Text.Encoding.UTF8);
    }

    public static string XMLSerial(object obj, System.Text.Encoding Encoding) {
        System.Xml.Serialization.XmlSerializer xmlSer;
        System.Xml.XmlWriter xmlWR;
        System.IO.MemoryStream Stm;
        byte[] XMLArray;
        string RetValue;

        xmlSer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
        Stm = new System.IO.MemoryStream();

        if (Encoding == null)
            Encoding = System.Text.Encoding.UTF8;

        xmlWR = System.Xml.XmlWriter.Create(Stm, new System.Xml.XmlWriterSettings() { Encoding = Encoding });
        xmlSer.Serialize(xmlWR, obj);

        Stm.Position = 0;
        XMLArray = (byte[])Array.CreateInstance(typeof(byte), Stm.Length);

        Stm.Read(XMLArray, 0, XMLArray.Length);
        Stm.Dispose();
        Stm = null;

        //xmlWR.Close();
        xmlWR = null;

        RetValue = Encoding.GetString(XMLArray);

        return RetValue;
    }

    public static object XMLDeserial(string xmlContent, Type objType) {
        System.Xml.Serialization.XmlSerializer xmlSer;
        System.IO.MemoryStream Stm;
        byte[] XMLArray;
        object RetValue = null;
        System.Xml.XmlReader xTR;

        if (string.IsNullOrEmpty(xmlContent) == false) {
            XMLArray = System.Text.Encoding.UTF8.GetBytes(xmlContent);

            Stm = new System.IO.MemoryStream(XMLArray);
            Stm.Position = 0;

            xTR = System.Xml.XmlReader.Create(Stm);
            xmlSer = new System.Xml.Serialization.XmlSerializer(objType);
            RetValue = xmlSer.Deserialize(xTR);

            Stm.Close();
            Stm.Dispose();
            Stm = null;

            //xTR.Close();
            xTR = null;

            xmlSer = null;
        }

        return RetValue;
    }
}
