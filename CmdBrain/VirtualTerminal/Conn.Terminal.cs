namespace No8.CmdBrain.VirtualTerminal;

public static partial class Con
{
    public static void ClearScreen() => Send(Terminal.ControlSeq.ClearScreen);

	public static class Cursor
	{
		public static void Set(int row, int col) => Send(Terminal.Cursor.Set(row, col));
		public static void Up(int n = 1) => Send(Terminal.Cursor.Up(n));
		public static void Down(int n = 1) => Send(Terminal.Cursor.Down(n));
		public static void Right(int n = 1) => Send(Terminal.Cursor.Right(n));
		public static void Left(int n = 1) => Send(Terminal.Cursor.Left(n));


        private static bool _cursorHidden = false;
        public static bool Hidden
        {
            get => _cursorHidden;
            set
            {
                if (_cursorHidden != value)
                {
                    _cursorHidden = value;
                    if (_cursorHidden)
                        Send(Terminal.Cursor.Hide);
                    else
                        Send(Terminal.Cursor.Show);
                }
            }
        }

        /// <summary>
        ///     Set cursor style (DECSCUSR), VT520. 
        /// </summary>
        /// <param name="style">
        ///     <para>0  ⇒  blinking block.</para>
        ///     <para>1  ⇒  blinking block (default).</para>
        ///     <para>2  ⇒  steady block.</para>
        ///     <para>3  ⇒  blinking underline.</para>
        ///     <para>4  ⇒  steady underline.</para>
        ///     <para>5  ⇒  blinking bar, xterm.</para>
        ///     <para>6  ⇒  steady bar, xterm.</para>
        /// </param>
        public static void SetStyle(int style) => Send(Terminal.Cursor.SetStyle(style));

        public static void SaveCursor() => Send(Terminal.Cursor.SaveCursor);
        public static void RestoreCursor() => Send(Terminal.Cursor.RestoreCursor);
    }


    public static class Mode
    {
        private static bool _altScreen = false;
        public static bool AltScreen
        {
            get => _altScreen;
            set
            {
                if (value == _altScreen) return;
                _altScreen = value;
                if (_altScreen)
                    Send(Terminal.Mode.ScreenAlt);
                else
                    Send(Terminal.Mode.ScreenNormal);
            }
        }

        public static void LocatorReportingCells() => Send(Terminal.Mode.LocatorReportingCells);
        public static void StopLocatorReporting() => Send(Terminal.Mode.StopLocatorReporting);

        public static void SendMouseXY() => Send(Terminal.Mode.SendMouseXY);
        public static void MouseTrackingHilite() => Send(Terminal.Mode.MouseTrackingHilite);
        public static void MouseTrackingCell() => Send(Terminal.Mode.MouseTrackingCell);
        public static void MouseTrackingAll() => Send(Terminal.Mode.MouseTrackingAll);
        public static void MouseTrackingFocus() => Send(Terminal.Mode.MouseTrackingFocus);
        public static void MouseTrackingUtf8() => Send(Terminal.Mode.MouseTrackingUtf8);
        public static void MouseTrackingSGR() => Send(Terminal.Mode.MouseTrackingSGR);

        public static void StartTracking() => Send(Terminal.Mode.StartTracking);
        public static void StopTracking() => Send(Terminal.Mode.StopTracking);

        public static void StopMouseXY() => Send(Terminal.Mode.StopMouseXY);
        public static void StopMouseTrackingHilite() => Send(Terminal.Mode.StopMouseTrackingHilite);
        public static void StopMouseTrackingCell() => Send(Terminal.Mode.StopMouseTrackingCell);
        public static void StopMouseTrackingAll() => Send(Terminal.Mode.StopMouseTrackingAll);
    }

    public static class EditingControlFunctions
    {
        public static void DeleteCharacter(int n = 1) => Send(Terminal.EditingControlFunctions.DeleteCharacter(n));
        public static void DeleteColumn(int n = 1) => Send(Terminal.EditingControlFunctions.DeleteColumn(n));
        public static void DeleteLine(int n = 1) => Send(Terminal.EditingControlFunctions.DeleteLine(n));
        public static void EraseCharacter(int n = 1) => Send(Terminal.EditingControlFunctions.EraseCharacter(n));

        /// <summary>
        ///     Erase in Display
        /// </summary>
        /// <param name="n">
        ///     0 Cursor to end of display
        ///     1 Top of display through cursor
        ///     2 Top to bottom of display
        /// </param>
        public static void EraseInDisplay(int n) => Send(Terminal.EditingControlFunctions.EraseInDisplay(n));

        /// <summary>
        ///     Erase in Line 
        /// </summary>
        /// <param name="n">
        ///     0 Cursor to end of line
        ///     1 Start of line through cursor
        ///     2 Start to end of line
        /// </param>
        public static void EraseInLine(int n) => Send(Terminal.EditingControlFunctions.EraseInLine(n));

        public static void InsertCharacter(int n = 1) => Send(Terminal.EditingControlFunctions.InsertCharacter(n));
        public static void InsertColumn(int n = 1) => Send(Terminal.EditingControlFunctions.InsertColumn(n));
        public static void InsertLine(int n = 1) => Send(Terminal.EditingControlFunctions.InsertLine(n));

        public static void InsertMode() => Send(Terminal.EditingControlFunctions.InsertMode);
        public static void ReplaceMode() => Send(Terminal.EditingControlFunctions.ReplaceMode);

        /// <summary>
        ///     Select Character Attribute
        /// </summary>
        /// <param name="ps">
        ///     0 DECSED and DECSEL can erase characters.
        ///     1 DECSED and DECSEL cannot erase characters.
        ///     2 Same as 0.
        /// </param>
        public static void SelectCharacterAttribute1(int ps) => Send(Terminal.EditingControlFunctions.SelectCharacterAttribute1(ps));

        /// <summary>
        ///     Selective Erase in Display 
        /// </summary>
        /// <param name="ps">
        ///     Ps Erase from . . .
        ///     0 Cursor to end of display
        ///     1 Top of display through cursor
        ///     2 Top to bottom of display
        /// </param>
        public static void SelectiveEraseInDisplay(int ps) => Send(Terminal.EditingControlFunctions.SelectiveEraseInDisplay(ps));

        /// <summary>
        ///     Selective Erase in Line
        /// </summary>
        /// <param name="ps">
        ///     Ps Erase from . . .
        ///     0 Cursor to end of line
        ///     1 Start of line through cursor
        ///     2 Start to end of line
        /// </param>
        public static void SelectiveEraseInLine(int ps) => Send(Terminal.EditingControlFunctions.SelectiveEraseInLine(ps));
    }

    public static class Scroll
    {
        public static void Set(int topRow, int bottomRow) => Send(Terminal.Scroll.Set(topRow, bottomRow));
        public static void Clear() => Send(Terminal.Scroll.Clear);
        public static void Up(int count) => Send(Terminal.Scroll.Up(count));
        public static void Down(int count) => Send(Terminal.Scroll.Down(count));
    }

}
