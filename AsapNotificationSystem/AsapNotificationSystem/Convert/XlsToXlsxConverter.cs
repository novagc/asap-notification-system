using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Spire.Xls;

namespace AsapNotificationSystem.Convert
{
    public static class XlsToXlsxConverter
    {
        public static FileInfo Convert(string path, bool deleteOldFile = false) => Convert(new FileInfo(path));

        public static FileInfo Convert(FileInfo fileInfo, bool deleteOldFile = false)
        {
            if (fileInfo.Extension == ".xls")
            {
                using var wb = new Workbook();
                wb.LoadFromFile(fileInfo.FullName);
                wb.SaveToFile(fileInfo.FullName + "x", ExcelVersion.Version2016);

                if(deleteOldFile)
                    File.Delete(fileInfo.FullName);

                fileInfo = new FileInfo(fileInfo.FullName + "x");
            }

            return fileInfo;
        }
    }
}
