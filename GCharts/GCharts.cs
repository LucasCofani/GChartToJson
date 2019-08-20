using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GCharts
{
    public static class GCharts
    {
        private static string ToDate(string data)
        {
            //fazer javascript
            var saida = "\"Date(";
            var cdata = new DateTime();
            DateTime.TryParse(data, out cdata);
            saida += cdata.Year + "," + cdata.Month + "," + cdata.Day;
            return saida + ")\"";
        }
        public static string ListToGChartJson<T>(this List<T> list)
        {
            if (list.Count.Equals(0))
                throw new System.ArgumentException("The list is empty!");
            else
            {
                var _prop = new List<string>();
                {
                    var listTypes = new[] { "System.Int32", "System.DateTime", "System.Double", "System.String" };
                    var prop = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var item in prop)
                    {
                        if (listTypes.Contains(item.PropertyType.FullName))
                            _prop.Add(item.Name);
                    }
                }
                var Dict = new Dictionary<string, string>();
                Dict["System.Int32"] = "number";
                Dict["System.DateTime"] = "date";
                Dict["System.String"] = "string";
                Dict["System.Double"] = "number";

                var JSONstring = "{ \"cols\": [@cols], \"rows\": [@rows]}";
                var rows = new StringBuilder();
                var col = new StringBuilder();
                foreach (var prop in _prop)
                {
                    col.Append("{\"id\":\"" + prop + "\",\"label\": \"" +
                            prop + "\",\"type\":\"" +
                            Dict[list[0].GetType().GetProperty(prop).GetValue(list[0]).GetType().FullName]
                            + "\"}"
                            );
                }
                JSONstring = JSONstring.Replace("@cols", col.ToString().Replace("}{", "},{"));
                foreach (var item in list)
                {
                    var row = new StringBuilder();
                    foreach (var prop in _prop)
                    {
                        row.Append("{\"v\": " +
                                //Trata string
                                (Dict[item.GetType().GetProperty(prop).GetValue(item).GetType().FullName] == "string" ? "\""
                                + item.GetType().GetProperty(prop).GetValue(item).ToString().Replace("\"","\\\"") + "\"" : "")
                                //Trata Data
                                + (Dict[item.GetType().GetProperty(prop).GetValue(item).GetType().FullName] == "date" ?
                                ToDate(item.GetType().GetProperty(prop).GetValue(item).ToString())
                                : "")
                                //Trata numeros
                                + (Dict[item.GetType().GetProperty(prop).GetValue(item).GetType().FullName] == "number" ?
                                item.GetType().GetProperty(prop).GetValue(item).ToString().Replace(",", ".")
                                : "")
                            + "}"
                            );
                    }
                    rows.Append("{\"c\":[" + row.ToString().Replace("}{", "},{") + "]}");
                }
                JSONstring = JSONstring.Replace("@rows", rows.ToString().Replace("]}{\"c\":[", "]},{\"c\":["));
                //Console.WriteLine(JSONstring);
                return JSONstring;
            }
        }
    }
}
