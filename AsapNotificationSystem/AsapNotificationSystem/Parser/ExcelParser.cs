using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AsapNotificationSystem.Convert;
using AsapNotificationSystem.DataBase.Models;
using AsapNotificationSystem.Message;
using OfficeOpenXml;

namespace AsapNotificationSystem.Parser
{
    public class ExcelParser : IParser<string, IEnumerable<MessageModel>>
    {
        public ExcelParser()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public IEnumerable<MessageModel> Parse(string path)
        {
            var mtDict = new Dictionary<string, int>
            {
                { "Корпус (Кампус ДВФУ) / Общежитие (материк)", -1 },
                { "Дата начала работ", -1},
                { "Время начала работ", -1 },
                { "Дата окончания работ", -1 },
                { "Время окончания работ", -1 },
                { "Отключаемые системы", -1 },
            };

            var fileInfo = XlsToXlsxConverter.Convert(path);
            using var package = new ExcelPackage(fileInfo);

            var ws = package.Workbook.Worksheets.First();
            var count = ws.Cells["A:A"].Count() - 1;
            var temp = ws.Cells["1:1"];
            
            temp.AsParallel().ForAll(x =>
            {
                if (mtDict.ContainsKey(x.Text))
                    mtDict[x.Text] = x.Address[0] - 'A' + 1;
            });

            if(mtDict.ContainsValue(-1))
                throw new Exception("Invalid Excel");

            var res = new List<MessageModel>();

            Enumerable.Range(0, count).AsParallel().ForAll(x =>
            {
                try
                {
                    var mes = new MessageModel();
                    var d = new Dictionary<string, string>();

                    foreach (var col in mtDict.Keys)
                    {
                        d.Add(col, "");
                    }

                    mtDict.Keys.AsParallel().ForAll(i =>
                    {
                        d[i] = ws.Cells[2 + x, mtDict[i]].First().Text;
                    });

                    mes.BuildingNumber = StringToBnConverter.Convert(d["Корпус (Кампус ДВФУ) / Общежитие (материк)"]);

                    if (mes.BuildingNumber == BuildingNumber.Other)
                        throw new Exception();

                    mes.From = $"{d["Дата начала работ"].Replace('/', '.')} {d["Время начала работ"]}";
                    mes.To = $"{d["Дата окончания работ"].Replace('/', '.')} {d["Время окончания работ"]}";
                    mes.What = d["Отключаемые системы"];

                    mes.TimeFrom = DateTime.Parse(mes.From);
                    mes.TimeTo = DateTime.Parse(mes.To);
                
                    lock (res)
                    {
                        res.Add(mes);
                    }
                }
                catch
                {
                    // ignored
                }
            });

            return res;
        }
    }
}
