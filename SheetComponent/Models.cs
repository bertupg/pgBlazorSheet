using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetComponent
{
    public struct Position
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public override string ToString() => Utils.GetColumnLabel(Col) + Row.ToString();

        public static bool operator == (Position a, Position b) => a.Row == b.Row && a.Col == b.Col;
        public static bool operator !=(Position a, Position b) => !(a==b);

        public override bool Equals(object obj) => obj is Position other && this == other;
        public override int GetHashCode() => (Row, Col).GetHashCode();

        public static implicit operator Position((int, int) pos) => new Position() { Row = pos.Item1, Col = pos.Item2 };
        public void Deconstruct(out int row, out int col) => (row, col) = (Row, Col);
    }

    public struct Range
    {
        public Position Start { get; set; }
        public Position End { get; set; }

        public int Top { get => Math.Min(Start.Row, End.Row); }
        public int Bottom { get => Math.Max(Start.Row, End.Row); }
        public int Left { get => Math.Min(Start.Col, End.Col); }
        public int Right { get => Math.Max(Start.Col, End.Col); }

        public int Height { get => Bottom - Top + 1; }
        public int Width { get => Right - Left + 1; }

    }

    public class Cell
    {
        public Position Position { get; set; }
        public string Value { get; set; }
    }

}
