using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetComponent
{
    public partial class Sheet
    {
        [Parameter] public int Rows { get; set; } = 20;
        [Parameter] public int Columns { get; set; } = 10;

        private List<Cell> SheetData = new();

        private ElementReference CellInput { get; set; }
        private ElementReference StatusBar { get; set; }

        public Range Selection { get; private set; }
        //public bool InEdit { get; private set; }

        private bool MouseDown;
        private Position CurrentPosition;
        private Position PreviousPosition;

        private Position StartPosition;
        private Position EndPosition;
        private async Task SetSelection(Position? start = null, Position? end = null)
        {
            if (start != null)
            {
                CurrentPosition = 
                StartPosition = start.Value;
                if (end == null) EndPosition = start.Value;
            }
            if (end != null) EndPosition = end.Value;
            Selection = new Range() { Start = StartPosition, End = EndPosition };
            //StateHasChanged();
            if (CellInput.Context != null) await CellInput.FocusAsync();
        }

        private void TableBodyKeyPress(KeyboardEventArgs args)
        {
            Console.WriteLine(nameof(TableBodyKeyPress) + ":" + args.Code);
            //if (!InEdit)
            //{
            //    InEdit = true;
            //    StateHasChanged();
            //}
        }

        private void CellClick(Position pos, MouseEventArgs args)
        {
            if (args.Button == 0)
            {
                //CurrentPosition = pos;
            }
        }

        private async void CellMouseDown(Position pos, MouseEventArgs args)
        {
            if (args.Button == 0)
            {
                if (pos != CurrentPosition) // sto cambiando cella: se ci sono digitazioni non salvate, le consolido
                {

                }
                MouseDown = true;
                PreviousPosition = CurrentPosition;
                await SetSelection(start: pos);
            }
        }

        private async void CellMouseUp(Position pos, MouseEventArgs args)
        {
            if (args.Button == 0)
            {
                MouseDown = false;
                await SetSelection(end: pos);
                if (EndPosition != StartPosition)
                {
                    CurrentPosition = StartPosition; // mouse up e mouse down in celle diverse => l'evento click non viene sollevato e devo sopperire
                }
            }
        }

        private async void CellMouseMove(Position pos, MouseEventArgs args)
        {
            if (MouseDown)
            {
                await SetSelection(end: pos);
                if (EndPosition != StartPosition)
                {
                    CurrentPosition = StartPosition; // mouse up e mouse move in celle diverse => l'evento click non viene sollevato e devo sopperire
                }
            }
        }

        private async void CellInputKeyPress(KeyboardEventArgs args)
        {
            //await StatusBar.FocusAsync();
        }

        private async void CellInputKeyDown(KeyboardEventArgs args)
        {
        }

        private void UpdateCellValue(Position pos, string newValue)
        {
            var cell = SheetData.Find(x => x.Position == CurrentPosition);
            if (cell is null) SheetData.Add(cell = new() { Position = CurrentPosition });
            cell.Value = newValue;
            Console.WriteLine($"Changed {CurrentPosition} to {cell.Value}");
        }

        private Cell GetCell(Position pos, bool createNew = false)
        {
            var cell = SheetData.Find(x => x.Position == pos);
            if (cell is null && createNew) SheetData.Add(cell = new() { Position = pos });
            return cell;
        }

        private IEnumerable<string> getTdClasses(Position pos)
        {
            if (pos.Col >= Selection.Left && pos.Col <= Selection.Right && pos.Row >= Selection.Top && pos.Row <= Selection.Bottom)
            {
                if (pos != Selection.Start) yield return "selected";
                if (pos.Row == Selection.Top) yield return "selection-top";
                if (pos.Row == Selection.Bottom) yield return "selection-bottom";
                if (pos.Col == Selection.Left) yield return "selection-left";
                if (pos.Col == Selection.Right) yield return "selection-right";
            }
        }

        private IEnumerable<string> getThClasses(Position pos)
        {
            if (pos.Col == 0) yield return "vertical-ruler-cell";
        }

    }
}
