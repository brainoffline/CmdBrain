namespace No8.CmdBrain.VirtualTerminal;

public enum ControlChar : byte
{
    NUL = 0x00, //    Null
    SOH = 0x01, //    Start of Header
    STX = 0x02, //    Start of Text
    ETX = 0x03, //    End of Text
    EOT = 0x04, //    End of Transmission
    ENQ = 0x05, //    Enquiry
    ACK = 0x06, //    Acknowledge
    BEL = 0x07, //    Bell
    BS = 0x08,  //    Backspace
    HT = 0x09,  //    Horizontal Tab
    LF = 0x0A,  //    Line Feed
    VT = 0x0B,  //    Vertical Tab
    FF = 0x0C,  //    Form Feed
    CR = 0x0D,  //    Carriage Return
    SO = 0x0E,  //    Shift Out
    SI = 0x0F,  //    Shift In
    DLE = 0x10, //    Data Link Escape
    DC1 = 0x11, //    Device Control 1
    DC2 = 0x12, //    Device Control 2
    DC3 = 0x13, //    Device Control 3
    DC4 = 0x14, //    Device Control 4
    NAK = 0x15, //    Negative Acknowledge
    SYN = 0x16, //    Synchronize
    ETB = 0x17, //    End of Transmission Block
    CAN = 0x18, //    Cancel
    EM = 0x19,  //    End of Medium
    SUB = 0x1A, //    Substitute
    ESC = 0x1B, //    Escape
    FS = 0x1C,  //    File Separator
    GS = 0x1D,  //    Group Separator
    RS = 0x1E,  //    Record Separator
    US = 0x1F,  //    Unit Separator
    DEL = 0x7F, //    Delete
}