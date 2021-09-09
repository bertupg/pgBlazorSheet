using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetComponent
{
    public static class Utils
    {
        public static string GetColumnLabel(int id, string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ") => (id > letters.Length ? GetColumnLabel(id / letters.Length) : "") + letters[id % letters.Length];
    }
}
