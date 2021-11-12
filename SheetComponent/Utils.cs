using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetComponent
{
    public static class Utils
    {
        private static string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string GetColumnLabel(int col) => col > 0 ? InternalGetLabel(col - 1) : "";
        private static string InternalGetLabel(int id) => (id > letters.Length ? GetColumnLabel(id / letters.Length) : "") + letters[id % letters.Length];

        public static string JoinStrings(this IEnumerable<string> list, string separatore = " ") => string.Join(separatore, list);
    }
}
