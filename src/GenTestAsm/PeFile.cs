using System;
//using System.Collections.Generic;
using System.IO;

namespace GenTestAsm
{
    /// <summary>
    /// Parses PE files
    /// </summary>
    internal class PeFile : IDisposable
    {
        private FileStream _file;
        private BinaryReader _reader;

        public PeFile(string path)
        {
            _file = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.Read );
            _reader = new BinaryReader(_file);
        }

        public uint GetPeHeaderOffset()
        {
            return ReadDwordAtOffset(0x3c);
        }

        public uint GetPeSignature()
        {
            return ReadDwordAtOffset(GetPeHeaderOffset());
        }

        public bool IsValidPeFile()
        {
            return GetPeSignature() == 0x00004550; // 'P', 'E', 0, 0
        }

        public uint GetOptionalHeaderSize()
        {
            return ReadWordAtOffset( GetPeHeaderOffset() + 20 );
        }

        public uint GetOptionalHeaderOffset()
        {
            return GetPeHeaderOffset() + 24;
        }

        public uint GetOptionalHeaderMagic()
        {
            return ReadWordAtOffset( GetOptionalHeaderOffset() );
        }

        public bool IsPe32()
        {
            return GetOptionalHeaderMagic() == 0x10b;
        }

        public bool IsPe32Plus()
        {
            return GetOptionalHeaderMagic() == 0x20b;
        }

        public uint GetNumberOfDataDirEntries()
        {
            EnsurePe32();
            return ReadDwordAtOffset(GetOptionalHeaderOffset() + 92);
        }

        public uint GetImageBase()
        {
            EnsurePe32();
            return ReadDwordAtOffset(GetOptionalHeaderOffset() + 28);
        }

        public uint GetExportTableRva()
        {
            EnsurePe32();
            if (GetOptionalHeaderSize() < 100) throw new ApplicationException("There is no export table in the optional header");
            if (GetNumberOfDataDirEntries() < 1) throw new ApplicationException("There is no export table in the optional header");
            return ReadDwordAtOffset(GetOptionalHeaderOffset() + 96);
        }

        public uint GetExportTableDirectoryOffset()
        {
            return GetFilePointerForRva( GetExportTableRva() );
        }

        public uint GetExportedNamesPointerTableRva()
        {
            return ReadDwordAtOffset( GetExportTableDirectoryOffset() + 32 );
        }

        public uint GetExportedNamesPointerTableOffset()
        {
            return GetFilePointerForRva( GetExportedNamesPointerTableRva() );
        }

        public uint GetExportedNamePtrOffset(uint nName)
        {
            return GetExportedNamesPointerTableOffset() + 4*nName;
        }

        public string GetExportedName(uint nName)
        {
            uint ptrOffset = GetExportedNamePtrOffset( nName );
            uint ptrRva = ReadDwordAtOffset(ptrOffset);
            uint nameOffset = GetFilePointerForRva(ptrRva);

            _file.Seek( nameOffset, SeekOrigin.Begin );
            byte[] bytes = new byte[1024]; // max size
            int i=0;
            for (; i<bytes.Length; ++i)
            {
                byte b = _reader.ReadByte();
                if (b == 0) break;
                bytes[i] = b;
            }

            return System.Text.Encoding.ASCII.GetString( bytes, 0, i );
        }

        public uint GetNumberOfExportedNames()
        {
            return ReadDwordAtOffset( GetExportTableDirectoryOffset() + 24 );
        }


        public uint GetNumberOfSections()
        {
            return ReadWordAtOffset(GetPeHeaderOffset() + 6 );
        }

        public uint GetSectionTableStart()
        {
            return GetOptionalHeaderOffset() + GetOptionalHeaderSize();
        }

        public uint GetSectionStart(uint section)
        {
            return GetSectionTableStart() + 40*section;
        }

        public uint GetSectionVirtualBase(uint section)
        {
            return ReadDwordAtOffset( GetSectionStart(section) + 12 );
        }

        public uint GetSectionVirtualSize(uint section)
        {
            return ReadDwordAtOffset( GetSectionStart(section) + 8 );
        }

        public uint GetSectionContainingRva(uint rva)
        {
            uint nSections = GetNumberOfSections();
            for (uint i=0; i<nSections; ++i)
            {
                uint vBase = GetSectionVirtualBase(i);
                if (rva < vBase) continue; // not our section
                if (rva < vBase + GetSectionVirtualSize(i))
                {
                    return i;
                }
            }

            return 0xFFFFFFFF;
        }

        public uint GetSectionFilePointer(uint nSection)
        {
            return ReadDwordAtOffset( GetSectionStart( nSection ) + 20 );
        }

        public uint GetSectionFileSize(uint nSection)
        {
            return ReadDwordAtOffset( GetSectionStart( nSection ) + 16 );
        }

        public uint GetFilePointerForRva(uint rva)
        {
            uint nSection = GetSectionContainingRva( rva );
            if (nSection == 0xFFFFFFFF) return 0xFFFFFFFF; // not found
            uint vBase = GetSectionVirtualBase( nSection );
            uint fileSize = GetSectionFileSize(nSection);
            if (rva >= vBase + fileSize) return 0xFFFFFFFF; // beyond physical section end on disk
            uint fileBase = GetSectionFilePointer(nSection);
            return fileBase + (rva-vBase);
        }

        public string[] GetExportedNames()
        {
            uint nNames = GetNumberOfExportedNames();
            string[] result = new string[nNames];
            for (uint i=0; i<nNames; ++i)
            {
                result[i] = GetExportedName( i );
            }

            return result;
        }

        public void Dispose()
        {
            _file.Close();
        }

        private void EnsurePe32()
        {
            if (!IsPe32()) throw new ApplicationException( "Only PE32 files are supported by this version" );
        }

        private uint ReadDwordAtOffset(uint offset)
        {
            _file.Seek(offset, SeekOrigin.Begin);
            return _reader.ReadUInt32();
        }

        private ushort ReadWordAtOffset(uint offset)
        {
            _file.Seek( offset, SeekOrigin.Begin );
            return _reader.ReadUInt16();
        }
    }
}
