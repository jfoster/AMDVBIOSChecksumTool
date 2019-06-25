using System.Runtime.InteropServices;

namespace AMDVBIOSChecksumTool
{
    class ATOM
    {
        public const byte ATOM_ROM_SIZE_OFFSET = 0x2;
        public const byte ATOM_ROM_CHECKSUM_OFFSET = 0x21;

        public const byte ATOM_ROM_HEADER_POINTER = 0x48;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_COMMON_TABLE_HEADER
        {
            public short usStructureSize;
            public byte ucTableFormatRevision;
            public byte ucTableContentRevision;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ATOM_ROM_HEADER
        {
            public ATOM_COMMON_TABLE_HEADER sHeader;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x4)]
            public char[] uaFirmWareSignature;
            public ushort usBiosRuntimeSegmentAddress;
            public ushort usProtectedModeInfoOffset;
            public ushort usConfigFilenameOffset;
            public ushort usCRC_BlockOffset;
            public ushort usBIOS_BootupMessageOffset;
            public ushort usInt10Offset;
            public ushort usPciBusDevInitCode;
            public ushort usIoBaseAddress;
            public ushort usSubsystemVendorID;
            public ushort usSubsystemID;
            public ushort usPCI_InfoOffset;
            public ushort usMasterCommandTableOffset;
            public ushort usMasterDataTableOffset;
            public byte ucExtendedFunctionCode;
            public byte ucReserved;
        }
    }
}
