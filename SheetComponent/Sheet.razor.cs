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
        [Parameter] public bool UseTables { get; set; } = false;

        private List<Cell> SheetData = new();

        private ElementReference CellInput { get; set; }
        private ElementReference StatusBar { get; set; }
        private ElementReference SheetDiv { get; set; }

        public Range Selection { get; private set; } = new() { Start = Position.Unit, End = Position.Unit };
        public Range WorkingArea { get; private set; }

        private bool MouseDown;
        private MouseEventArgs LastMouseEvent;
        private bool KeyDown;
        private KeyboardEventArgs LastKeyEvent;
        private bool InEdit;
        private Position CurrentPosition = Position.Unit;

        private Position StartPosition = Position.Unit;
        private Position EndPosition = Position.Unit;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await SheetDiv.FocusAsync(); // permette al div principale di intercettare tutti gli eventi da tastiera
        }

        protected override void OnParametersSet()
        {
            WorkingArea = new() { Start = Position.Unit, End = (Rows, Columns) };
        }

        private void SetSelection(Position? start = null, Position? end = null)
        {
            if (start != null)
            {
                //CurrentPosition = 
                StartPosition = start.Value;
                if (end == null) EndPosition = start.Value;
            }
            if (end != null) EndPosition = end.Value;
            Selection = new Range() { Start = StartPosition, End = EndPosition };
            //if (CellInput.Context != null) await CellInput.FocusAsync();
        }

        private void CellClick(Position pos, MouseEventArgs args)
        {
            LastMouseEvent = args;
            //if (args.Button == 0)
            //{
            //    CurrentPosition = pos;
            //}
        }

        private void CellMouseDown(Position pos, MouseEventArgs args)
        {
            LastMouseEvent = args;
            if (args.Button == 0)
            {
                MouseDown = true;
                if (pos == Position.OrigineAssi)
                {
                    CurrentPosition = (1, 1); //TODO: impostare all'origne della viewport
                    SetSelection(pos);
                    MouseDown = false; // interrompo l'ulteriore selezione di qualunque altra cosa
                }
                else if (pos.IsIntestazioneRiga)
                {
                    CurrentPosition = (pos.Row, 1); //TODO: impostare la colonna alla prima visibile della viewport
                    SetSelection((pos.Row, 0)); // seleziono intera riga
                }
                else if (pos.IsIntestazioneColonna)
                {
                    CurrentPosition = (1, pos.Col); //TODO: impostare la riga alla prima visibile della viewport
                    SetSelection((0, pos.Col)); // seleziono intera colonna
                }
                else
                {
                    if (args.ShiftKey) SetSelection(end: pos);
                    else
                    {
                        CurrentPosition = pos;
                        SetSelection(start: pos);
                    }
                }
            }
        }

        private void CellMouseUp(Position pos, MouseEventArgs args)
        {
            LastMouseEvent = args;
            if (MouseDown && args.Button == 0)
            {
                MouseDown = false;
                if (args.ShiftKey)
                {
                    if (pos.InRange(WorkingArea)) SetSelection(end: pos);
                }
                //SetSelection(end: pos);
                //if (EndPosition != StartPosition)
                //{
                //    CurrentPosition = StartPosition; // mouse up e mouse down in celle diverse => l'evento click non viene sollevato e devo sopperire
                //}
            }
        }

        private void CellMouseMove(Position pos, MouseEventArgs args)
        {
            if (MouseDown)
            {
                SetSelection(end: pos);
                if (EndPosition != StartPosition)
                {
                    CurrentPosition = StartPosition; // mouse up e mouse move in celle diverse => l'evento click non viene sollevato e devo sopperire
                }
            }
        }

        private void SheetKeyDown(KeyboardEventArgs args)
        {
            KeyDown = true;
            LastKeyEvent = args;
            Action<Position> moveFunc = args.ShiftKey ? ExtendCurrentSelection : MoveCurrentPosition;
            switch (args.Key)
            {
                case "ArrowLeft": moveFunc((0, -1)); break;
                case "ArrowRight": moveFunc((0, 1)); break;
                case "ArrowUp": moveFunc((-1, 0)); break;
                case "ArrowDown": moveFunc((1, 0)); break;
            }
        }

        private void MoveCurrentPosition(Position delta)
        {
            var newpos = CurrentPosition + delta;
            if (newpos.InRange(WorkingArea))
            {
                CurrentPosition = newpos;
                SetSelection(newpos);
            }
            else
            {
                SetSelection(CurrentPosition); // per emulare il comportamento di excel, reimposto la selezione sulla cella corrente senza spostarmi
            }
        }

        private void ExtendCurrentSelection(Position delta)
        {
            var newEnd = Selection.End + delta;
            if (newEnd.InRange(WorkingArea)) SetSelection(end: newEnd);
        }

            private Cell GetCell(Position pos, bool createNew = false)
        {
            var cell = SheetData.Find(x => x.Position == pos);
            if (cell is null && createNew) SheetData.Add(cell = new(pos));
            return cell;
        }

        private string GetCellClass(Position pos)
        {
            string resp;

            if (pos == Position.OrigineAssi)
            {
                resp = "header";
            }
            else if (pos.IsIntestazioneRiga)
            {
                resp = "header row-header";
                if (Selection.Top == 0 || (pos.Row >= Selection.Top && pos.Row <= Selection.Bottom)) resp += " selected";
                if (Selection.Left == 0) resp += " full";
            }
            else if (pos.IsIntestazioneColonna)
            {
                resp = "header col-header";
                if (Selection.Left == 0 || (pos.Col >= Selection.Left && pos.Col <= Selection.Right)) resp += " selected";
                if (Selection.Top == 0) resp += " full";
            }
            else // area dati (riga e colonna > 0)
            {
                resp = "cell";
                if (pos.InRange(Selection) || Selection.Start == Position.OrigineAssi)
                {
                    if (pos != CurrentPosition) resp += " selected";
                    if (pos.Row == Selection.Top || (Selection.Start.Row == 0 && pos.Row == 1)) resp += " selection-top";
                    if (pos.Row == Selection.Bottom) resp += " selection-bottom";
                    if (pos.Col == Selection.Left || (Selection.Start.Col == 0 && pos.Col == 1)) resp += " selection-left";
                    if (pos.Col == Selection.Right) resp += " selection-right";
                }
            }

            return resp;
        }
    }
}
