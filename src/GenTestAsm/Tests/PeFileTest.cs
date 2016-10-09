#if DEBUG
using System;
using System.Collections;
using NUnit.Framework;

namespace GenTestAsm
{
    [TestFixture]
    public class PeFileTest
    {
        const string FILE_NAME = @"..\..\src\GenTestAsm\Tests\GuineaPigDll.dll";

        [Test]
        public void OpenFile()
        {
            PeFile peFile = new PeFile( FILE_NAME );
        }

        [Test]
        public void PeHeaderOffset()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 224, peFile.GetPeHeaderOffset() );
        }

        [Test]
        public void PeSignatureGood()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.IsTrue( peFile.IsValidPeFile() );
        }

        [Test]
        public void OptionalHeaderSize()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 224, peFile.GetOptionalHeaderSize() );
        }

        [Test]
        public void OptionalHeaderMagic()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 0x10b, peFile.GetOptionalHeaderMagic() ); // PE32
        }

        [Test]
        public void OptionalHeaderOffset()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 248, peFile.GetOptionalHeaderOffset() );
        }

        [Test]
        public void IsPe32()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.IsTrue(peFile.IsPe32());
            Assert.IsFalse(peFile.IsPe32Plus());
        }

        [Test]
        public void DataDirEntries()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 16, peFile.GetNumberOfDataDirEntries() );
        }

        [Test]
        public void ExportTableRva()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 219808, peFile.GetExportTableRva() );
        }

        [Test]
        public void ImageBase()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 0x10000000, peFile.GetImageBase() );
        }

        [Test]
        public void NumberOfSections()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 6, peFile.GetNumberOfSections() );
        }

        [Test]
        public void SectionTableStart()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 472, peFile.GetSectionTableStart() );
        }

        [Test]
        public void SectionStart()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 512, peFile.GetSectionStart(1) );
        }

        [Test]
        public void SectionVirtualBase()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 0x11000, peFile.GetSectionVirtualBase(1) );
        }

        [Test]
        public void SectionVirtualSize()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 127221, peFile.GetSectionVirtualSize(1) );
        }

        [Test]
        public void GetSectionContainingRva()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 1, peFile.GetSectionContainingRva( 0x11010 ) );
        }

        [Test]
        public void GetSectionContainingExportTable()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 2, peFile.GetSectionContainingRva( peFile.GetExportTableRva() ) );
        }

        [Test]
        public void SectionFilePointer()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 4096, peFile.GetSectionFilePointer(1) );
        }

        [Test]
        public void SectionFileSize()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 0x20000, peFile.GetSectionFileSize(1) );
        }

        [Test]
        public void FilePointerForRva()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 4097, peFile.GetFilePointerForRva(0x11001) );
        }

        [Test]
        public void FilePointerForExportTable()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 154272, peFile.GetFilePointerForRva( peFile.GetExportTableRva() ) );
        }

        [Test]
        public void ExportTableDirectoryOffset()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 154272, peFile.GetExportTableDirectoryOffset());
        }

        [Test]
        public void NumberOfExportedNames()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 3, peFile.GetNumberOfExportedNames());
        }

        [Test]
        public void ExportedNamesPointerTableRva()
        {                                                        
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 219860, peFile.GetExportedNamesPointerTableRva());
        }

        [Test]
        public void ExportedNamesPointerTableOffset()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( 154324, peFile.GetExportedNamesPointerTableOffset());
        }

        [Test]
        public void ExportedName()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            Assert.AreEqual( "?ExportC@@YAXXZ", peFile.GetExportedName(0));
        }

        [Test]
        public void NameList()
        {
            PeFile peFile = new PeFile( FILE_NAME );
            string[] names = peFile.GetExportedNames();
            string[] expected = 
                {
                    "?ExportC@@YAXXZ",
                    "ExportB",
                    "_ExportA@0"
                };

            Assert.AreEqual( expected.Length, names.Length );
            for (int i=0; i<expected.Length; ++i)
            {
                Assert.AreEqual( expected[i], names[i] );
            }
        }
    }
}
#endif