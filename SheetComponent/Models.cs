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
        /// <summary>La posizione che corrisponde all'incrocio degli assi di intestazione righe/colonne. Indica anche la dimensione di un range nullo (zero righe x zero colonne)</summary>
        public static Position OrigineAssi = (0, 0);
        /// <summary>La posizione che corrisponde alla prima cella in alto a sinistra (A1). Indica anche la dimensione di un range di una singola cella (1 riga x 1 colonna)</summary>
        public static Position Unit = (1, 1);

        public int Row { get; set; }
        public int Col { get; set; }

        public bool IsIntestazioneRiga { get => Col == 0 && Row > 0; }
        public bool IsIntestazioneColonna { get => Row == 0 && Col > 0; }

        public bool InRange(Range r) =>
               (r.Start.IsIntestazioneColonna || (Row >= r.Top && Row <= r.Bottom))
            && (r.Start.IsIntestazioneRiga || (Col >= r.Left && Col <= r.Right));

        public override string ToString() => Utils.GetColumnLabel(Col) + (Row==0?"":Row.ToString());

        public static bool operator == (Position a, Position b) => a.Row == b.Row && a.Col == b.Col;
        public static bool operator !=(Position a, Position b) => !(a==b);

        public static Position operator +(Position a, Position b) => new Position() { Row = a.Row + b.Row, Col = a.Col + b.Col };

        public override bool Equals(object obj) => obj is Position other && this == other;
        public override int GetHashCode() => (Row, Col).GetHashCode();

        public static implicit operator Position((int, int) pos) => new Position() { Row = pos.Item1, Col = pos.Item2 };
        //public static implicit operator (int, int)(Position pos) => (pos.Row, pos.Col);

        public void Deconstruct(out int row, out int col) => (row, col) = (Row, Col);
    }

    public struct Range
    {
        public Position Start { get; set; }
        public Position End { get; set; }

        /// <summary>Restituisce il valore della riga più in alto (con numero di riga più basso)</summary>
        public int Top { get => Math.Min(Start.Row, End.Row); }

        /// <summary>Restituisce il valore della riga più in basso (con numero di riga più alto)</summary>
        public int Bottom { get => Math.Max(Start.Row, End.Row); }

        /// <summary>Restituisce il valore della colonna più a sinistra (con la lettera più bassa)</summary>
        public int Left { get => Math.Min(Start.Col, End.Col); }

        /// <summary>Restituisce il valore della colonna più a destra (con la lettera più alta)</summary>
        public int Right { get => Math.Max(Start.Col, End.Col); }

        /// <summary>Restituisce il numero di righe totali</summary>
        private int Height { get => Top == 0 ? 0 : (Bottom - Top + 1); }

        /// <summary>Restituisce il numero di colonne totali</summary>
        private int Width { get => Left == 0 ? 0 : Right - Left + 1; }

        /// <summary>Dimensione del range. Proprietà di 3° Livello: se possibile, usare <see cref="Height" /> e <see cref="Width" />, oppure <see cref="Top"/>, <see cref="Bottom"/>, <see cref="Left"/>, <see cref="Right"/></summary>
        public Position Size { get => new Position() { Row = Height, Col = Width }; }

        public override string ToString() => $"{(Position)(Top, Left)}:{(Position)(Bottom, Right)}";


    }

    public class Cell
    {
        private Cell() { }
        public Cell(Position pos) { Position = pos; }
        public Cell(Range merge) { Position = merge.Start; Merge = merge; }

        public Position Position { get; set; }
        public Range Merge { get; set; }
        public string Value { get; set; }
    }

}
