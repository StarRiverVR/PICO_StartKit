/**
 * @brief convert xml data to binary
*/
using System;
using System.IO;
using System.Security;
using System.Collections;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
// 只允许编辑器环境下测试用
// 不要随意打开 否则ios执行档又要增加很多了
using System.Xml;
#endif

namespace com.vivo.codelibrary
{
    public static class SecurityTools
    {
        enum ESecurityElementType
        {
            Root,
            Children
        }

        public static bool ConvertXmlFileToBinaryFile(string InXmlPath, string InBinaryPath)
        {
            string XmlText = File.ReadAllText(InXmlPath, System.Text.Encoding.UTF8);

            if (string.IsNullOrEmpty(XmlText))
            {
                VLog.Error($"{InXmlPath} is not an valid xml file.");
                return false;
            }

            return ConvertXmlTextToBinaryFile(XmlText, InBinaryPath);
        }

        public static bool ConvertXmlTextToBinaryFile(string InXmlText, string InBinaryPath)
        {
            SecurityParser Parser = new SecurityParser();

            Parser.LoadXml(InXmlText);

            SecurityElement Root = Parser.root;

            return ConvertSecurityElementToBinaryFile(Root, InBinaryPath);
        }

        public static bool ConvertSecurityElementToBinaryFile(SecurityElement InRoot, string InBinaryPath)
        {
            using (FileStream fs = File.OpenWrite(InBinaryPath))
            {
                using (BinaryWriter Writer = new BinaryWriter(fs))
                {
                    Write(Writer, InRoot, (byte)ESecurityElementType.Root);
                }
            }

            return true;
        }

        private static void WriteString( this BinaryWriter InWriter, string InText )
        {
            if( InText != null )
            {
                InWriter.Write(InText);
            }
            else
            {
                InWriter.Write("");
            }
        }

        private static void Write(
            BinaryWriter InWriterRef,
            SecurityElement InElement,
            byte InType)
        {
            InWriterRef.Write(InType);

            // base information
            InWriterRef.WriteString(InElement.Tag);
            InWriterRef.WriteString(InElement.Text);

            // attributes
            Hashtable Attributes = InElement.Attributes;
            int AttributesCount = Attributes != null ? Attributes.Count : 0;
            InWriterRef.Write(AttributesCount);

            if (Attributes != null)
            {
                var AttIter = Attributes.GetEnumerator();
                while (AttIter.MoveNext())
                {
                    string Key = AttIter.Key as string;
                    string Value = AttIter.Value as string;

                    if (Key==null || Value==null)
                    {
                        VLog.Error("Invalid Attributes");
                    }

                    InWriterRef.WriteString(Key);
                    InWriterRef.WriteString(Value);
                }
            }


            // childrens.
            ArrayList Children = InElement.Children;

            int ChildrenCount = Children != null ? Children.Count : 0;
            InWriterRef.Write(ChildrenCount);

            if( Children != null )
            {
                var ChildIter = Children.GetEnumerator();

                while (ChildIter.MoveNext())
                {
                    SecurityElement Child = ChildIter.Current as SecurityElement;

                    if (Child==null)
                    {
                        VLog.Error("Invalid Security Element.");
                    }

                    Write(InWriterRef, Child, (byte)ESecurityElementType.Children);
                }
            }            
        }

        /////////////////////////////////////////////////////////
        // loading
        public static SecurityParser LoadXmlFromBinaryFile( string InPath )
        {
            SecurityParser Parser = new SecurityParser();
            
            if( !LoadXmlFromBinaryFile(Parser, InPath) )
            {
                return null;
            }

            return Parser;
        }

        public static bool LoadXmlFromBinaryFile( SecurityParser InParser, string InPath )
        {
            byte[] FileBytes = File.ReadAllBytes(InPath);

            return LoadXmlFromBinaryBuffer(InParser, FileBytes, InPath);
        }

        public static bool LoadXmlFromBinaryBuffer(SecurityParser InParser, byte[] InFileBytes, string InPath = "" )
        {
            if (InFileBytes == null || InFileBytes.Length < 4)
            {
                return false;
            }

            using (MemoryStream ms = new MemoryStream(InFileBytes))
            {
                using (BinaryReader Reader = new BinaryReader(ms))
                {
                    SecurityElement Root = LoadRootSecurityElement(Reader);

                    if (Root==null)
                    {
                        VLog.Error($"Failed load root Security Element in file: {InPath}");
                    }

                    if (Root != null)
                    {
                        InParser.root = Root;

                        return true;
                    }
                }
            }

            return false;
        }

        private static SecurityElement LoadRootSecurityElement(BinaryReader InReader)
        {
            try
            {
                SecurityElement Root = LoadSecurityElementChecked(InReader, (byte)ESecurityElementType.Root, null);

                return Root;
            }
            catch(Exception e)
            {
                VLog.Exception(e);
                return null;
            }
        }

        private static SecurityElement LoadSecurityElementChecked(BinaryReader InReader, byte InType, SecurityElement InParent)
        {
            byte ReadedType = InReader.ReadByte();

            if( ReadedType != InType )
            {
                return null;
            }

            string Tag = InReader.ReadString();
            string Text = InReader.ReadString();

            SecurityElement Element = new SecurityElement(Tag, Text);

            int AttributesCount = InReader.ReadInt32();

            if (AttributesCount>= 512)
            {
                VLog.Error("too many attributes.");
            }

            for( int i=0; i<AttributesCount; ++i )
            {
                string Key = InReader.ReadString();
                string Value = InReader.ReadString();

                Element.AddAttribute(Key, Value);
            }

            int ChildrenCount = InReader.ReadInt32();

            if (ChildrenCount>= 515)
            {
                VLog.Error("too many children");
            }

            for( int i=0; i<ChildrenCount; ++i )
            {
                SecurityElement ChildElement = LoadSecurityElementChecked(InReader, (byte)ESecurityElementType.Children, Element);

                if (ChildElement==null)
                {
                    VLog.Error("invalid child element");
                }

            }

            if( InParent != null )
            {
                InParent.AddChild(Element);
            }

            return Element;
        }

#if UNITY_EDITOR
// 只允许编辑器环境下测试用
// 不要随意打开 否则ios执行档又要增加很多了
        // 写完了之后发现直接ToString就可以了，艹，先留着吧
        public static void DumpSecurityElementToXml(SecurityElement InRoot, string InPath)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            DumpSecurityElement(doc, null, InRoot);

            doc.Save(InPath);
        }

        private static void DumpSecurityElement(XmlDocument InDocument, XmlNode InParent, SecurityElement InElement)
        {
            XmlNode node = InDocument.CreateElement(InElement.Tag);
            node.InnerText = InElement.Text;

            if( InParent != null )
            {
                InParent.AppendChild(node);
            }
            else
            {
                InDocument.AppendChild(node);
            }

            if (InElement.Attributes != null)
            {
                var Iter = InElement.Attributes.GetEnumerator();
                
                while( Iter.MoveNext() )
                {
                    XmlAttribute XmlAttr = InDocument.CreateAttribute(Iter.Key as string);
                    XmlAttr.Value = Iter.Value as string;

                    node.Attributes.Append(XmlAttr);
                }
            }

            if (InElement.Children != null )
            {
                foreach (SecurityElement Element in InElement.Children)
                {
                    DumpSecurityElement(InDocument, node, Element);
                }
            }            
        }
#endif  
    }
}