namespace No8.CmdBrain.VirtualTerminal;

/// <summary>
/// https://invisible-island.net/xterm/ctlseqs/ctlseqs.htm
/// https://en.wikipedia.org/wiki/ANSI_escape_code
/// https://docs.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences
/// https://vt100.net/docs/vt100-ug/chapter3.html#S3.3
/// https://espterm.github.io/docs/
/// https://www.xfree86.org/current/ctlseqs.html
/// https://vt100.net/dec/ek-vt520-rm.pdf
/// </summary>
/// <remarks>
/// Escape Sequence
///     ESC                 0x1b
///     Intermediate        0x20..0x2F  <SP>!"#$%&'()*+,-./
///     Final               0x30..0x7E  0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
/// Control Sequence
///     CSI                 0x1b [
///     P...P (up to 16)    0x30..0x3F  0123456789:;<=>?
///     I...I (0 or more)   0x20..0x2F  <SP>!"#$%&'()*+,-./
///     Final (1)           0x40..0x7E  @ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
/// Device Control Strings
///     DCS                 0x1b P
///     P...P (up to 16)    0x30..0x3F  0123456789:;<=>?
///     I...I (0 or more)   0x20..0x2F  <SP>!"#$%&'()*+,-./
///     Final (1)           0x40..0x7E  @ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
///     Data String         *****
///     String Term         0x1b \
/// Parameters are always unsigned decimal numbers, separated by ;
/// CAN (0x18) Cancel can cancel a sequence. Indicates an error
/// SUB (0x1a) Cancel a sequence in progress
/// </remarks>
public static class Terminal
{
    [Obsolete]
    public static class Control8Bit
    {
        public static char IND => '\x84';   // Index
        public static char NEL => '\x85';   // Next Line   
        public static char HTS => '\x88';   // Horizontal Tab Set   
        public static char RI => '\x8D';    // Reverse Index   
        public static char SS2 => '\x8E';   // Single Shift 2   
        public static char SS3 => '\x8F';   // Single Shift 3   
        public static char DCS => '\x90';   // Device Control String   
        public static char SOS => '\x98';   // Start of String   
        public static char DECID => '\x9A'; // DEC Private Id   
        public static char CSI => '\x9B';   // Control Sequence Indicator   
        public static char ST => '\x9C';    // String Terminator   
        public static char OSC => '\x9D';   // Operating System Command   
        public static char PM => '\x9E';    // Privacy Message
        public static char APC => '\x9F';   // Application Program Command
    }

    public static class Control
    {
        public static string IND => "\x1bD";              // IND Index
        public static string NEL => "\x1bE";              // NEL Next Line
        public static string HTS => "\x1bH";              // HTS Horizontal Tab Set
        public static string RI => "\x1bM";               // RI Reverse Index
        public static string SS2 => "\x1bN";              // SS2 Single Shift Select G2, VT220
        public static string SS3 => "\x1bO";              // SS3 Single Shift Select G3, VT220
        public static string DCS => "\x1bP";              // DCS Device Control String, VT220
        public static string SPA => "\x1bV";              // SPA Start of Guarded Area
        public static string SOS => "\x1bX";              // SOS Start of String
        [Obsolete] public static string DECID => "\x1bZ"; // DECID obsolete
        public static string CSI => "\x1b[";               // CSI Control Sequence Introducer
        public static string OSC => "\x1b]";               // OSC Operating System Command
        public static string ST => "\x1b\\";               // ST String Terminator
        public static string PM => "\x1b^";                // PM Privacy Message
        public static string APC => "\x1b_";               // APC Application Program Command


        public static string AnsiConformance1 => "\x1b L"; // Set ANSI conformance level 1 (dpANS X3.134.1).
        public static string AnsiConformance2 => "\x1b M"; // Set ANSI conformance level 2 (dpANS X3.134.1).
        public static string AnsiConformance3 => "\x1b N"; // Set ANSI conformance level 3 (dpANS X3.134.1).

        public static string BackIndex => "\x1b6";         // ESC 6     Back Index (DECBI), VT420 and up.
        public static string SaveCursor => "\x1b7";        // ESC 7     Save Cursor (DECSC), VT100.
        public static string RestoreCursor => "\x1b8";     // ESC 8     Restore Cursor (DECRC), VT100.
        public static string ForwardIndex => "\x1b9";      // ESC 9     Forward Index (DECFI), VT420 and up.
        public static string ApplicationKeypad => "\x1b="; // ESC =>     Application Keypad (DECKPAM).
        public static string NormalKeypad => "\x1b>";      // ESC >     Normal Keypad (DECKPNM), VT100.

        public static string
            CursorLowerLeft =
                "\x1bF"; // ESC F     Cursor to lower left corner of screen.This is enabled by the hpLowerleftBugCompat resource.

        public static string FullReset => "\x1bc"; // ESC c     Full Reset (RIS), VT100.

        public static string
            MemoryLock => "\x1bl"; // ESC l     Memory Lock (per HP terminals).  Locks memory above the cursor.

        public static string MemoryUnlock => "\x1bm"; // ESC m     Memory Unlock(per HP terminals).
        public static string LS2 => "\x1bn";          // ESC n     Invoke the G2 Character Set as GL(LS2).
        public static string LS3 => "\x1bo";          // ESC o     Invoke the G3 Character Set as GL(LS3).
        public static string LS3R => "\x1b|";         // ESC |     Invoke the G3 Character Set as GR(LS3R).
        public static string LS2R => "\x1b}";         // ESC }     Invoke the G2 Character Set as GR(LS2R).
        public static string LS1R => "\x1b~";         // ESC ~     Invoke the G1 Character Set as GR(LS1R), VT100.
    }

    public static class SessionManagement
    {
        /// <summary>
        ///     Enable Session - DECES
        /// </summary>
        public static string EnableSession => "\x1b[&x"; // CSI & x
        
        /// <summary>
        ///     Session Page Memory Allocation - DECSPMA
        ///     n1..n4 => Pages per session
        /// </summary>
        public static string SessionPageMemoryAllocation(int n1, int n2, int n3, int n4) // CSI Pn1; Pn2; Pn3; Pn4; , x
            => $"\x1b[{n1};{n2};{n3};{n4},x";

        /// <summary>
        ///     Update Session DECUS
        /// </summary>
        /// <param name="n">
        ///     1 Only when active
        ///     2 When available
        ///     3 At regular intervals
        /// </param>
        public static string UpdateSession(int n)   // CSI Ps , y  
            => $"\x1b[{n},y";
    }

    public static class WindowManagement
    {
        public static string AutoResizeEnable => "\x1b[?98h"; // CSI ? 98 h - DECARSM
        public static string AutoResizeDisable => "\x1b[?98l"; // CSI ? 98 l - DECARSM

        public static string HorizontalCursorCouplingEnable => "\x1b[?60h"; // CSI ? 60 h - DECHCCM
        public static string HorizontalCursorCouplingDisable => "\x1b[?60l"; // CSI ? 60 l - DECHCCM

        public static string PageCursorCouplingEnable => "\x1b[?64h";  // CSI ? 64 h - DECPCCM
        public static string PageCursorCouplingDisable => "\x1b[?64l"; // CSI ? 64 l - DECPCCM

        public static string VerticalCursorCouplingEnable => "\x1b[?61h";  // CSI ? 61 h - DECVCCM
        public static string VerticalCursorCouplingDisable => "\x1b[?61l"; // CSI ? 61 l - DECVCCM
    }

    public static class DisplayExtent
    {
        /// <summary>
        ///     Request Display Extent - DECRQDE
        /// </summary>
        public static string RequestDisplayedExtent => "\x1b[\"v";  // CSI " v

        /// <summary>
        ///     Report Displayed Extent - DECRPDE
        /// </summary>
        public static string ReportDisplayedExtent(int h, int w, int ml, int mt, int mp) => $"\x1b[{h};{w};{ml};{mt};{mp}\"w"; // CSI Ph;Pw;Pml;Pmt;Pmp; " w

        /// <summary>
        ///     Select Number of lines per Screen - DECSNLS
        /// </summary>
        public static string SelectNumberOfLinesPerScreen(int n) => $"\x1b[{n}*|";  // CSI Pn * | 
    }

    public static class Window
    {
        public static string FramedWindowsModeEnabled => "\x1b[?111h";  // CSI ? 111 h - DECFWM 
        public static string FramedWindowsModeDisabled => "\x1b[?111l"; // CSI ? 111 l - DECFWM
        
        public static string ReviewPreviousLinesEnabled => "\x1b[?111h";  // CSI ? 112 h - DECRPL 
        public static string ReviewPreviousLinesDisabled => "\x1b[?111l"; // CSI ? 112 l - DECRPL

        /// <summary>
        ///     Set icon name - DECSIN
        /// </summary>
        /// <param name="name">Up to 12 characters</param>
        public static string SetIconName(string name) => $"\x1b]2L;{name}\x1b\\"; // OSC 2 D...D ST
        
        /// <summary>
        ///     Set Window Title - DECSWT
        /// </summary>
        /// <param name="title">Up to 30 characters</param>
        public static string SetWindowTitle(string title) => $"\x1b]21;{title}\x1b\\"; // OSC 2 D...D ST
    }

    public static class Pan
    {
        public static string PanUp(int lines) => $"\x1b[{lines}T"; // CSI P1 T
        public static string PanDown(int lines) => $"\x1b[{lines}S"; // CSI P1 S
    }

    public static class AudibleAttributes
    {
        /// <summary>
        ///     Set margin-bell volume (DECSMBV), VT520. 
        /// </summary>
        /// <param name="vol">
        ///     Ps => 0, 1        ⇒  off.
        ///     Ps => 2, 3, 4     ⇒  low.
        ///     Ps => 5, 6, 7, 8  ⇒  high.
        /// </param>
        public static string MarginBellVolume(string vol) => $"\x1b[{vol} u"; // CSI Ps SP u
        
        /// <summary>
        ///     Set warning-bell volume (DECSWBV), VT520. 
        /// </summary>
        /// <param name="vol">
        ///     Ps => 0, 1        ⇒  off.
        ///     Ps => 2, 3, 4     ⇒  low.
        ///     Ps => 5, 6, 7, 8  ⇒  high.
        /// </param>
        public static string WarningBellVolume(string vol) => $"\x1b[{vol} t"; // CSI Ps SP t

        /// <summary>
        ///     Play Sound - DECPS
        /// </summary>
        /// <param name="vol">0..7</param>
        /// <param name="duration">0..255 1/32nd of a second</param>
        /// <param name="note">
        ///     Note Selection
        ///     0  silent   8  G5       16 D#6      24 B6
        ///     1  C5       9  G#5      17 E6       25 C7
        ///     2  C#5      10 A5       18 F6
        ///     3  D5       11 A#5      19 F#6
        ///     4  D#5(Eb)  12 B5       20 G6
        ///     5  E5       13 C6       21 G#6
        ///     6  F5       14 C#6      22 A6
        ///     7  F#5      15 D6       23 A#6
        /// </param>
        public static string PlaySound(int vol, int duration, int note)
        {
            return $"\x1b[{vol};{duration};{note},~";
        }
    }
    
    /// <summary>
    ///     https://en.wikipedia.org/wiki/ANSI_escape_code#SGR
    /// </summary>
    public static class Color
    {
        public static string Fore(int r, int g, int b) => $"\x1b[38;2;{r};{g};{b}m";
        public static string Fore(System.Drawing.Color color) => $"\x1b[38;2;{color.R};{color.G};{color.B}m";
        public static string Fore(int color256) => $"\x1b[38;5;{color256}m";
        public static string Fore(Color4Bit color) => $"\x1b[{(int)color + 30}m";
        public static string ForeDefault => "\x1b[39m";

        public static string Back(int r, int g, int b) => $"\x1b[48;2;{r};{g};{b}m";
        public static string Back(int color256) => $"\x1b[48;5;{color256}m";
        public static string Back(Color4Bit color) => $"\x1b[{(int)color + 40}m";
        public static string BackDefault => "\x1b[49m";

        public static string Underline(int r, int g, int b) => $"\x1b[58;2;{r};{g};{b}m";
        public static string UnderlineDefault => "\x1b[59m";

        /// <summary>
        ///     Assign Color - DECAC
        /// </summary>
        /// <param name="n1">1 = Normal text, 2 = Window frame</param>
        /// <param name="foreColor">0..15</param>
        /// <param name="backColor">0..15</param>
        public static string AssignColor(int n1, int foreColor, int backColor) => $"\x1b[{n1};{foreColor};{backColor},|";

        /// <summary>
        ///     Alternative Text Color - DECATC
        /// </summary>
        /// <param name="textAttribute">
        ///     0 = Normal text
        ///     1 = Bold
        ///     2 = Reverse
        ///     3 = Underline
        ///     4 = Blink
        ///     5 = Bold reverse
        ///     6 = Bold underline
        ///     7 = Bold blink
        ///     8 = Reverse Underline
        ///     9 = Reverse Blink
        ///     10 = Underline blink
        ///     11 = Bold reverse underline
        ///     12 = Bold reverse blink
        ///     13 = Bold underline blink
        ///     14 = Reverse underline blink
        ///     15 = Bold reverse underline blink
        /// </param>
        /// <param name="foreColor">0-15 color map entry</param>
        /// <param name="backColor">0-15 color map entry</param>
        public static string AlternativeTextColor(int textAttribute, int foreColor, int backColor) => $"\x1b[{textAttribute};{foreColor};{backColor},}}"; // CSI Ps1 Ps2 Ps3 , }

        public static string AlternateTextColorBlinkModeSet => "\x1b[?115h"; // CSI ? 115 h - DECATCBM
        public static string AlternateTextColorBlinkModeClear => "\x1b[?115l"; // CSI ? 115 l - DECATCBM
        
        public static string AlternateTextColorUnderlineModeSet => "\x1b[?114h";   // CSI ? 114 h - DECATCUM
        public static string AlternateTextColorUnderlineModeClear => "\x1b[?114l"; // CSI ? 114 l - DECATCUM

        public static string AlternateTextColorBoldBlinkModeSet => "\x1b[?116h";   // CSI ? 116 h - DECBBSM
        public static string AlternateTextColorBoldBlinkModeClear => "\x1b[?116l"; // CSI ? 116 l - DECBBSM

        public static string ColorTableRequest(int Pu) => $"\x1b[2;{Pu}$u"; // CSI 2 ; Pu $ u - DECCTR 
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Pc">Color number</param>
        /// <param name="Pu">Color coordinate system (1 = HLS, 2 = RGB)</param>
        /// <param name="Px">Hue = 0..360, Red=0..100</param>
        /// <param name="Py">Lightness = 0..100, Green = 0..100</param>
        /// <param name="Pz">Saturation = 0..100, Blue = 0..100</param>
        public static string ColorTableResponse(int Pc, int Pu, int Px, int Py, int Pz)
        {
            return $"\x1bP2$s{Pc};{Pu};{Px};{Py};{Pz}\x1b\\";
            // DCS 2 $ s D...D ST - DECCTR 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Pc">Color number</param>
        /// <param name="Pu">Color coordinate system (1 = HLS, 2 = RGB)</param>
        /// <param name="Px">Hue = 0..360, Red=0..100</param>
        /// <param name="Py">Lightness = 0..100, Green = 0..100</param>
        /// <param name="Pz">Saturation = 0..100, Blue = 0..100</param>
        public static string ColorTableRestore(int Pc, int Pu, int Px, int Py, int Pz) => "";                       // DCS 2 $ p D...D ST - DECCTR 

        public static string EraseColorModeSet => $"\x1b[?117h"; // CSI ? 117 h - DECECM
        public static string EraseColorModeClear => $"\x1b[?117l"; // CSI ? 117 l - DECECM

        /// <summary>
        ///     Select Color Look-Up Table 
        /// </summary>
        /// <param name="Ps">
        ///     0 Mono
        ///     1 Alternate color (use text attributes)
        ///     2 Alternate color
        ///     3 ANSI SGR color          
        /// </param>
        public static string SelectColorLookupTable(int Ps) => $"\x1b[{Ps}){{"; //  CSI Ps ) { - DECSTGLT 
    }

    public static class VisualAttributes
    {
        public static string DoubleHeightLineTop => "\x1b#3";    // ESC # 3 - DECDHL 
        public static string DoubleHeightLineBottom => "\x1b#4"; // ESC # 3 - DECDHL
        public static string SingleWidthLine => "\x1b#5";        //ESC # 5 - DECSWL
        public static string DoubleWidthLine => "\x1b#6";        //ESC # 6 - DECDWL

        public static string ScreenModeLight => "\x1b[?5h"; // CSI ? 5 h - DECSCNM 
        public static string ScreenModeDark => "\x1b[?5l"; // CSI ? 5 l - DECSCNM 

        /// <summary>
        ///     Select Graphic Rendition
        /// </summary>
        /// <param name="attr">
        ///     0 All attributes off
        ///     1 Bold
        ///     4 Underline
        ///     5 Blink
        ///     7 Negative (reverse) image 
        ///     22 Normal (bold off)
        ///     24 Underline off
        ///     25 Blinking off
        ///     27 Positive image (negative off)
        /// </param>
        /// <param name="col">
        ///     30 Foreground Color 0           Default Color Black
        ///     31 Foreground Color 1           Default Color Red
        ///     32 Foreground Color 2           Default Color Green
        ///     33 Foreground Color 3           Default Color Yellow
        ///     34 Foreground Color 4           Default Color Blue
        ///     35 Foreground Color 5           Default Color Magenta
        ///     36 Foreground Color 6           Default Color Cyan
        ///     37 Foreground Color 7           Default Color White
        ///     39 Default Foreground Color     Default color White
        ///     40 Background Color 0           Default Color Black
        ///     41 Background Color 1           Default Color Red
        ///     42 Background Color 2           Default Color Green
        ///     43 Background Color 3           Default Color Yellow
        ///     44 Background Color 4           Default Color Blue
        ///     45 Background Color 5           Default Color Magenta
        ///     46 Background Color 6           Default Color Cyan
        ///     47 Background Color 7           Default Color White
        ///     49 Default Background Color     Default Color Black
        /// </param>
        public static string SelectGraphicRendition(int attr, int col) => $"\x1b[{attr};{col}m"; // CSI Ps ; Ps m - SGR
    }

    public static class EditingControlFunctions
    {
        public static string DeleteCharacter(int n = 1) => $"\x1b[{n}P"; // CSI Pn P - DCH
        public static string DeleteColumn(int n = 1) => $"\x1b[{n}'~";   // CSI Pn ’ ~ - DECDC
        public static string DeleteLine(int n = 1) => $"\x1b[{n}M";      // CSI Pn M - DL
        public static string EraseCharacter(int n = 1) => $"\x1b[{n}X";  // CSI Pn X - ECH
        
        /// <summary>
        ///     Erase in Display
        /// </summary>
        /// <param name="n">
        ///     0 Cursor to end of display
        ///     1 Top of display through cursor
        ///     2 Top to bottom of display
        /// </param>
        public static string EraseInDisplay(int n) => $"\x1b[{n}J";  // CSI Pn J - ED
        
        /// <summary>
        ///     Erase in Line 
        /// </summary>
        /// <param name="n">
        ///     0 Cursor to end of line
        ///     1 Start of line through cursor
        ///     2 Start to end of line
        /// </param>
        public static string EraseInLine(int n) => $"\x1b[{n}K"; // CSI Pn K - EL

        public static string InsertCharacter(int n = 1) => $"\x1b[{n}@"; // CSI Pn @ - ICH
        public static string InsertColumn(int n = 1) => $"\x1b[{n}'}}";  // CSI Pn ' } - DECIC
        public static string InsertLine(int n = 1) => $"\x1b[{n}L";      // CSI Pn L - IL
        
        public static string InsertMode => $"\x1b[4h"; // CSI 4 h - IRM
        public static string ReplaceMode => $"\x1b[4l"; // CSI 4 l - IRM

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Ps">
        ///     0 DECSED and DECSEL can erase characters.
        ///     1 DECSED and DECSEL cannot erase characters.
        ///     2 Same as 0.
        /// </param>
        public static string SelectCharacterAttribute1(int Ps) => $"\x1b[{Ps}\"q"; // CSI Ps " q - DECSCA

        /// <summary>
        ///     Select erase in Display 
        /// </summary>
        /// <param name="Ps">
        ///     Ps Erase from . . .
        ///     0 Cursor to end of display
        ///     1 Top of display through cursor
        ///     2 Top to bottom of display
        /// </param>
        public static string SelectiveEraseInDisplay(int Ps) => $"\x1b[?{Ps}J"; // CSI ? J - DECSED
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Ps">
        ///     Ps Erase from . . .
        ///     0 Cursor to end of line
        ///     1 Start of line through cursor
        ///     2 Start to end of line
        /// </param>
        public static string SelectiveEraseInLine(int Ps) => $"\x1b[?{Ps}K"; // CSI ? K - DECSEL
    }

    public static class RectangleAreaProcessing
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="psn">
        ///     0 Off
        ///     1 Bold Pt Top line
        ///     4 Underline Pl Left column
        ///     5 Blink Pb Bottom line
        ///     7 Negative image Pr Right column
        ///     22 No bold
        ///     24 No underline
        ///     25 No blink
        ///     27 Positive image
        /// </param>
        public static string ChangeAttributeInRectangle(int top, int left, int bottom, int right, int psn)
        {
            return "\x1b[{top};{left};{bottom};{right};{psn}$r";
            // CSI Pt;Pl;Pb;Pr; Ps1;...Psn $ r - DECCARA
        }

        public static string CopyRectangularArea(
            int sourceTop, 
            int sourceLeft,
            int sourceBottom,
            int sourceRight,
            int sourcePageNumber, 
            int top, 
            int left, 
            int pageNumber)
        {
            return $"\x1b[{sourceTop};{sourceLeft};{sourceBottom};{sourceRight};{sourcePageNumber};{top};{left};{pageNumber}$v";
            // CSI Pts;Pls;Pbs;Prs;Pps;Ptd;Pld;Ppd$v - DECCRA
        }

        public static string EraseRectangularArea(int top, int left, int bottom, int right) => $"\x1b[{top};{left};{bottom};{right}$z"; // CSI Pt;Pl;Pb;Pr $ z - DECERA
        public static string FillRectangleArea(char ch, int top, int left, int bottom, int right) => $"\x1b[{ch};{top};{left};{bottom};{right}$x"; // CSI Pch; Pt;Pl;Pb;Pr $ x - DECRARA

        /// <summary>
        /// 
        /// </summary>
        /// <param name="psn">
        ///     Ps Reverse Attributes
        ///     0 All
        ///     1 Bold
        ///     4 Underline
        ///     5 Blink
        ///     7 Negative image
        /// </param>
        public static string ReverseAttributeInRectangle(int top, int left, int bottom, int right, int psn)
        {
            return $"\x1b[{top};{left};{bottom};{right};{psn}$t";
            // CSI Pt;Pl;Pb;Pr;Psn $ t - DECRARA
        }

        public static string SelectAttributeChangeExtent(int n) => $"\x1b[{n}*x"; // CSI Ps * x - DECSACE

        public static string SelectiveEraseRectangularArea(int top, int left, int bottom, int right) => $""; // CSI Pt;Pl;Pb;Pr $ { - DECSERA
    }

    public static class TextProcessing
    {
        // TODO: https://vt100.net/dec/ek-vt520-rm.pdf
        // DECAWM
    }
    public static class GraphicCharacterSets
    {
        // TODO: https://vt100.net/dec/ek-vt520-rm.pdf
        // DECAUPSS
    }
    
    public static class KeyboardProcessing
    {
        // TODO: https://vt100.net/dec/ek-vt520-rm.pdf
        // DECARM
    }

    public enum Color4Bit
    {
        Black = 0,
        Red = 1,
        Green = 2,
        Yellow = 3,
        Blue = 4,
        Magenta = 5,
        Cyan = 6,
        White = 7,
        BrightBlack = 60,
        BrightRed = 61,
        BrightGreen = 62,
        BrightYellow = 63,
        BrightBlue = 64,
        BrightMagenta = 65,
        BrightCyan = 66,
        BrightWhite = 67,
    }

    public static class InputSeq
    {
        public static string UpArrow => "\x1b[A";
        public static string DownArrow => "\x1b[B";
        public static string RightArrow => "\x1b[C";
        public static string LeftArrow => "\x1b[D";
        public static string HomeArrow => "\x1b[H";
        public static string EndArrow => "\x1b[F";

        public static string CtrlUpArrow => "\x1b[1;5A";
        public static string CtrlDownArrow => "\x1b[1;5B";
        public static string CtrlRightArrow => "\x1b[1;5C";
        public static string CtrlLeftArrow => "\x1b[1;5D";

        public static string Insert => "\x1b[2~";
        public static string Delete => "\x1b[3~";
        public static string PageUp => "\x1b[5~";
        public static string PageDown => "\x1b[6~";

        public static string F1 => "\x1bOP";
        public static string F2 => "\x1bOQ";
        public static string F3 => "\x1bOR";
        public static string F4 => "\x1bOS";
        public static string F5 => "\x1b[15~";
        public static string F6 => "\x1b[17~";
        public static string F7 => "\x1b[18~";
        public static string F8 => "\x1b[19~";
        public static string F9 => "\x1b[20~";
        public static string F10 => "\x1b[21~";
        public static string F11 => "\x1b[23~";
        public static string F12 => "\x1b[24~";
    }

    public static class Mode
    {
        public static string ScreenAlt => "\x1b[?1049h";
        public static string ScreenNormal => "\x1b[?1049l";

        public static string LocatorReportingCells => "\x1b[1;2'z";
        public static string StopLocatorReporting => "\x1b['z";

        public static string SendMouseXY => "\x1b[?1000h";
        public static string MouseTrackingHilite => "\x1b[?1001h";
        public static string MouseTrackingCell => "\x1b[?1002h";
        public static string MouseTrackingAll => "\x1b[?1003h";
        public static string MouseTrackingFocus => "\x1b[?1004h";
        public static string MouseTrackingUtf8 => "\x1b[?1005h";
        public static string MouseTrackingSGR => "\x1b[?1006h";

        public static string StartTracking => "\x1b[?1003h\x1b[?1004h\x1b[?1006h";
        public static string StopTracking => "\x1b[?1003l\x1b[?1004l\x1b[?1006l";

        public static string StopMouseXY => "\x1b[?1000l";
        public static string StopMouseTrackingHilite => "\x1b[?1001l";
        public static string StopMouseTrackingCell => "\x1b[?1002l";
        public static string StopMouseTrackingAll => "\x1b[?1003l";
    }

    public static class Func
    {
        public static string SendDeviceAttributesPrimary => "\x1b[c";
        public static string SendDeviceAttributesSecondary => "\x1b[>c";
        public static string SendDeviceAttributesTertiary => "\x1b[=c";

        public static string DeviceStatusReport => "\x1b[5n";
        public static string ReportCursorPosition => "\x1b[6n";
    }

    public static class SpecialKey
    {
        // ReSharper disable InconsistentNaming
        public static char ACK => '\x06'; // Acknowledge
        public static char BEL => '\x07'; // Ctrl-G Bell
        public static char BS => '\x08';  // Ctrl-H Backspace
        public static char CAN => '\x18'; // Cancel
        public static char CR => '\x0D';  // Ctrl-M Carriage Return
        public static char DC1 => '\x11'; // Device Control 1
        public static char DC2 => '\x12'; // Device Control 2
        public static char DC3 => '\x13'; // Device Control 3
        public static char DC4 => '\x14'; // Device Control 4
        public static char DEL => '\x7F'; // Del
        public static char DLE => '\x10'; // Data Link Escape
        public static char EM => '\x05';  // End of Medium
        public static char ENQ => '\x05'; // Ctrl-E Enquire
        public static char EOT => '\x04'; // End of Transmission
        
        public static char ESC => '\x1b'; // Escape

        public static char ETB => '\x17'; // End of Transmission Block
        public static char ETX => '\x03'; // End of Text
        public static char FF => '\x0C';  // Ctrl-L Form Feed
        public static char FS => '\x1C';  // File Separator
        public static char GS => '\x1D';  // Group Separator
        public static char LF => '\x0A';  // Ctrl-J Line Feed
        public static char NAK => '\x15'; // Negative Acknowledge
        public static char RS => '\x1E';  // Record Separator
        public static char SI => '\x0F';  // Ctrl-O Shift In
        public static char SO => '\x0E';  // Ctrl-N Shift Out
        public static char SOH => '\x01'; // Start of Heading
        public static char SP => '\x20';  // Space
        public static char STX => '\x02'; // Start of Text
        public static char SUB => '\x1A'; // Substitute
        public static char SYN => '\x16'; // Synchronous Idle
        public static char HTS => '\x09'; // Ctrl-I Tab
        public static char US => '\x1F';  // Unit Separator

        public static char VT => '\x0B'; // Ctrl-K Vertical Tab
        // ReSharper restore InconsistentNaming
    }





    public static class Cursor
    {
        /// <summary>
        ///     Cursor Position[row; column] (default => [1,1]) (CUP).
        /// </summary>
        public static string Set(int row, int col) => $"\x1b[{row};{col}H"; // CUP cursor position, // CSI Ps; Ps H    


        public static string Up(int n = 1) => n == 1 ? "\x1b[A" : $"\x1b[{n}A";    // CUU cursor up
        public static string Down(int n = 1) => n == 1 ? "\x1b[B" : $"\x1b[{n}B";  // CUD cursor down
        public static string Right(int n = 1) => n == 1 ? "\x1b[C" : $"\x1b[{n}C"; // CUF cursor forwards
        public static string Left(int n = 1) => n == 1 ? "\x1b[D" : $"\x1b[{n}D";  // CUB cursor backwards

        public static string Show => "\x1b[?25h";
        public static string Hide => "\x1b[?25l";

        /// <summary>
        ///     Set cursor style (DECSCUSR), VT520. 
        /// </summary>
        /// <param name="Ps">
        ///     <para>0  ⇒  blinking block.</para>
        ///     <para>1  ⇒  blinking block (default).</para>
        ///     <para>2  ⇒  steady block.</para>
        ///     <para>3  ⇒  blinking underline.</para>
        ///     <para>4  ⇒  steady underline.</para>
        ///     <para>5  ⇒  blinking bar, xterm.</para>
        ///     <para>6  ⇒  steady bar, xterm.</para>
        /// </param>
        public static string SetStyle(int style) => $"\x1b[{style} q"; // CSI Ps SP q

        public static string SaveCursor => "\x1b[s"; // CSI s     Save cursor, available only when DECLRMM is disabled (SCOSC, also ANSI.SYS).
        public static string RestoreCursor => "\x1b[u"; // CSI u     Restore cursor (SCORC, also ANSI.SYS).

    }

    public static class Graphics
    {
        public static string Reset => "\x1b[0m";            // Reset / Normal
        public static string Bold => "\x1b[1m";             // Bold or increased intensity
        public static string Faint => "\x1b[2m";            // Faint or decreased intensity
        public static string Italic => "\x1b[3m";           // Italic Not widely supported
        public static string Underline => "\x1b[4m";        // Underline
        public static string SlowBlink => "\x1b[5m";        // Slow Blink  less than 150 per minute
        public static string FastBlink => "\x1b[6m";        // Rapid Blink
        public static string Reverse => "\x1b[7m";          // Reverse video
        public static string Conceal => "\x1b[8m";          // Conceal aka Hide
        public static string Strike => "\x1b[9m";           // Crossed-out	aka Strike
        public static string PrimaryFont => "\x1b[10m";     // Primary(default) font
        public static string AltFont11 => "\x1b[11m";       // Alternative font
        public static string DoubleUnderline => "\x1b[21m"; // Doubly underline
        public static string NormalColor => "\x1b[22m";     // Normal color or intensity
        public static string ItalicOff => "\x1b[23m";       // Not italic
        public static string UnderlineOff => "\x1b[24m";    // Underline off
        public static string BlinkOff => "\x1b[25m";        // Blink off
        public static string ReverseOff => "\x1b[27m";      // Reverse off
        public static string ConcealOff => "\x1b[28m";      // Conceal off
        public static string StrikeOff => "\x1b[29m";       // Strike off
        public static string Overlined => "\x1b[53m";       // Overlined
        public static string OverlinedOff => "\x1b[55m";    // Overlined off
        public static string SuperScript => "\x1b[73m";     // superscript
        public static string SubScript => "\x1b[74m";       // subscript
    }

    public static class Scroll
    {
        public static string Set(int topRow, int bottomRow) => $"\x1b[{topRow};{bottomRow}r";
        public static string Clear => "\x1b[r";
        /// <summary>
        ///     Scroll up [count] lines(default => 1) (SD), VT420.
        /// </summary>
        public static string Up(int count) => $"\x1b[{count}S"; // CSI Ps S
        /// <summary>
        ///     Scroll down [count] lines(default => 1) (SD), VT420.
        /// </summary>
        public static string Down(int count) => $"\x1b[{count}T"; // CSI Ps T    

    }

    public static class Tab
    {
        public static string Set => Control.HTS;
        public static string Clear => "\x1b[g";
        public static string ClearAll => "\x1b[3g";
    }

    /// <summary>
    ///     Device Template sequences.
    ///     All values require a value replaced in the string
    /// </summary>
    public static class Device
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        ///     Request Status String(DECRQSS), VT420 and up.
        /// </summary>
        /// <param name="Pt">
        ///     m       ⇒  SGR
        ///     " p     ⇒  DECSCL
        ///     SP q    ⇒  DECSCUSR
        ///     " q     ⇒  DECSCA
        ///     r       ⇒  DECSTBM
        ///     s       ⇒  DECSLRM
        ///     t       ⇒  DECSLPP
        ///     $ |     ⇒  DECSCPP
        ///     * |     ⇒  DECSNLS
        ///     xterm responds with DCS 1 $ r Pt ST for valid requests,
        ///     replacing the Pt with the corresponding CSI string,
        ///     or DCS 0 $ r Pt ST for invalid requests.
        /// </param>
        public static string DCS_RequestStatusString(string Pt) => $"\x1bP$q{Pt}\x1b\\"; // DCS $ q Pt ST 

        /// <summary>
        ///     Request resource values (XTGETXRES), xterm.  
        /// </summary>
        /// <param name="Pt">
        ///     list of names encoded in hexadecimal
        ///     (2 digits per character) separated by ; which correspond to xterm resource names.
        ///     Only boolean, numeric and string resources are supported by this query.
        ///     xterm responds with DCS 1 + R Pt ST for valid requests,
        ///     adding to Pt an => , and the value of the corresponding resource that xterm is using,
        ///     or DCS 0 + R Pt ST for invalid requests.
        ///     The strings are encoded in hexadecimal (2 digits per character).
        /// </param>
        public static string DCS_RequestResourceValue(string Pt) => $"\x1bP+Q{Pt}\x1b\\"; //DCS + Q Pt ST

        /// <summary>
        ///     Set Termcap/Terminfo Data (XTSETTCAP), xterm.
        /// </summary>
        /// <param name="Pt">
        ///     name to use for retrieving data from the
        ///     terminal database.  The data will be used for the "tcap" keyboard configuration's function- and special-keys, as well as
        ///     by the Request Termcap/Terminfo String control.
        /// </param>
        public static string DCS_SetTermInfoData(string Pt) => $"\x1bP+p{Pt}\x1b\\"; //DCS + p Pt ST


        /// <summary>
        ///     Request Termcap/Terminfo String (XTGETTCAP), xterm.
        ///     DCS + q Pt ST
        /// </summary>
        /// <param name="Pt">
        ///     Names encoded in hexadecimal (2 digits per character) separated by ; which correspond to termcap or terminfo key names.
        ///     A few special features are also recognized, which are not key names:
        ///     o   Co for termcap colors (or colors for terminfo colors), and
        ///     o   TN for termcap name (or name for terminfo name).
        ///     o   RGB for the ncurses direct-color extension.
        ///     Only a terminfo name is provided, since termcap applications cannot use this information. xterm responds with
        ///         DCS 1 + r Pt ST for valid requests,
        ///     adding to Pt an => , and the value of the corresponding string that xterm would send, or
        ///         DCS 0 + r Pt ST for invalid requests.
        ///     The strings are encoded in hexadecimal (2 digits per character).
        /// </param>
        public static string RequestTermInfoString(string Pt)
        {
            var str = Convert.ToHexString(Encoding.UTF8.GetBytes(Pt));
            return $"\x1bP+q{str}\x1b\\";
        }

        // ReSharper restore InconsistentNaming
    }

    public static class ControlSeq
    {
        // ReSharper disable InconsistentNaming

        public static string CSI_ => "\x1b["; // 
        public static string ST_ => "\x1b\\"; // String terminator

        /// <summary>
        ///     Clear screen
        /// </summary>
        public static string ClearScreen => "\x1b[2J";

        /// <summary>
        ///     Initiate highlight mouse tracking (XTHIMOUSE), xterm.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="firstRow"></param>
        /// <param name="lastRow"></param>
        /// <returns></returns>
        public static string CSI_InitHighlightMouseTracking(string func, string startX, string startY, string firstRow,
                                                            string lastRow)
        {
            return $"\x1b[{func};{startX};{startY};{firstRow};{lastRow}T"; // CSI Ps ; Ps ; Ps ; Ps ; Ps T
        }

        /// <summary>
        ///     Send Device Attributes (Primary DA).
        /// </summary>
        /// <param name="Ps">
        ///     Ps => 0  or omitted ⇒  request attributes from terminal.The response depends on the decTerminalID resource setting.
        ///     ⇒  CSI? 1 ; 2 c("VT100 with Advanced Video Option")
        ///     ⇒  CSI? 1 ; 0 c("VT101 with No Options")
        ///     ⇒  CSI? 4 ; 6 c("VT132 with Advanced Video and Graphics")
        ///     ⇒  CSI? 6 c("VT102")
        ///     ⇒  CSI? 7 c("VT131")
        ///     ⇒  CSI? 12 ; Ps c("VT125")
        ///     ⇒  CSI? 62 ; Ps c("VT220")
        ///     ⇒  CSI? 63 ; Ps c("VT320")
        ///     ⇒  CSI? 64 ; Ps c("VT420")
        ///     The VT100-style response parameters do not mean anything by themselves.VT220(and higher) parameters do, telling the host what features the terminal supports:
        ///     Ps => 1  ⇒  132-columns.
        ///     Ps => 2  ⇒  Printer.
        ///     Ps => 3  ⇒  ReGIS graphics.
        ///     Ps => 4  ⇒  Sixel graphics.
        ///     Ps => 6  ⇒  Selective erase.
        ///     Ps => 8  ⇒  User-defined keys.
        ///     Ps => 9  ⇒  National Replacement Character sets.
        ///     Ps => 15  ⇒  Technical characters.
        ///     Ps => 16  ⇒  Locator port.
        ///     Ps => 17  ⇒  Terminal state interrogation.
        ///     Ps => 18  ⇒  User windows.
        ///     Ps => 21  ⇒  Horizontal scrolling.
        ///     Ps => 22  ⇒  ANSI color, e.g., VT525.
        ///     Ps => 28  ⇒  Rectangular editing.
        ///     Ps => 29  ⇒  ANSI text locator (i.e., DEC Locator mode).
        ///     XTerm supports part of the User windows feature, providing a single page(which corresponds to its visible window).  
        ///     Rather than resizing the font to change the number of lines/columns
        ///     in a fixed-size display, xterm uses the window extension controls(DECSNLS, DECSCPP, DECSLPP)
        ///     to adjust its visible window's size.  The "cursor coupling" controls (DECHCCM, DECPCCM, DECVCCM) are ignored.        
        /// </param>
        public static string CSI_SendDeviceAttrPrimary(string Ps) => $"\x1b[{Ps}c"; // CSI Ps c  

        /// <summary>
        ///     Send Device Attributes (Secondary DA).
        /// </summary>
        /// <param name="Ps">
        ///     Ps => 0  or omitted ⇒  request the terminal's identification code.
        ///     The response depends on the decTerminalID resource setting.  It should apply only to VT220 and up, but xterm extends this to VT100.
        ///     ⇒  CSI  > Pp ; Pv ; Pc c
        ///     where Pp denotes the terminal type
        ///         Pp => 0  ⇒  "VT100".
        ///         Pp => 1  ⇒  "VT220".
        ///         Pp => 2  ⇒  "VT240" or "VT241".
        ///         Pp => 18  ⇒  "VT330".
        ///         Pp => 19  ⇒  "VT340".
        ///         Pp => 24  ⇒  "VT320".
        ///         Pp => 32  ⇒  "VT382".
        ///         Pp => 41  ⇒  "VT420".
        ///         Pp => 61  ⇒  "VT510".
        ///         Pp => 64  ⇒  "VT520".
        ///         Pp => 65  ⇒  "VT525".
        ///     and Pv is the firmware version (for xterm, this was originally the XFree86 patch number, starting with 95).
        ///     In a DEC terminal, Pc indicates the ROM cartridge registration number and is always zero.
        /// </param>
        public static string CSI_SendDeviceAttrSecondary(string Ps) => $"\x1b[{Ps}>c"; // CSI > Ps c  

        /// <summary>
        ///     Horizontal and Vertical Position[row; column] (default => [1,1]) (HVP). 
        /// </summary>
        public static string HorzVertPosition(string row, string col) => $"\x1b[{row};{col}f"; // CSI Ps; Ps f 

        /// <summary>
        ///     Tab Clear 
        /// </summary>
        /// <param name="Ps">
        ///     Ps => 0  ⇒  Clear     Current Column(default).
        ///     Ps => 3  ⇒  Clear All.
        /// </param>
        public static string TabClear(string Ps) => $"\x1b[{Ps}g"; // CSI Ps g  Tab Clear (TBC). 

        /// <summary>
        ///     Set Mode 
        /// </summary>
        /// <param name="Ps">
        ///     Ps => 2  ⇒  Keyboard  Action Mode(KAM).
        ///     Ps => 4  ⇒  Insert    Mode(IRM).
        ///     Ps => 1 2  ⇒  Send/receive(SRM).
        ///     Ps => 2 0  ⇒  Automatic Newline(LNM).
        /// </param>
        public static string SetMode(string Ps) => $"\x1b[{Ps}h"; // CSI Pm h  Set Mode (SM).

        /// <summary>
        ///     DEC Private Mode Set(DECSET). 
        /// </summary>
        /// <param name="Ps">
        ///     Ps => 1  ⇒  Application Cursor Keys(DECCKM), VT100.
        ///     Ps => 2  ⇒  Designate USASCII for character sets G0-G3 (DECANM), VT100, and set VT100 mode.
        ///     Ps => 3  ⇒  132 Column Mode (DECCOLM), VT100.
        ///     Ps => 4  ⇒  Smooth (Slow) Scroll (DECSCLM), VT100.
        ///     Ps => 5  ⇒  Reverse Video (DECSCNM), VT100.
        ///     Ps => 6  ⇒  Origin Mode (DECOM), VT100.
        ///     Ps => 7  ⇒  Auto-Wrap Mode (DECAWM), VT100.
        ///     Ps => 8  ⇒  Auto-Repeat Keys (DECARM), VT100.
        ///     Ps => 9  ⇒  Send Mouse X & Y on button press.  See the section Mouse Tracking.This is the X10 xterm mouse protocol.
        ///     Ps => 10  ⇒  Show toolbar (rxvt).
        ///     Ps => 12  ⇒  Start blinking cursor (AT&T 610).
        ///     Ps => 13  ⇒  Start blinking cursor(set only via resource or menu).
        ///     Ps => 14  ⇒  Enable XOR of blinking cursor control sequence and menu.
        ///     Ps => 18  ⇒  Print Form Feed(DECPFF), VT220.
        ///     Ps => 19  ⇒  Set print extent to full screen(DECPEX), VT220.
        ///     Ps => 25  ⇒  Show cursor(DECTCEM), VT220.
        ///     Ps => 30  ⇒  Show scrollbar(rxvt).
        ///     Ps => 35  ⇒  Enable font-shifting functions(rxvt).
        ///     Ps => 38  ⇒  Enter Tektronix mode(DECTEK), VT240, xterm.
        ///     Ps => 40  ⇒  Allow 80 ⇒  132 mode, xterm.
        ///     Ps => 41  ⇒  more(1) fix(see curses resource).
        ///     Ps => 42  ⇒  Enable National Replacement Character sets (DECNRCM), VT220.
        ///     Ps => 43  ⇒  Enable Graphics Expanded Print Mode(DECGEPM).
        ///     Ps => 44  ⇒  Turn on margin bell, xterm.
        ///     Ps => 44  ⇒  Enable Graphics Print Color Mode(DECGPCM).
        ///     Ps => 45  ⇒  Reverse-wraparound mode, xterm.
        ///     Ps => 45  ⇒  Enable Graphics Print ColorSpace(DECGPCS).
        ///     Ps => 46  ⇒  Start logging, xterm.  This is normally disabled by a compile-time option.
        ///     Ps => 47  ⇒  Use Alternate Screen Buffer, xterm.This may be disabled by the titeInhibit resource.
        ///     Ps => 47  ⇒  Enable Graphics Rotated Print Mode (DECGRPM).
        ///     Ps => 66  ⇒  Application keypad mode (DECNKM), VT320.
        ///     Ps => 67  ⇒  Backarrow key sends backspace (DECBKM), VT340, VT420.This sets the backarrowKey resource to "true".
        ///     Ps => 69  ⇒  Enable left and right margin mode (DECLRMM), VT420 and up.
        ///     Ps => 80  ⇒  Enable Sixel Scrolling (DECSDM).
        ///     Ps => 95  ⇒  Do not clear screen when DECCOLM is set/reset (DECNCSM), VT510 and up.
        ///     Ps => 1000  ⇒  Send Mouse X & Y on button press and release.  See the section Mouse Tracking.This is the X11 xterm mouse protocol.
        ///     Ps => 1001  ⇒  Use Hilite Mouse Tracking, xterm.
        ///     Ps => 1002  ⇒  Use Cell Motion Mouse Tracking, xterm.See the section Button-event tracking.
        ///     Ps => 1003  ⇒  Use All Motion Mouse Tracking, xterm. See the section Any-event tracking.
        ///     Ps => 1004  ⇒  Send FocusIn/FocusOut events, xterm. 
        ///     Ps => 1005  ⇒  Enable UTF-8 Mouse Mode, xterm.
        ///     Ps => 1006  ⇒  Enable SGR Mouse Mode, xterm.
        ///     Ps => 1007  ⇒  Enable Alternate Scroll Mode, xterm.  This corresponds to the alternateScroll resource.
        ///     Ps => 1010  ⇒  Scroll to bottom on tty output (rxvt). This sets the scrollTtyOutput resource to "true".
        ///     Ps => 1011  ⇒  Scroll to bottom on key press(rxvt).  This sets the scrollKey resource to "true".
        ///     Ps => 1015  ⇒  Enable urxvt Mouse Mode.
        ///     Ps => 1016  ⇒  Enable SGR Mouse PixelMode, xterm.
        ///     Ps => 1034  ⇒  Interpret "meta" key, xterm.This sets the eighth bit of keyboard input (and enables the eightBitInput resource).
        ///     Ps => 1035  ⇒  Enable special modifiers for Alt and NumLock keys, xterm.  This enables the numLock resource.
        ///     Ps => 1036  ⇒  Send ESC   when Meta modifies a key, xterm. This enables the metaSendsEscape resource.
        ///     Ps => 1037  ⇒  Send DEL from the editing-keypad Delete key, xterm.
        ///     Ps => 1039  ⇒  Send ESC  when Alt modifies a key, xterm. This enables the altSendsEscape resource, xterm.
        ///     Ps => 1040  ⇒  Keep selection even if not highlighted, xterm.  This enables the keepSelection resource.
        ///     Ps => 1041  ⇒  Use the CLIPBOARD selection, xterm.  This enables the selectToClipboard resource.
        ///     Ps => 1042  ⇒  Enable Urgency window manager hint when Control-G is received, xterm.This enables the bellIsUrgent resource.
        ///     Ps => 1043  ⇒  Enable raising of the window when Control-G is received, xterm.This enables the popOnBell resource.
        ///     Ps => 1044  ⇒  Reuse the most recent data copied to CLIP- BOARD, xterm.This enables the keepClipboard resource.
        ///     Ps => 1046  ⇒  Enable switching to/from Alternate Screen Buffer, xterm.This works for terminfo-based systems, updating the titeInhibit resource.
        ///     Ps => 1047  ⇒  Use Alternate Screen Buffer, xterm.This may be disabled by the titeInhibit resource.
        ///     Ps => 1048  ⇒  Save cursor as in DECSC, xterm.This may be disabled by the titeInhibit resource.
        ///     Ps => 1049  ⇒  Save cursor as in DECSC, xterm.After saving the cursor, switch to the Alternate Screen Buffer, 
        ///                    clearing it first.This may be disabled by the titeInhibit resource.  
        ///                    This control combines the effects of the 1047 and 1048  modes.
        ///                    Use this with terminfo-based applications rather than the 47  mode.
        ///     Ps => 1050  ⇒  Set terminfo/termcap function-key mode, xterm.
        ///     Ps => 1051  ⇒  Set Sun function-key mode, xterm.
        ///     Ps => 1052  ⇒  Set HP function-key mode, xterm.
        ///     Ps => 1053  ⇒  Set SCO function-key mode, xterm.
        ///     Ps => 1060  ⇒  Set legacy keyboard emulation, i.e, X11R6, xterm.
        ///     Ps => 1061  ⇒  Set VT220 keyboard emulation, xterm.
        ///     Ps => 2004  ⇒  Set bracketed paste mode, xterm.        
        /// </param>
        public static string PrivateModeSetDec(int Ps) => $"\x1b[?{Ps}h"; // CSI ? Pm h

        /// <summary>
        ///     DEC Private Mode Reset (DECRST). 
        /// </summary>
        /// <param name="Ps">
        ///     Ps => 1  ⇒  Normal Cursor Keys (DECCKM), VT100.
        ///     Ps => 2  ⇒  Designate VT52 mode (DECANM), VT100.
        ///     Ps => 3  ⇒  80 Column Mode (DECCOLM), VT100.
        ///     Ps => 4  ⇒  Jump (Fast) Scroll (DECSCLM), VT100.
        ///     Ps => 5  ⇒  Normal Video (DECSCNM), VT100.
        ///     Ps => 6  ⇒  Normal Cursor Mode (DECOM), VT100.
        ///     Ps => 7  ⇒  No Auto-Wrap Mode (DECAWM), VT100.
        ///     Ps => 8  ⇒  No Auto-Repeat Keys (DECARM), VT100.
        ///     Ps => 9  ⇒  Don't send Mouse X & Y on button press, xterm.
        ///     Ps => 10  ⇒  Hide toolbar (rxvt).
        ///     Ps => 12  ⇒  Stop blinking cursor (AT&T 610).
        ///     Ps => 13  ⇒  Disable blinking cursor (reset only via resource or menu).
        ///     Ps => 14  ⇒  Disable XOR of blinking cursor control sequence and menu.
        ///     Ps => 18  ⇒  Don't Print Form Feed (DECPFF), VT220.
        ///     Ps => 19  ⇒  Limit print to scrolling region (DECPEX), VT220.
        ///     Ps => 25  ⇒  Hide cursor (DECTCEM), VT220.
        ///     Ps => 30  ⇒  Don't show scrollbar (rxvt).
        ///     Ps => 35  ⇒  Disable font-shifting functions (rxvt).
        ///     Ps => 40  ⇒  Disallow 80 ⇒  132 mode, xterm.
        ///     Ps => 41  ⇒  No more(1) fix (see curses resource).
        ///     Ps => 42  ⇒  Disable National Replacement Character sets (DECNRCM), VT220.
        ///     Ps => 43  ⇒  Disable Graphics Expanded Print Mode (DECGEPM).
        ///     Ps => 44  ⇒  Turn off margin bell, xterm.
        ///     Ps => 44  ⇒  Disable Graphics Print Color Mode (DECGPCM).
        ///     Ps => 45  ⇒  No Reverse-wraparound mode, xterm.
        ///     Ps => 45  ⇒  Disable Graphics Print ColorSpace (DECGPCS).
        ///     Ps => 46  ⇒  Stop logging, xterm.  This is normally disabled by a compile-time option.
        ///     Ps => 47  ⇒  Use Normal Screen Buffer, xterm.
        ///     Ps => 47  ⇒  Disable Graphics Rotated Print Mode (DECGRPM).
        ///     Ps => 66  ⇒  Numeric keypad mode (DECNKM), VT320.
        ///     Ps => 67  ⇒  Backarrow key sends delete (DECBKM), VT340, VT420.  This sets the backarrowKey resource to "false".
        ///     Ps => 69  ⇒  Disable left and right margin mode (DECLRMM), VT420 and up.
        ///     Ps => 80  ⇒  Disable Sixel Scrolling (DECSDM).
        ///     Ps => 95  ⇒  Clear screen when DECCOLM is set/reset (DECNCSM), VT510 and up.
        ///     Ps => 1000  ⇒  Don't send Mouse X & Y on button press and release.  See the section Mouse Tracking.
        ///     Ps => 1001  ⇒  Don't use Hilite Mouse Tracking, xterm.
        ///     Ps => 1002  ⇒  Don't use Cell Motion Mouse Tracking, xterm.  See the section Button-event tracking.
        ///     Ps => 1003  ⇒  Don't use All Motion Mouse Tracking, xterm. See the section Any-event tracking.
        ///     Ps => 1004  ⇒  Don't send FocusIn/FocusOut events, xterm.
        ///     Ps => 1005  ⇒  Disable UTF-8 Mouse Mode, xterm.
        ///     Ps => 1006  ⇒  Disable SGR Mouse Mode, xterm.
        ///     Ps => 1007  ⇒  Disable Alternate Scroll Mode, xterm.  This corresponds to the alternateScroll resource.
        ///     Ps => 1010  ⇒  Don't scroll to bottom on tty output (rxvt).  This sets the scrollTtyOutput resource to "false".
        ///     Ps => 1011  ⇒  Don't scroll to bottom on key press (rxvt). This sets the scrollKey resource to "false".
        ///     Ps => 1015  ⇒  Disable urxvt Mouse Mode.
        ///     Ps => 1016  ⇒  Disable SGR Mouse Pixel-Mode, xterm.
        ///     Ps => 1034  ⇒  Don't interpret "meta" key, xterm.  This disables the eightBitInput resource.
        ///     Ps => 1035  ⇒  Disable special modifiers for Alt and NumLock keys, xterm.  This disables the numLock resource.
        ///     Ps => 1036  ⇒  Don't send ESC  when Meta modifies a key, xterm.  This disables the metaSendsEscape resource.
        ///     Ps => 1037  ⇒  Send VT220 Remove from the editing-keypad Delete key, xterm.
        ///     Ps => 1039  ⇒  Don't send ESC when Alt modifies a key, xterm.  This disables the altSendsEscape resource.
        ///     Ps => 1040  ⇒  Do not keep selection when not highlighted, xterm.  This disables the keepSelection resource.
        ///     Ps => 1041  ⇒  Use the PRIMARY selection, xterm.  This disables the selectToClipboard resource.
        ///     Ps => 1042  ⇒  Disable Urgency window manager hint when Control-G is received, xterm.  This disables the bellIsUrgent resource.
        ///     Ps => 1043  ⇒  Disable raising of the window when Control-G is received, xterm.  This disables the popOnBell resource.
        ///     Ps => 1046  ⇒  Disable switching to/from Alternate Screen Buffer, xterm.  This works for terminfo-based systems, updat- ing the 
        ///                     titeInhibit resource.  If currently using the Alternate Screen Buffer, xterm switches to the Normal Screen Buffer.
        ///     Ps => 1047  ⇒  Use Normal Screen Buffer, xterm.  Clear the screen first if in the Alternate Screen Buffer.  
        ///                     This may be disabled by the titeInhibit resource.
        ///     Ps => 1048  ⇒  Restore cursor as in DECRC, xterm.  This may be disabled by the titeInhibit resource.
        ///     Ps => 1049  ⇒  Use Normal Screen Buffer and restore cursor as in DECRC, xterm.  This may be disabled by the titeInhibit resource.  
        ///                     This combines the effects of the 1047 and 1048  modes.  
        ///                     Use this with terminfo-based applications rather than the 47 mode.
        ///     Ps => 1050  ⇒  Reset terminfo/termcap function-key mode, xterm.
        ///     Ps => 1051  ⇒  Reset Sun function-key mode, xterm.
        ///     Ps => 1052  ⇒  Reset HP function-key mode, xterm.
        ///     Ps => 1053  ⇒  Reset SCO function-key mode, xterm.
        ///     Ps => 1060  ⇒  Reset legacy keyboard emulation, i.e, X11R6, xterm.
        ///     Ps => 1061  ⇒  Reset keyboard emulation to Sun/PC style, xterm.
        ///     Ps => 2004  ⇒  Reset bracketed paste mode, xterm.
        /// </param>
        public static string PrivateResetDec(string Ps) => $"\x1b[?{Ps}l"; // CSI ? Ps l

        /// <summary>
        ///     Set character attributes
        /// </summary>
        /// <remarks>
        ///     <para>Some of the above note the edition of ECMA-48 which first describes a feature.  In its successive editions from 1979 to
        ///     1991 (2nd 1979, 3rd 1984, 4th 1986, and 5th 1991), ECMA-48 listed codes through 6 5 (skipping several toward the end of
        ///     the range).  Most of the ECMA-48 codes not implemented in xterm were never implemented in a hardware terminal.  Several
        ///     (such as 39 and 49 ) are either noted in ECMA-48 as implementation defined, or described in vague terms.</para>
        ///
        ///     <para>The successive editions of ECMA-48 give little attention to changes from one edition to the next, except to
        ///     comment on features which have become obsolete.  ECMA-48 1st (1976) is unavailable; there is no reliable
        ///     source of information which states whether "ANSI" color was defined in that edition, or later (1979).
        ///     The VT100 (1978) implemented the most commonly used non-color video attributes which are given in the 2nd edition.
        ///     While 8-color support is described in ECMA-48 2nd edition, the VT500 series (introduced in 1993) were the
        ///     first DEC terminals implementing "ANSI" color.  The DEC terminal's use of color is known to differ from xterm;
        ///     useful documentation on this series became available too late to influence xterm.</para>
        ///
        ///     <para>XTerm maintains a color palette whose entries are identified by an index beginning with zero.
        ///     If 88- or 256-color support is compiled, the following apply:</para>
        ///     <para>o   All parameters are decimal integers.</para>
        ///     <para>o   RGB values range from zero (0) to 255.</para>
        ///     <para>o   The 88- and 256-color support uses subparameters described in ISO-8613-6 for indexed color.
        ///         ISO-8613-6 also mentions direct color, using a similar scheme.  xterm supports that, too.</para>
        ///     <para>o   xterm allows either colons (standard) or semicolons (legacy) to separate the subparameters
        ///         (but after the first colon, colons must be used).</para>
        ///
        ///     <para>The indexed- and direct-color features are summarized in the
        ///     FAQ, which explains why semicolon is accepted as a subparameter delimiter:
        ///     Can I set a color by its number?</para>
        ///     <para>These ISO-8613-6 controls (marked in ECMA-48 5th edition as "reserved for future standardization")
        ///     are supported by xterm:</para>
        ///     <para>Ps => 38 : 2 : Pi : Pr : Pg : Pb ⇒  Set foreground color using RGB values.</para>
        ///     <para>If xterm is not compiled with direct-color support, it uses the closest match in its palette for the given RGB Pr/Pg/Pb.
        ///     The color space identifier Pi is ignored.</para>
        ///     <para>Ps => 38 : 5 : Ps ⇒  Set foreground color to Ps, using indexed color.</para>
        ///     <para>Ps => 48 : 2 : Pi : Pr : Pg : Pb ⇒  Set background color using RGB values.</para>
        ///     <para>If xterm is not compiled with direct-color support, it uses the closest match in its palette for the given RGB Pr/Pg/Pb.
        ///     The color space identifier Pi is ignored.</para>
        ///     <para>Ps => 48 : 5 : Ps ⇒  Set background color to Ps, using indexed color.</para>
        ///
        ///     <para>This variation on ISO-8613-6 is supported for compatibility with KDE konsole:</para>
        ///     <para>Ps => 38 ; 2 ; Pr ; Pg ; Pb ⇒  Set foreground color using RGB values.</para>
        ///     <para>If xterm is not compiled with direct-color support, it uses the closest match in its palette for the given RGB Pr/Pg/Pb.</para>
        ///     <para>Ps => 48 ; 2 ; Pr ; Pg ; Pb ⇒  Set background color using RGB values.</para>
        /// <para>If xterm is not compiled with direct-color support, it uses the closest match in its palette for the given RGB Pr/Pg/Pb.</para>
        ///
        /// <para>In each case, if xterm is compiled with direct-color support, and the resource directColor is true,
        /// then rather than choosing the closest match, xterm asks the X server to directly render a given color.</para>
        /// </remarks>
        /// <param name="Ps">
        ///     <para>Ps => 0  ⇒  Normal (default), VT100.</para>
        ///     <para>Ps => 1  ⇒  Bold, VT100.</para>
        ///     <para>Ps => 2  ⇒  Faint, decreased intensity, ECMA-48 2nd.</para>
        ///     <para>Ps => 3  ⇒  Italicized, ECMA-48 2nd.</para>
        ///     <para>Ps => 4  ⇒  Underlined, VT100.</para>
        ///     <para>Ps => 5  ⇒  Blink, VT100. This appears as Bold in X11R6 xterm.</para>
        ///     <para>Ps => 7  ⇒  Inverse, VT100.</para>
        ///     <para>Ps => 8  ⇒  Invisible, i.e., hidden, ECMA-48 2nd, VT300.</para>
        ///     <para>Ps => 9  ⇒  Crossed-out characters, ECMA-48 3rd.</para>
        ///     <para>Px => 10 ⇒  Primary(default) font</para> 
        ///     <para>Px => 11 ⇒  Alternative font</para>
        ///     <para>Ps => 21  ⇒  Doubly-underlined, ECMA-48 3rd.</para>
        ///     <para>Ps => 22  ⇒  Normal (neither bold nor faint), ECMA-48 3rd.</para>
        ///     <para>Ps => 23  ⇒  Not italicized, ECMA-48 3rd.</para>
        ///     <para>Ps => 24  ⇒  Not underlined, ECMA-48 3rd.</para>
        ///     <para>Ps => 25  ⇒  Steady (not blinking), ECMA-48 3rd.</para>
        ///     <para>Ps => 27  ⇒  Positive (not inverse), ECMA-48 3rd.</para>
        ///     <para>Ps => 28  ⇒  Visible, i.e., not hidden, ECMA-48 3rd, VT300.</para>
        ///     <para>Ps => 29  ⇒  Not crossed-out, ECMA-48 3rd.</para>
        ///     <para>Ps => 30  ⇒  Set foreground color to Black.</para>
        ///     <para>Ps => 31  ⇒  Set foreground color to Red.</para>
        ///     <para>Ps => 32  ⇒  Set foreground color to Green.</para>
        ///     <para>Ps => 33  ⇒  Set foreground color to Yellow.</para>
        ///     <para>Ps => 34  ⇒  Set foreground color to Blue.</para>
        ///     <para>Ps => 35  ⇒  Set foreground color to Magenta.</para>
        ///     <para>Ps => 36  ⇒  Set foreground color to Cyan.</para>
        ///     <para>Ps => 37  ⇒  Set foreground color to White.</para>
        ///     <para>Ps => 39  ⇒  Set foreground color to default, ECMA-48 3rd.</para>
        ///     <para>Ps => 40  ⇒  Set background color to Black.</para>
        ///     <para>Ps => 41  ⇒  Set background color to Red.</para>
        ///     <para>Ps => 42  ⇒  Set background color to Green.</para>
        ///     <para>Ps => 43  ⇒  Set background color to Yellow.</para>
        ///     <para>Ps => 44  ⇒  Set background color to Blue.</para>
        ///     <para>Ps => 45  ⇒  Set background color to Magenta.</para>
        ///     <para>Ps => 46  ⇒  Set background color to Cyan.</para>
        ///     <para>Ps => 47  ⇒  Set background color to White.</para>
        ///     <para>Ps => 49  ⇒  Set background color to default, ECMA-48 3rd.</para>
        ///
        /// <para>If 16-color support is compiled, the following aixterm controls apply.  Assume that xterm's resources are set so that
        /// the ISO color codes are the first 8 of a set of 16.  Then the aixterm colors are the bright versions of the ISO colors:</para>
        ///     <para>Ps => 90  ⇒  Set foreground color to Black.</para>
        ///     <para>Ps => 91  ⇒  Set foreground color to Red.</para>
        ///     <para>Ps => 92  ⇒  Set foreground color to Green.</para>
        ///     <para>Ps => 93  ⇒  Set foreground color to Yellow.</para>
        ///     <para>Ps => 94  ⇒  Set foreground color to Blue.</para>
        ///     <para>Ps => 95  ⇒  Set foreground color to Magenta.</para>
        ///     <para>Ps => 96  ⇒  Set foreground color to Cyan.</para>
        ///     <para>Ps => 97  ⇒  Set foreground color to White.</para>
        ///     <para>Ps => 100  ⇒  Set background color to Black.</para>
        ///     <para>Ps => 101  ⇒  Set background color to Red.</para>
        ///     <para>Ps => 102  ⇒  Set background color to Green.</para>
        ///     <para>Ps => 103  ⇒  Set background color to Yellow.</para>
        ///     <para>Ps => 104  ⇒  Set background color to Blue.</para>
        ///     <para>Ps => 105  ⇒  Set background color to Magenta.</para>
        ///     <para>Ps => 106  ⇒  Set background color to Cyan.</para>
        ///     <para>Ps => 107  ⇒  Set background color to White.</para>
        /// 
        /// <para>If xterm is compiled with the 16-color support disabled, it supports the following, from rxvt:</para>
        ///     <para>Ps => 100  ⇒  Set foreground and background color to default.</para>
        /// </param>
        public static string SetCharacterAttr(string Ps) => $"\x1b[{Ps}m"; // CSI Pm m  Character Attributes (SGR).


        /// <summary>
        ///     Set/reset key modifier options (XTMODKEYS), xterm.  Set or reset resource-values used by xterm to decide whether to construct
        ///     escape sequences holding information about the modifiers pressed with a given key.
        ///
        ///     If no parameters are given, all resources are reset to their initial values.
        /// </summary>
        /// <param name="Pp">
        ///     The first parameter Pp identifies the resource to set/reset.
        ///     If the second parameter is omitted, the resource is reset to its initial value.  Values 3  and 5  are reserved for
        ///     keypad-keys and string-keys.
        ///         Pp => 0  ⇒  modifyKeyboard.
        ///         Pp => 1  ⇒  modifyCursorKeys.
        ///         Pp => 2  ⇒  modifyFunctionKeys.
        ///         Pp => 4  ⇒  modifyOtherKeys.
        /// </param>
        /// <param name="Pv">
        ///     The second parameter Pv is the value to assign to the resource.
        /// </param>
        public static string SetKeyModifier(string Pp, string? Pv = null)
        {
            if (Pv != null)
                return $"\x1b[>{Pp};{Pv}m"; // CSI > Pp ; Pv m
            return $"\x1b[>{Pp}m";          // CSI > Pp m
        }

        /// <summary>
        ///     Disable key modifier options, xterm.  These modifiers may be enabled via the CSI > Pm m sequence.  This control sequence
        ///     corresponds to a resource value of "-1", which cannot be set with the other sequence.
        /// </summary>
        /// <param name="Ps">
        ///     The parameter identifies the resource to be disabled:
        ///         Ps => 0  ⇒  modifyKeyboard.
        ///         Ps => 1  ⇒  modifyCursorKeys.
        ///         Ps => 2  ⇒  modifyFunctionKeys.
        ///         Ps => 4  ⇒  modifyOtherKeys.
        ///     If the parameter is omitted, modifyFunctionKeys is disabled. When modifyFunctionKeys is disabled, xterm uses the modifier
        ///     keys to make an extended sequence of function keys rather than adding a parameter to each function key to denote the
        ///     modifiers.
        /// </param>
        public static string DisableKeyModifier(string Ps) => $"\x1b[>{Ps}n"; // CSI > Ps n

        /// <summary>
        ///     Device status report 
        /// </summary>
        /// <remarks>
        ///     Note: it is possible for this sequence to be sent by a function key.  For example, with the default keyboard
        ///     configuration the shifted F1 key may send (with shift-, control-, alt- modifiers)
        ///  <code>
        ///         CSI 1 ; 2  R , or
        ///         CSI 1 ; 5  R , or
        ///         CSI 1 ; 6  R , etc.
        /// </code>
        /// </remarks>
        /// <param name="Ps">
        ///     Ps => 5  ⇒  Status Report. Result ("OK") is CSI 0 n
        ///     Ps => 6  ⇒  Report Cursor Position (CPR) [row;column]. Result is CSI r ; c R
        ///     The second parameter encodes the modifiers; values range from 2 to 16.  See the section PC-Style Function Keys for the
        ///     codes.  The modifyFunctionKeys and modifyKeyboard resources can change the form of the string sent from the
        ///     modified F1 key.
        /// </param>
        /// <returns></returns>
        public static string DeviceStatusReport(int Ps) =>
            $"\x1b[{Ps}n"; // CSI Ps n  Device Status Report (DSR).

        /// <summary>
        ///     Device Status Report (DSR, DEC-specific). 
        /// </summary>
        /// <param name="Ps">
        ///     Ps => 6  ⇒  Report Cursor Position (DECXCPR).  The response [row;column] is returned as CSI ? r ; c R (assumes the default page, i.e., "1").
        ///     Ps => 15  ⇒  Report Printer status.  The response is CSI ? 10 n  (ready).  or CSI ? 11 n  (not ready).
        ///     Ps => 25  ⇒  Report UDK status.  The response is CSI ? 20 n  (unlocked) or CSI ? 21 n  (locked).
        ///     Ps => 26  ⇒  Report Keyboard status.  The response is CSI ? 27 ; 1 ; 0 ; 0 n  (North American).
        ///
        ///     The last two parameters apply to VT300 & up (keyboard ready) and VT400 & up (LK01) respectively.
        ///     Ps => 53  ⇒  Report Locator status.  The response is CSI ? 53 n  Locator available, if compiled-in, or CSI ? 50 n  No Locator, if not.
        ///     Ps => 55  ⇒  Report Locator status.  The response is CSI ? 53 n  Locator available, if compiled-in, or CSI ? 50 n  No Locator, if not.
        ///     Ps => 56  ⇒  Report Locator type.  The response is CSI ? 57 ; 1 n  Mouse, if compiled-in, or CSI ? 57 ; 0 n  Cannot identify, if not.
        ///     Ps => 62  ⇒  Report macro space (DECMSR).  The response is CSI Pn *  { .
        ///     Ps => 63  ⇒  Report memory checksum (DECCKSR), VT420 and up.
        ///     The response is DCS Pt ! ~ x x x x ST .
        ///     Pt is the request id (from an optional parameter to the request).
        ///     The x's are hexadecimal digits 0-9 and A-F.
        ///
        ///     Ps => 75  ⇒  Report data integrity.  The response is CSI ? 70 n  (ready, no errors).
        ///     Ps => 85  ⇒  Report multi-session configuration.  The response is CSI ? 83 n  (not configured for multiple-session operation).
        /// </param>
        public static string DeviceStatusReportDec(int Ps) => $"\x1b[?{Ps}n"; // CSI ? Ps n

        /// <summary>
        ///     Set resource value pointerMode (XTSMPOINTER), xterm.
        ///     This is used by xterm to decide whether to hide the pointer cursor as the user types.
        /// </summary>
        /// <param name="Ps">
        ///     <para>If no parameter is given, xterm uses the default, which is 1 .</para>
        /// 
        ///     <para>Ps => 0  ⇒  never hide the pointer.</para>
        ///     <para>Ps => 1  ⇒  hide if the mouse tracking mode is not enabled.</para>
        ///     <para>Ps => 2  ⇒  always hide the pointer, except when leaving the window.</para>
        ///     <para>Ps => 3  ⇒  always hide the pointer, even if leaving/entering the window.</para>
        /// </param>
        public static string SetResourceValue(string Ps) => $"\x1b[>{Ps}p"; // CSI > Ps p

        /// <summary>
        ///     Soft terminal reset
        /// </summary>
        public static string SoftTerminalReset => "\x1b[!p"; // CSI ! p   Soft terminal reset (DECSTR), VT220 and up.

        /// <summary>
        ///     Report name version
        ///     The response is a DSR sequence identifying the version: DCS > | text ST 
        /// </summary>
        /// <param name="Ps">
        ///     Ps => 0  ⇒  Report xterm name and version(XTVERSION).
        /// </param>
        /// <returns></returns>
        public static string ReportNameVersion(string Ps) => $"\x1b[>{Ps}q"; // CSI > Ps q

        /// <summary>
        ///     Load LEDs
        /// </summary>
        /// <param name="Ps">
        /// <list type="bullet"></list>
        ///     <para>0  ⇒  Clear all LEDS (default).</para>
        ///     <para>1  ⇒  Light Num Lock.</para>
        ///     <para>2  ⇒  Light Caps Lock.</para>
        ///     <para>3  ⇒  Light Scroll Lock.</para>
        ///     <para>21  ⇒  Extinguish Num Lock.</para>
        ///     <para>22  ⇒  Extinguish Caps Lock.</para>
        ///     <para>23  ⇒  Extinguish Scroll Lock.</para>
        /// </param>
        public static string LoadLEDs(string Ps) => $"\x1b[{Ps}q"; // CSI Ps q  Load LEDs (DECLL), VT100.

        /// <summary>
        ///     Set Scrolling Region [top;bottom] (default => full size of window) (DECSTBM), VT100.
        /// </summary>
        public static string SetScrollingRegion(string top, string bottom) => $"\x1b[{top};{bottom}r"; // CSI Ps ; Ps r


        /// <summary>
        ///     Restore DEC Private Mode Values (XTRESTORE), xterm.
        /// </summary>
        /// <param name="Ps">
        ///     The value of Ps previously saved is restored.  Values are the Same as (DECSET) 
        ///     Ps => 1  ⇒  Application Cursor Keys(DECCKM), VT100.
        ///     Ps => 2  ⇒  Designate USASCII for character sets G0-G3 (DECANM), VT100, and set VT100 mode.
        ///     Ps => 3  ⇒  132 Column Mode (DECCOLM), VT100.
        ///     Ps => 4  ⇒  Smooth (Slow) Scroll (DECSCLM), VT100.
        ///     Ps => 5  ⇒  Reverse Video (DECSCNM), VT100.
        ///     Ps => 6  ⇒  Origin Mode (DECOM), VT100.
        ///     Ps => 7  ⇒  Auto-Wrap Mode (DECAWM), VT100.
        ///     Ps => 8  ⇒  Auto-Repeat Keys (DECARM), VT100.
        ///     Ps => 9  ⇒  Send Mouse X & Y on button press.  See the section Mouse Tracking.This is the X10 xterm mouse protocol.
        ///     Ps => 10  ⇒  Show toolbar (rxvt).
        ///     Ps => 12  ⇒  Start blinking cursor (AT&T 610).
        ///     Ps => 13  ⇒  Start blinking cursor(set only via resource or menu).
        ///     Ps => 14  ⇒  Enable XOR of blinking cursor control sequence and menu.
        ///     Ps => 18  ⇒  Print Form Feed(DECPFF), VT220.
        ///     Ps => 19  ⇒  Set print extent to full screen(DECPEX), VT220.
        ///     Ps => 25  ⇒  Show cursor(DECTCEM), VT220.
        ///     Ps => 30  ⇒  Show scrollbar(rxvt).
        ///     Ps => 35  ⇒  Enable font-shifting functions(rxvt).
        ///     Ps => 38  ⇒  Enter Tektronix mode(DECTEK), VT240, xterm.
        ///     Ps => 40  ⇒  Allow 80 ⇒  132 mode, xterm.
        ///     Ps => 41  ⇒  more(1) fix(see curses resource).
        ///     Ps => 42  ⇒  Enable National Replacement Character sets (DECNRCM), VT220.
        ///     Ps => 43  ⇒  Enable Graphics Expanded Print Mode(DECGEPM).
        ///     Ps => 44  ⇒  Turn on margin bell, xterm.
        ///     Ps => 44  ⇒  Enable Graphics Print Color Mode(DECGPCM).
        ///     Ps => 45  ⇒  Reverse-wraparound mode, xterm.
        ///     Ps => 45  ⇒  Enable Graphics Print ColorSpace(DECGPCS).
        ///     Ps => 46  ⇒  Start logging, xterm.  This is normally disabled by a compile-time option.
        ///     Ps => 47  ⇒  Use Alternate Screen Buffer, xterm.This may be disabled by the titeInhibit resource.
        ///     Ps => 47  ⇒  Enable Graphics Rotated Print Mode (DECGRPM).
        ///     Ps => 66  ⇒  Application keypad mode (DECNKM), VT320.
        ///     Ps => 67  ⇒  Backarrow key sends backspace (DECBKM), VT340, VT420.This sets the backarrowKey resource to "true".
        ///     Ps => 69  ⇒  Enable left and right margin mode (DECLRMM), VT420 and up.
        ///     Ps => 80  ⇒  Enable Sixel Scrolling (DECSDM).
        ///     Ps => 95  ⇒  Do not clear screen when DECCOLM is set/reset (DECNCSM), VT510 and up.
        ///     Ps => 1000  ⇒  Send Mouse X & Y on button press and release.  See the section Mouse Tracking.This is the X11 xterm mouse protocol.
        ///     Ps => 1001  ⇒  Use Hilite Mouse Tracking, xterm.
        ///     Ps => 1002  ⇒  Use Cell Motion Mouse Tracking, xterm.See the section Button-event tracking.
        ///     Ps => 1003  ⇒  Use All Motion Mouse Tracking, xterm. See the section Any-event tracking.
        ///     Ps => 1004  ⇒  Send FocusIn/FocusOut events, xterm. 
        ///     Ps => 1005  ⇒  Enable UTF-8 Mouse Mode, xterm.
        ///     Ps => 1006  ⇒  Enable SGR Mouse Mode, xterm.
        ///     Ps => 1007  ⇒  Enable Alternate Scroll Mode, xterm.  This corresponds to the alternateScroll resource.
        ///     Ps => 1010  ⇒  Scroll to bottom on tty output (rxvt). This sets the scrollTtyOutput resource to "true".
        ///     Ps => 1011  ⇒  Scroll to bottom on key press(rxvt).  This sets the scrollKey resource to "true".
        ///     Ps => 1015  ⇒  Enable urxvt Mouse Mode.
        ///     Ps => 1016  ⇒  Enable SGR Mouse PixelMode, xterm.
        ///     Ps => 1034  ⇒  Interpret "meta" key, xterm.This sets the eighth bit of keyboard input (and enables the eightBitInput resource).
        ///     Ps => 1035  ⇒  Enable special modifiers for Alt and NumLock keys, xterm.  This enables the numLock resource.
        ///     Ps => 1036  ⇒  Send ESC   when Meta modifies a key, xterm. This enables the metaSendsEscape resource.
        ///     Ps => 1037  ⇒  Send DEL from the editing-keypad Delete key, xterm.
        ///     Ps => 1039  ⇒  Send ESC  when Alt modifies a key, xterm. This enables the altSendsEscape resource, xterm.
        ///     Ps => 1040  ⇒  Keep selection even if not highlighted, xterm.  This enables the keepSelection resource.
        ///     Ps => 1041  ⇒  Use the CLIPBOARD selection, xterm.  This enables the selectToClipboard resource.
        ///     Ps => 1042  ⇒  Enable Urgency window manager hint when Control-G is received, xterm.This enables the bellIsUrgent resource.
        ///     Ps => 1043  ⇒  Enable raising of the window when Control-G is received, xterm.This enables the popOnBell resource.
        ///     Ps => 1044  ⇒  Reuse the most recent data copied to CLIP- BOARD, xterm.This enables the keepClipboard resource.
        ///     Ps => 1046  ⇒  Enable switching to/from Alternate Screen Buffer, xterm.This works for terminfo-based systems, updating the titeInhibit resource.
        ///     Ps => 1047  ⇒  Use Alternate Screen Buffer, xterm.This may be disabled by the titeInhibit resource.
        ///     Ps => 1048  ⇒  Save cursor as in DECSC, xterm.This may be disabled by the titeInhibit resource.
        ///     Ps => 1049  ⇒  Save cursor as in DECSC, xterm.After saving the cursor, switch to the Alternate Screen Buffer, 
        ///                    clearing it first.This may be disabled by the titeInhibit resource.  
        ///                    This control combines the effects of the 1047 and 1048  modes.
        ///                    Use this with terminfo-based applications rather than the 47  mode.
        ///     Ps => 1050  ⇒  Set terminfo/termcap function-key mode, xterm.
        ///     Ps => 1051  ⇒  Set Sun function-key mode, xterm.
        ///     Ps => 1052  ⇒  Set HP function-key mode, xterm.
        ///     Ps => 1053  ⇒  Set SCO function-key mode, xterm.
        ///     Ps => 1060  ⇒  Set legacy keyboard emulation, i.e, X11R6, xterm.
        ///     Ps => 1061  ⇒  Set VT220 keyboard emulation, xterm.
        ///     Ps => 2004  ⇒  Set bracketed paste mode, xterm.
        /// </param>
        public static string RestorePrivateModeValuesDec(string Ps) => $"\x1b[?{Ps}r"; // CSI ? Pm r


        /// <summary>
        ///     Change Attributes in Rectangular Area (DECCARA), VT400 and up.
        /// </summary>
        /// <param name="Pt">top</param>
        /// <param name="Pl">left</param>
        /// <param name="Pb">bottom</param>
        /// <param name="Pr">right</param>
        /// <param name="Ps">the SGR attributes to change: 0, 1, 4, 5, 7.</param>
        /// <returns></returns>
        public static string ChangeAttrInArea(string Pt, string Pl, string Pb, string Pr, string Ps)
        {
            return $"\x1b[{Pt};{Pl};{Pb};{Pr};{Ps}$r";
            // CSI Pt ; Pl ; Pb ; Pr ; Ps $ r
        }


        /// <summary>
        ///     Set left and right margins (DECSLRM), VT420 and up.  This is available only when DECLRMM is enabled.
        /// </summary>
        public static string SetMargins(string left, string right) => $"\x1b[{left};{right}s"; // CSI Pl ; Pr s

        /// <summary>
        ///     Save DEC Private Mode Values (XTSAVE), xterm.
        /// </summary>
        /// <param name="Ps">
        ///     Ps => 1  ⇒  Application Cursor Keys(DECCKM), VT100.
        ///     Ps => 2  ⇒  Designate USASCII for character sets G0-G3 (DECANM), VT100, and set VT100 mode.
        ///     Ps => 3  ⇒  132 Column Mode (DECCOLM), VT100.
        ///     Ps => 4  ⇒  Smooth (Slow) Scroll (DECSCLM), VT100.
        ///     Ps => 5  ⇒  Reverse Video (DECSCNM), VT100.
        ///     Ps => 6  ⇒  Origin Mode (DECOM), VT100.
        ///     Ps => 7  ⇒  Auto-Wrap Mode (DECAWM), VT100.
        ///     Ps => 8  ⇒  Auto-Repeat Keys (DECARM), VT100.
        ///     Ps => 9  ⇒  Send Mouse X & Y on button press.  See the section Mouse Tracking.This is the X10 xterm mouse protocol.
        ///     Ps => 10  ⇒  Show toolbar (rxvt).
        ///     Ps => 12  ⇒  Start blinking cursor (AT&T 610).
        ///     Ps => 13  ⇒  Start blinking cursor(set only via resource or menu).
        ///     Ps => 14  ⇒  Enable XOR of blinking cursor control sequence and menu.
        ///     Ps => 18  ⇒  Print Form Feed(DECPFF), VT220.
        ///     Ps => 19  ⇒  Set print extent to full screen(DECPEX), VT220.
        ///     Ps => 25  ⇒  Show cursor(DECTCEM), VT220.
        ///     Ps => 30  ⇒  Show scrollbar(rxvt).
        ///     Ps => 35  ⇒  Enable font-shifting functions(rxvt).
        ///     Ps => 38  ⇒  Enter Tektronix mode(DECTEK), VT240, xterm.
        ///     Ps => 40  ⇒  Allow 80 ⇒  132 mode, xterm.
        ///     Ps => 41  ⇒  more(1) fix(see curses resource).
        ///     Ps => 42  ⇒  Enable National Replacement Character sets (DECNRCM), VT220.
        ///     Ps => 43  ⇒  Enable Graphics Expanded Print Mode(DECGEPM).
        ///     Ps => 44  ⇒  Turn on margin bell, xterm.
        ///     Ps => 44  ⇒  Enable Graphics Print Color Mode(DECGPCM).
        ///     Ps => 45  ⇒  Reverse-wraparound mode, xterm.
        ///     Ps => 45  ⇒  Enable Graphics Print ColorSpace(DECGPCS).
        ///     Ps => 46  ⇒  Start logging, xterm.  This is normally disabled by a compile-time option.
        ///     Ps => 47  ⇒  Use Alternate Screen Buffer, xterm.This may be disabled by the titeInhibit resource.
        ///     Ps => 47  ⇒  Enable Graphics Rotated Print Mode (DECGRPM).
        ///     Ps => 66  ⇒  Application keypad mode (DECNKM), VT320.
        ///     Ps => 67  ⇒  Backarrow key sends backspace (DECBKM), VT340, VT420.This sets the backarrowKey resource to "true".
        ///     Ps => 69  ⇒  Enable left and right margin mode (DECLRMM), VT420 and up.
        ///     Ps => 80  ⇒  Enable Sixel Scrolling (DECSDM).
        ///     Ps => 95  ⇒  Do not clear screen when DECCOLM is set/reset (DECNCSM), VT510 and up.
        ///     Ps => 1000  ⇒  Send Mouse X & Y on button press and release.  See the section Mouse Tracking.This is the X11 xterm mouse protocol.
        ///     Ps => 1001  ⇒  Use Hilite Mouse Tracking, xterm.
        ///     Ps => 1002  ⇒  Use Cell Motion Mouse Tracking, xterm.See the section Button-event tracking.
        ///     Ps => 1003  ⇒  Use All Motion Mouse Tracking, xterm. See the section Any-event tracking.
        ///     Ps => 1004  ⇒  Send FocusIn/FocusOut events, xterm. 
        ///     Ps => 1005  ⇒  Enable UTF-8 Mouse Mode, xterm.
        ///     Ps => 1006  ⇒  Enable SGR Mouse Mode, xterm.
        ///     Ps => 1007  ⇒  Enable Alternate Scroll Mode, xterm.  This corresponds to the alternateScroll resource.
        ///     Ps => 1010  ⇒  Scroll to bottom on tty output (rxvt). This sets the scrollTtyOutput resource to "true".
        ///     Ps => 1011  ⇒  Scroll to bottom on key press(rxvt).  This sets the scrollKey resource to "true".
        ///     Ps => 1015  ⇒  Enable urxvt Mouse Mode.
        ///     Ps => 1016  ⇒  Enable SGR Mouse PixelMode, xterm.
        ///     Ps => 1034  ⇒  Interpret "meta" key, xterm.This sets the eighth bit of keyboard input (and enables the eightBitInput resource).
        ///     Ps => 1035  ⇒  Enable special modifiers for Alt and NumLock keys, xterm.  This enables the numLock resource.
        ///     Ps => 1036  ⇒  Send ESC   when Meta modifies a key, xterm. This enables the metaSendsEscape resource.
        ///     Ps => 1037  ⇒  Send DEL from the editing-keypad Delete key, xterm.
        ///     Ps => 1039  ⇒  Send ESC  when Alt modifies a key, xterm. This enables the altSendsEscape resource, xterm.
        ///     Ps => 1040  ⇒  Keep selection even if not highlighted, xterm.  This enables the keepSelection resource.
        ///     Ps => 1041  ⇒  Use the CLIPBOARD selection, xterm.  This enables the selectToClipboard resource.
        ///     Ps => 1042  ⇒  Enable Urgency window manager hint when Control-G is received, xterm.This enables the bellIsUrgent resource.
        ///     Ps => 1043  ⇒  Enable raising of the window when Control-G is received, xterm.This enables the popOnBell resource.
        ///     Ps => 1044  ⇒  Reuse the most recent data copied to CLIP- BOARD, xterm.This enables the keepClipboard resource.
        ///     Ps => 1046  ⇒  Enable switching to/from Alternate Screen Buffer, xterm.This works for terminfo-based systems, updating the titeInhibit resource.
        ///     Ps => 1047  ⇒  Use Alternate Screen Buffer, xterm.This may be disabled by the titeInhibit resource.
        ///     Ps => 1048  ⇒  Save cursor as in DECSC, xterm.This may be disabled by the titeInhibit resource.
        ///     Ps => 1049  ⇒  Save cursor as in DECSC, xterm.After saving the cursor, switch to the Alternate Screen Buffer, 
        ///                    clearing it first.This may be disabled by the titeInhibit resource.  
        ///                    This control combines the effects of the 1047 and 1048  modes.
        ///                    Use this with terminfo-based applications rather than the 47  mode.
        ///     Ps => 1050  ⇒  Set terminfo/termcap function-key mode, xterm.
        ///     Ps => 1051  ⇒  Set Sun function-key mode, xterm.
        ///     Ps => 1052  ⇒  Set HP function-key mode, xterm.
        ///     Ps => 1053  ⇒  Set SCO function-key mode, xterm.
        ///     Ps => 1060  ⇒  Set legacy keyboard emulation, i.e, X11R6, xterm.
        ///     Ps => 1061  ⇒  Set VT220 keyboard emulation, xterm.
        ///     Ps => 2004  ⇒  Set bracketed paste mode, xterm.        
        /// </param>
        public static string SavePrivateModeValuesDec(string Ps) => $"\x1b[?{Ps}s"; // CSI ? Pm s


        /// <summary>
        ///     Window manipulation (XTWINOPS), dtterm, extended by xterm.
        ///     These controls may be disabled using the allowWindowOps resource. 
        /// </summary>
        /// <param name="Ps">
        ///     xterm uses Extended Window Manager Hints (EWMH) to maximize the window.  
        ///     Some window managers have incomplete support for EWMH.  For instance, fvwm, flwm and quartz-wm advertise support for 
        ///     maximizing windows horizontally or vertically, but in fact equate those to the maximize operation.
        ///     
        ///     Valid values for the first (and any additional parameters) are:
        ///     Ps => 1  ⇒  De-iconify window.
        ///     Ps => 2  ⇒  Iconify window.
        ///     Ps => 3 ;  x ;  y ⇒  Move window to [x, y].
        ///     Ps => 4 ;  height ;  width ⇒  Resize the xterm window to given height and width in pixels.  Omitted parameters reuse the current height or width.  Zero parameters use the display's height or width.
        ///     Ps => 5  ⇒  Raise the xterm window to the front of the stacking order.
        ///     Ps => 6  ⇒  Lower the xterm window to the bottom of the stacking order.
        ///     Ps => 7  ⇒  Refresh the xterm window.
        ///     Ps => 8 ;  height ;  width ⇒  Resize the text area to given height and width in characters.  Omitted parameters reuse the current height or width.  Zero parameters use the display's height or width.
        ///     Ps => 9 ;  0  ⇒  Restore maximized window.
        ///     Ps => 9 ;  1  ⇒  Maximize window (i.e., resize to screen size).
        ///     Ps => 9 ;  2  ⇒  Maximize window vertically.
        ///     Ps => 9 ;  3  ⇒  Maximize window horizontally.
        ///     Ps => 10 ;  0  ⇒  Undo full-screen mode.
        ///     Ps => 10 ;  1  ⇒  Change to full-screen.
        ///     Ps => 10 ;  2  ⇒  Toggle full-screen.
        ///     Ps => 11  ⇒  Report xterm window state.
        ///             If the xterm window is non-iconified, it returns CSI 1 t .
        ///             If the xterm window is iconified, it returns CSI 2 t .
        ///     Ps => 13  ⇒  Report xterm window position.
        ///             Note: X Toolkit positions can be negative, but the reported values are unsigned, in the range 0-65535.
        ///             Negative values correspond to 32768-65535. 
        ///             Result is CSI 3 ; x ; y t
        ///     Ps => 13 ;  2  ⇒  Report xterm text-area position. 
        ///             Result is CSI 3 ; x ; y t
        ///     Ps => 14  ⇒  Report xterm text area size in pixels. 
        ///             Result is CSI  4 ;  height ;  width t
        ///     Ps => 14 ;  2  ⇒  Report xterm window size in pixels. Normally xterm's window is larger than its text area, since it includes the frame (or decoration) applied by the window manager, as well as the area used by a scroll-bar. 
        ///             Result is CSI  4 ;  height ;  width t
        ///     Ps => 15  ⇒  Report size of the screen in pixels. 
        ///             Result is CSI  5 ;  height ;  width t
        ///     Ps => 16  ⇒  Report xterm character cell size in pixels. 
        ///             Result is CSI  6 ;  height ;  width t
        ///     Ps => 18  ⇒  Report the size of the text area in characters. 
        ///             Result is CSI  8 ;  height ;  width t
        ///     Ps => 19  ⇒  Report the size of the screen in characters. 
        ///             Result is CSI  9 ;  height ;  width t
        ///     Ps => 20  ⇒  Report xterm window's icon label. 
        ///             Result is OSC  L  label ST
        ///     Ps => 21  ⇒  Report xterm window's title. 
        ///             Result is OSC  l  label ST
        ///     Ps => 22 ; 0  ⇒  Save xterm icon and window title on stack.
        ///     Ps => 22 ; 1  ⇒  Save xterm icon title on stack.
        ///     Ps => 22 ; 2  ⇒  Save xterm window title on stack.
        ///     Ps => 23 ; 0  ⇒  Restore xterm icon and window title from stack.
        ///     Ps => 23 ; 1  ⇒  Restore xterm icon title from stack.
        ///     Ps => 23 ; 2  ⇒  Restore xterm window title from stack.
        ///     Ps >= 24  ⇒  Resize to Ps lines (DECSLPP), VT340 and VT420. xterm adapts this by resizing its window.
        /// </param>
        public static string WindowManipulation(string Ps, string? x = null, string? y = null)
        {
            if (x is not null && y is not null)
                return $"\x1b[{Ps};{x};{y}t";
            if (x is not null)
                return $"\x1b[{Ps};{x}t";
            return $"\x1b[{Ps}t";
            // CSI Ps ; Ps ; Ps t
        }


        /// <summary>
        ///     This xterm control sets one or more features of the title modes (XTSMTITLE), xterm.
        /// </summary>
        /// <param name="Ps">
        ///     Each parameter enables a single feature.
        ///       Ps => 0  ⇒  Set window/icon labels using hexadecimal.
        ///       Ps => 1  ⇒  Query window/icon labels using hexadecimal.
        ///       Ps => 2  ⇒  Set window/icon labels using UTF-8.
        ///       Ps => 3  ⇒  Query window/icon labels using UTF-8.  (See discussion of Title Modes)
        /// </param>
        public static string TitleStuff(string Ps) => $"\x1b[>{Ps}t"; // CSI > Pm t



        /// <summary>
        ///     Reverse Attributes in Rectangular Area (DECRARA), VT400 and up.
        /// </summary>
        /// <param name="Ps">
        ///     Ps denotes the attributes to reverse, i.e.,  1, 4, 5, 7.
        /// </param>
        public static string ReverseAttrArea(string top, string left, string bottom, string right, string Ps)
        {
            return $"\x1b[{top};{left};{bottom};{right};{Ps}$t";
            // CSI Pt ; Pl ; Pb ; Pr ; Ps $ t
        }

        public static string CursorRestore => "\x1b[u"; // CSI u     Restore cursor (SCORC, also ANSI.SYS).
        
        /// <summary>
        ///     Copy Rectangular Area (DECCRA), VT400 and up.
        /// </summary>
        public static string CopyArea(
            string top,
            string left,
            string bottom,
            string right,
            string sourcePage,
            string targetTop,
            string targetLeft,
            string targetPage)
        {
            return $"\x1b[{top};{left};{bottom};{right};{sourcePage};{targetTop}{targetLeft}{targetPage}$v";
            // CSI Pt ; Pl ; Pb ; Pr ; Pp ; Pt ; Pl ; Pp $ v
        }

        /// <summary>
        ///     Request presentation state report (DECRQPSR), VT320 and up. 
        /// </summary>
        /// <param name="Ps">
        ///         Ps => 0  ⇒  error.
        ///         Ps => 1  ⇒  cursor information report (DECCIR).
        ///     Response is
        ///         DCS 1 $ u Pt ST
        /// 
        ///     Refer to the VT420 programming manual, which requires six pages to document the data string Pt,
        ///         Ps => 2  ⇒  tab stop report (DECTABSR).
        ///     Response is
        ///         DCS 2 $ u Pt ST
        /// 
        ///     The data string Pt is a list of the tab-stops, separated by "/" characters.
        /// </param>
        public static string RequestPresentationPageReport(string Ps) => $"\x1b[{Ps}$w"; // CSI Ps $ w

        /// <summary>
        ///         Enable Filter Rectangle (DECEFR), VT420 and up.
        /// Parameters are [top;left;bottom;right].
        /// Defines the coordinates of a filter rectangle and activates it.  Anytime the locator is detected outside of the filter
        /// rectangle, an outside rectangle event is generated and the rectangle is disabled.  Filter rectangles are always treated
        /// as "one-shot" events.  Any parameters that are omitted default to the current locator position.  If all parameters are
        /// omitted, any locator motion will be reported.  DECELR always cancels any previous rectangle definition.
        /// </summary>
        public static string EnableFilterRectangle(string top, string left, string bottom, string right)
        {
            return $"\x1b[{top};{left};{bottom};{right}'w";
            // CSI Pt ; Pl ; Pb ; Pr ' w
        }

        /// <summary>
        ///     Request Terminal Parameters (DECREQTPARM).
        /// </summary>
        /// <param name="Ps">
        ///     if Ps is a "0" (default) or "1", and xterm is emulating VT100, the control sequence elicits a response of the 
        ///     same form whose parameters describe the terminal:
        ///         Ps ⇒  the given Ps incremented by 2.
        ///         Pn => 1  ⇐  no parity.
        ///         Pn => 1  ⇐  eight bits.
        ///         Pn => 1  ⇐  2 8  transmit 38.4k baud.
        ///         Pn => 1  ⇐  2 8  receive 38.4k baud.
        ///         Pn => 1  ⇐  clock multiplier.
        ///         Pn => 0  ⇐  STP flags.
        /// </param>
        public static string RequestTerminalParameters(string Ps) => $"\x1b[{Ps}x"; // CSI Ps x  

        /// <summary>
        ///     Select Attribute Change Extent (DECSACE), VT420 and up. 
        /// </summary>
        /// <param name="Ps">
        ///     Ps => 0  ⇒  from start to end position, wrapped.
        ///     Ps => 1  ⇒  from start to end position, wrapped.
        ///     Ps => 2  ⇒  rectangle (exact).
        /// </param>
        public static string SelectAttrChangeExtent(string Ps) => $"\x1b[{Ps}*x"; // CSI Ps * x

        /// <summary>
        ///     Fill Rectangular Area (DECFRA), VT420 and up.
        /// </summary>
        /// <param name="Pc">the Character to use</param>
        public static string FillArea(string Pc, string top, string left, string bottom, string right)
        {
            return $"\x1b[{Pc};{top};{left};{bottom};{right}$x";
            // CSI Pc ; Pt ; Pl ; Pb ; Pr $ x
        }

        /// <summary>
        ///     Enable Locator Reporting (DECELR)
        /// </summary>
        /// <param name="Ps">
        ///     Ps => 0  ⇒  Locator disabled (default).
        ///     Ps => 1  ⇒  Locator enabled.
        ///     Ps => 2  ⇒  Locator enabled for one report, then disabled.
        /// </param>
        /// <param name="Pu">
        /// Specifies the coordinate unit for locator reports.
        ///     Pu => 0  or omitted ⇒  default to character cells.
        ///     Pu => 1  ⇐  device physical pixels.
        ///     Pu => 2  ⇐  character cells.
        /// </param>
        public static string EnableLocatorReporting(int Ps, int Pu) => $"\x1b[{Ps};{Pu}'z"; // CSI Ps ; Pu ' z

        /// <summary>
        ///     Erase Rectangular Area (DECERA), VT400 and up. 
        /// </summary>
        public static string EraseArea(string top, string left, string bottom, string right) =>
            $"\x1b[{top};{left};{bottom};{right}$z"; // CSI Pt ; Pl ; Pb ; Pr $ z

        /// <summary>
        ///     Select Locator Events (DECSLE). 
        /// </summary>
        /// <param name="Pm">
        ///     Valid values for the first (and any additional parameters) are:
        ///         Ps => 0  ⇒  only respond to explicit host requests (DECRQLP).  Default. It also cancels any filter rectangle.
        ///         Ps => 1  ⇒  report button down transitions.
        ///         Ps => 2  ⇒  do not report button down transitions.
        ///         Ps => 3  ⇒  report button up transitions.
        ///         Ps => 4  ⇒  do not report button up transitions.
        /// </param>
        /// <returns></returns>
        public static string SelectLocatorEvents(string Pm) => $"\x1b[{Pm}'{{"; // CSI Pm ' {

        public static string PushVideoAttr => "\x1b[#{"; // CSI # {

        /// <summary>
        ///     The optional parameters correspond to the SGR encoding for video
        /// </summary>
        /// <param name="Pm">
        ///     The optional parameters correspond to the SGR encoding for video
        ///     attributes, except for colors (which do not have a unique SGR code):
        ///         Ps => 1  ⇒  Bold.
        ///         Ps => 2  ⇒  Faint.
        ///         Ps => 3  ⇒  Italicized.
        ///         Ps => 4  ⇒  Underlined.
        ///         Ps => 5  ⇒  Blink.
        ///         Ps => 7  ⇒  Inverse.
        ///         Ps => 8  ⇒  Invisible.
        ///         Ps => 9  ⇒  Crossed-out characters.
        ///         Ps => 2 1  ⇒  Doubly-underlined.
        ///         Ps => 3 0  ⇒  Foreground color.
        ///         Ps => 3 1  ⇒  BackgroundBrush color.
        ///     If no parameters are given, all of the video attributes are saved.  The stack is limited to 10 levels.
        /// </param>
        public static string PushVideoAttrs(string Pm) => $"\x1b[{Pm}#{{"; // CSI Pm # {


        /// <summary>
        ///     Selective Erase Rectangular Area (DECSERA), VT400 and up.
        /// </summary>
        public static string SelectiveEraseArea(string top, string left, string bottom, string right)
        {
            return $"\x1b[{top};{left};{bottom};{right}${{";
            // CSI Pt ; Pl ; Pb ; Pr $ {
        }

        /// <summary>
        ///     Report selected graphic rendition (XTREPORTSGR), xterm.
        ///     The response is an SGR sequence which contains the attributes which are common to all cells in a rectangle.
        /// </summary>
        public static string ReportSelectdGfxRend(string top, string left, string bottom, string right) =>
            $"\x1b[{top};{left};{bottom};{right}#|"; // CSI Pt ; Pl ; Pb ; Pr # |

        /// <summary>
        ///     Select columns per page (DECSCPP), VT340. 
        /// </summary>
        /// <param name="Ps">
        ///     Ps => 0  ⇒  80 columns, default if Ps omitted.
        ///     Ps => 80  ⇒  80 columns.
        ///     Ps => 132  ⇒  132 columns.
        /// </param>
        public static string SelectColumnsPP(string Ps) => $"\x1b[{Ps}$|"; // CSI Ps $ |

        /// <summary>
        ///     Request Locator Position (DECRQLP). 
        /// </summary>
        /// <param name="Ps">
        ///     Valid values for the parameter are:
        ///         Ps => 0 , 1 or omitted ⇒  transmit a single DECLRP locator report.
        ///     
        ///     If Locator Reporting has been enabled by a DECELR, xterm will respond with a DECLRP Locator Report.  This report is also
        ///     generated on button up and down events if they have been enabled with a DECSLE, or when the locator is detected outside
        ///     of a filter rectangle, if filter rectangles have been enabled with a DECEFR.
        ///     
        ///         ⇐  CSI Pe ; Pb ; Pr ; Pc ; Pp &  w
        ///     
        ///     Parameters are [event;button;row;column;page].
        ///     Valid values for the event:
        ///         Pe => 0  ⇐  locator unavailable - no other parameters sent.
        ///         Pe => 1  ⇐  request - xterm received a DECRQLP.
        ///         Pe => 2  ⇐  left button down.
        ///         Pe => 3  ⇐  left button up.
        ///         Pe => 4  ⇐  middle button down.
        ///         Pe => 5  ⇐  middle button up.
        ///         Pe => 6  ⇐  right button down.
        ///         Pe => 7  ⇐  right button up.
        ///         Pe => 8  ⇐  M4 button down.
        ///         Pe => 9  ⇐  M4 button up.
        ///         Pe => 10  ⇐  locator outside filter rectangle.
        ///     The "button" parameter is a bitmask indicating which buttons are pressed:
        ///         Pb => 0  ⇐  no buttons down.
        ///         Pb & 1  ⇐  right button down.
        ///         Pb & 2  ⇐  middle button down.
        ///         Pb & 4  ⇐  left button down.
        ///         Pb & 8  ⇐  M4 button down.
        ///     The "row" and "column" parameters are the coordinates of the locator position in the xterm window, encoded as ASCII decimal.
        ///     The "page" parameter is not used by xterm.
        /// </param>
        public static string RequestLocatorPosition(string Ps) => $"\x1b[{Ps}'|"; // CSI Ps ' |


        /// <summary>
        ///     Select number of lines per screen (DECSNLS), VT420 and up.
        ///     https://vt100.net/docs/vt510-rm/DECSNLS.html
        /// </summary>
        public static string SelectLinesPerScreen(string Ps) => $"\x1b[{Ps}*|"; // CSI Ps * |

        /// <summary>
        ///     Pop video attributes from stack (XTPOPSGR), xterm.
        ///     Popping restores the video-attributes which were saved using XTPUSHSGR to their previous state.
        /// </summary>
        public static string PopVideaAttr => "\x1b[#}"; // CSI # }

        /// <summary>
        ///     Insert Ps Column(s) (default => 1) (DECIC), VT420 and up.
        ///     https://vt100.net/docs/vt510-rm/DECIC.html
        /// </summary>
        public static string InsertColumns(string Ps) => $"\x1b[{Ps}'}}"; // CSI Ps ' }

        /// <summary>
        ///     Delete Ps Column(s) (default => 1) (DECDC), VT420 and up.
        /// </summary>
        public static string DeleteColumns => "\x1b[{Ps}'~"; // CSI Ps ' ~

        public static string SetTextParameter => "\x1b]{Ps};{Pt}\x07"; // OSC Ps ; Pt BEL

        /// <summary>
        ///     Text Parameters.  Some control sequences return information:
        ///     For colors and font, if Pt is a "?", the control sequence elicits a response which consists of the control sequence which would set the corresponding value.
        ///     The dtterm control sequences allow you to determine the icon name and window title.
        ///     m accepts either BEL  or ST  for terminating OSC sequences, and when returning information, uses the same terminator 
        ///      in a query.  While the latter is preferred, the former is supported for legacy applications:
        ///     Although documented in the changes for X.V10R4 (December 1986), BEL  as a string terminator dates from X11R4 (December 1989).
        ///     Since XFree86-3.1.2Ee (August 1996), xterm has accepted ST (the documented string terminator in ECMA-48).
        ///     pecifies the type of operation to perform:
        ///     Ps => 0  ⇒  Change Icon Name and Window Title to Pt.
        ///     Ps => 1  ⇒  Change Icon Name to Pt.
        ///     Ps => 2  ⇒  Change Window Title to Pt.
        ///     Ps => 3  ⇒  Set X property on top-level window.  Pt should be in the form "prop=value", or just "prop" to delete the property.
        ///     Ps => 4 ; c ; spec ⇒  Change Color Number c to the color specified by spec.  
        ///                 This can be a name or RGB specification as per XParseColor.  Any number of c/spec pairs may be given.
        ///                 The color numbers correspond to the ANSI colors 0-7, their bright versions 8-15, and if supported, 
        ///                 the remainder of the 88-color or 256-color table.
        ///      "?" is given rather than a name or RGB specification, xterm replies with a control sequence of the same form which
        ///     be used to set the corresponding color.  Because more than one pair of color number and specification can be given in one
        ///     rol sequence, xterm can make more than one reply.
        ///     Ps => 5 ; c ; spec ⇒  Change Special Color Number c to the color specified by spec.  
        ///                 This can be a name or RGB specification 
        ///                 as per XParseColor.  Any number of c/spec pairs may be given.  The special colors can also be set by 
        ///                 adding the maximum  number of colors to these codes in an OSC 4  control:
        ///                 Pc => 0  ⇐  resource colorBD (BOLD).
        ///                 Pc => 1  ⇐  resource colorUL (UNDERLINE).
        ///                 Pc => 2  ⇐  resource colorBL (BLINK).
        ///                 Pc => 3  ⇐  resource colorRV (REVERSE).
        ///                 Pc => 4  ⇐  resource colorIT (ITALIC).
        ///     Ps => 6 ; c ; f ⇒  Enable/disable Special Color Number c.
        ///                 The second parameter tells xterm to enable the corresponding color mode if nonzero, disable it if zero.  
        ///                 OSC 6  is the same as OSC 106 .
        ///                 The 10 colors (below) which may be set or queried using 10 through 19  are denoted dynamic colors, 
        ///                 since the corresponding control sequences were the first means for setting xterm's colors dynamically, 
        ///                 i.e., after it was started.  
        ///                 They are not the same as the ANSI colors (however, the dynamic text foreground and background colors 
        ///                 are used when ANSI colors are reset using SGR 3 9  and 4 9 , respectively).  These controls may be 
        ///                 disabled using the allowColorOps resource.  At least one parameter is expected for Pt.  Each 
        ///                 successive parameter changes the next color in the list.  The value of Ps tells the starting point in 
        ///                 the list.  The colors are specified by name or RGB specification as per XParseColor.
        ///                 If a "?" is given rather than a name or RGB specification, xterm replies with a control sequence of 
        ///                 the same form which can be used to set the corresponding dynamic color.  Because more than one pair 
        ///                 of color number and specification can be given in one control sequence, xterm can make more than one reply.
        ///     Ps => 10  ⇒  Change VT100 text foreground color to Pt.
        ///     Ps => 11  ⇒  Change VT100 text background color to Pt.
        ///     Ps => 12  ⇒  Change text cursor color to Pt.
        ///     Ps => 13  ⇒  Change pointer foreground color to Pt.
        ///     Ps => 14  ⇒  Change pointer background color to Pt.
        ///     Ps => 15  ⇒  Change Tektronix foreground color to Pt.
        ///     Ps => 16  ⇒  Change Tektronix background color to Pt.
        ///     Ps => 17  ⇒  Change highlight background color to Pt.
        ///     Ps => 18  ⇒  Change Tektronix cursor color to Pt.
        ///     Ps => 19  ⇒  Change highlight foreground color to Pt.
        ///     Ps => 46  ⇒  Change Log File to Pt.  This is normally disabled by a compile-time option.
        ///     Ps => 50  ⇒  Set Font to Pt.  These controls may be disabled using the allowFontOps resource.  If Pt begins with a "#",
        ///                 index in the font menu, relative (if the next character is a plus or minus sign) or absolute.  
        ///                 A number is expected but not required after the sign (the default is the current entry for
        ///                 relative, zero for absolute indexing).
        ///                 The same rule (plus or minus sign, optional number) is used when querying the font.  
        ///                 The remainder of Pt is ignored.
        ///                 A font can be specified after a "#" index expression, by adding a space and then the font specifier.
        ///                 If the TrueType Fonts menu entry is set (the renderFont resource), then this control sets/queries the faceName resource.
        ///     Ps => 51  ⇒  reserved for Emacs shell.
        ///     Ps => 52  ⇒  Manipulate Selection Data.  These controls may be disabled using the allowWindowOps resource.  
        ///                 The parameter Pt is parsed as Pc ; Pd
        ///                 The first, Pc, may contain zero or more characters from the set 
        ///                     c , p , q , s , 0 , 1 , 2 , 3 , 4 , 5 , 6 , and 7 .  
        ///                 It is used to construct a list of selection parameters for clipboard, primary, secondary, select, 
        ///                 or cut buffers 0 through 7 respectively, in the order given.  If the parameter is empty,
        ///                 xterm uses s 0 , to specify the configurable primary/clipboard selection and cut buffer 0.
        ///                 The second parameter, Pd, gives the selection data.  Normally
        ///                 this is a string encoded in base64 (RFC-4648).  The data
        ///                 becomes the new selection, which is then available for pasting
        ///                 by other applications.
        ///                 If the second parameter is a ? , xterm replies to the host
        ///                 with the selection data encoded using the same protocol.  It
        ///                 uses the first selection found by asking successively for each
        ///                 item from the list of selection parameters.
        ///                 If the second parameter is neither a base64 string nor ? ,
        ///                 then the selection is cleared.
        ///     Ps => 104 ; c ⇒  Reset Color Number c.  
        ///                 It is reset to the color specified by the corresponding X resource.  Any number
        ///                 of c parameters may be given.  These parameters correspond to
        ///                 the ANSI colors 0-7, their bright versions 8-15, and if sup-
        ///                 ported, the remainder of the 88-color or 256-color table.  If
        ///                 no parameters are given, the entire table will be reset.
        ///     Ps => 105 ; c ⇒  Reset Special Color Number c.  
        ///                 It is reset to the color specified by the corresponding X resource.  Any
        ///                 number of c parameters may be given.  These parameters corre-
        ///                 spond to the special colors which can be set using an OSC 5
        ///                 control (or by adding the maximum number of colors using an
        ///                 OSC 4  control).
        ///     Ps => 106 ; c ; f ⇒  Enable/disable Special Color Number c.
        ///                 The second parameter tells xterm to enable the corresponding
        ///                 color mode if nonzero, disable it if zero.
        ///          ///                 Pc => 0  ⇐  resource colorBDMode (BOLD).
        ///                 Pc => 1  ⇐  resource colorULMode (UNDERLINE).
        ///                 Pc => 2  ⇐  resource colorBLMode (BLINK).
        ///                 Pc => 3  ⇐  resource colorRVMode (REVERSE).
        ///                 Pc => 4  ⇐  resource colorITMode (ITALIC).
        ///                 Pc => 5  ⇐  resource colorAttrMode (Override ANSI).
        ///     The dynamic colors can also be reset to their default (resource) values:
        ///     Ps => 110  ⇒  Reset VT100 text foreground color.
        ///     Ps => 111  ⇒  Reset VT100 text background color.
        ///     Ps => 112  ⇒  Reset text cursor color.
        ///     Ps => 113  ⇒  Reset pointer foreground color.
        ///     Ps => 114  ⇒  Reset pointer background color.
        ///     Ps => 115  ⇒  Reset Tektronix foreground color.
        ///     Ps => 116  ⇒  Reset Tektronix background color.
        ///     Ps => 117  ⇒  Reset highlight color.
        ///     Ps => 118  ⇒  Reset Tektronix cursor color.
        ///     Ps => 119  ⇒  Reset highlight foreground color.
        ///     Ps => I  ; c ⇒  Set icon to file.  Sun shelltool, CDE dtterm.
        ///                 The file is expected to be XPM format, and uses the same
        ///                 search logic as the iconHint resource.
        ///     Ps => l  ; c ⇒  Set window title.  Sun shelltool, CDE dtterm.
        ///     Ps => L  ; c ⇒  Set icon label.  Sun shelltool, CDE dtterm.
        /// </summary>
        public static string SetTextParameters(string Ps, string Pt) => $"\x1b]{Ps};{Pt}\x1b\\"; // OSC Ps ; Pt ST

        // ReSharper restore InconsistentNaming
    }
}

/*
     Dec   Hex    Binary   HTML     Char    Description
     0      00  00000000   &#0;     NUL	    Null
     1      01  00000001   &#1;     SOH	    Start of Header
     2      02  00000010   &#2;     STX	    Start of Text
     3      03  00000011   &#3;     ETX	    End of Text
     4      04  00000100   &#4;     EOT	    End of Transmission
     5      05  00000101   &#5;     ENQ	    Enquiry
     6      06  00000110   &#6;     ACK	    Acknowledge
     7      07  00000111   &#7;     BEL	    Bell
     8      08  00001000   &#8;     BS      Backspace
     9      09  00001001   &#9;     HT      Horizontal Tab
     10	    0A  00001010   &#10;    LF      Line Feed
     11	    0B  00001011   &#11;    VT      Vertical Tab
     12	    0C  00001100   &#12;    FF      Form Feed
     13	    0D  00001101   &#13;    CR      Carriage Return
     14	    0E  00001110   &#14;    SO      Shift Out
     15	    0F  00001111   &#15;    SI      Shift In
     16	    10  00010000   &#16;    DLE     Data Link Escape
     17	    11  00010001   &#17;    DC1     Device Control 1
     18	    12  00010010   &#18;    DC2     Device Control 2
     19	    13  00010011   &#19;    DC3     Device Control 3
     20	    14  00010100   &#20;    DC4     Device Control 4
     21	    15  00010101   &#21;    NAK     Negative Acknowledge
     22	    16  00010110   &#22;    SYN     Synchronize
     23	    17  00010111   &#23;    ETB     End of Transmission Block
     24	    18  00011000   &#24;    CAN     Cancel
     25	    19  00011001   &#25;    EM      End of Medium
     26	    1A  00011010   &#26;    SUB     Substitute
     27	    1B  00011011   &#27;    ESC     Escape
     28	    1C  00011100   &#28;    FS      File Separator
     29	    1D  00011101   &#29;    GS      Group Separator
     30	    1E  00011110   &#30;    RS      Record Separator
     31	    1F  00011111   &#31;    US      Unit Separator
     32	    20  00100000   &#32;    space   Space
     33	    21  00100001   &#33;    !       exclamation mark
     34	    22  00100010   &#34;    "       double quote
     35	    23  00100011   &#35;    #       number
     36	    24  00100100   &#36;    $       dollar
     37	    25  00100101   &#37;    %       percent
     38	    26  00100110   &#38;    &       ampersand
     39	    27  00100111   &#39;    '       single quote
     40	    28  00101000   &#40;    (       left parenthesis
     41	    29  00101001   &#41;    )       right parenthesis
     42	    2A  00101010   &#42;    *       asterisk
     43	    2B  00101011   &#43;    +       plus
     44	    2C  00101100   &#44;    ,       comma
     45	    2D  00101101   &#45;    -       minus
     46	    2E  00101110   &#46;    .       period
     47	    2F  00101111   &#47;    /       slash
     48	    30  00110000   &#48;    0       zero
     49	    31  00110001   &#49;    1       one
     50	    32  00110010   &#50;    2       two
     51	    33  00110011   &#51;    3       three
     52	    34  00110100   &#52;    4       four
     53	    35  00110101   &#53;    5       five
     54	    36  00110110   &#54;    6       six
     55	    37  00110111   &#55;    7       seven
     56	    38  00111000   &#56;    8       eight
     57	    39  00111001   &#57;    9       nine
     58	    3A  00111010   &#58;    :       colon
     59	    3B  00111011   &#59;    ;       semicolon
     60	    3C  00111100   &#60;    <       less than
     61	    3D  00111101   &#61;    =>       equality sign
     62	    3E  00111110   &#62;    >       greater than
     63	    3F  00111111   &#63;    ?       question mark
     64	    40  01000000   &#64;    @       at sign
     65	    41  01000001   &#65;    A        
     66	    42  01000010   &#66;    B        
     67	    43  01000011   &#67;    C        
     68	    44  01000100   &#68;    D        
     69	    45  01000101   &#69;    E        
     70	    46  01000110   &#70;    F        
     71	    47  01000111   &#71;    G        
     72	    48  01001000   &#72;    H        
     73	    49  01001001   &#73;    I        
     74	    4A  01001010   &#74;    J        
     75	    4B  01001011   &#75;    K        
     76	    4C  01001100   &#76;    L        
     77	    4D  01001101   &#77;    M        
     78	    4E  01001110   &#78;    N        
     79	    4F  01001111   &#79;    O        
     80	    50  01010000   &#80;    P        
     81	    51  01010001   &#81;    Q        
     82	    52  01010010   &#82;    R        
     83	    53  01010011   &#83;    S        
     84	    54  01010100   &#84;    T        
     85	    55  01010101   &#85;    U        
     86	    56  01010110   &#86;    V        
     87	    57  01010111   &#87;    W        
     88	    58  01011000   &#88;    X        
     89	    59  01011001   &#89;    Y        
     90	    5A  01011010   &#90;    Z        
     91	    5B  01011011   &#91;    [       left square bracket
     92	    5C  01011100   &#92;    \       backslash
     93	    5D  01011101   &#93;    ]       right square bracket
     94	    5E  01011110   &#94;    ^       caret / circumflex
     95	    5F  01011111   &#95;    _       underscore
     96	    60  01100000   &#96;    `       grave / accent
     97	    61  01100001   &#97;    a        
     98	    62  01100010   &#98;    b        
     99	    63  01100011   &#99;    c        
     100    64  01100100   &#100;   d        
     101    65  01100101   &#101;   e        
     102    66  01100110   &#102;   f        
     103    67  01100111   &#103;   g        
     104    68  01101000   &#104;   h        
     105    69  01101001   &#105;   i        
     106    6A  01101010   &#106;   j        
     107    6B  01101011   &#107;   k        
     108    6C  01101100   &#108;   l        
     109    6D  01101101   &#109;   m        
     110    6E  01101110   &#110;   n        
     111    6F  01101111   &#111;   o        
     112    70  01110000   &#112    p        
     113    71  01110001   &#113;   q        
     114    72  01110010   &#114;   r        
     115    73  01110011   &#115;   s        
     116    74  01110100   &#116;   t        
     117    75  01110101   &#117;   u        
     118    76  01110110   &#118;   v        
     119    77  01110111   &#119;   w        
     120    78  01111000   &#120;   x        
     121    79  01111001   &#121;   y        
     122    7A  01111010   &#122;   z        
     123    7B  01111011   &#123;   {       left curly bracket
     124    7C  01111100   &#124;   |       vertical bar
     125    7D  01111101   &#125;   }       right curly bracket
     126    7E  01111110   &#126;   ~       tilde
     127    7F  01111111   &#127;   DEL     delete
*/