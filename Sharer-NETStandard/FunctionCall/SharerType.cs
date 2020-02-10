﻿using System;
using System.IO;

namespace Sharer.FunctionCall
{
    public enum SharerType : byte
    {
        @void,
        @bool,
        @byte,
        @short,
        @int,
        @uint,
        @long,
        @ulong,
        int64,
        uint64,
        @float,
        @double
    }

    public static class SharerTypeHelper
    {
        public static int Sizeof(SharerType type)
        {
            switch (type)
            {
                case SharerType.@bool:
                case SharerType.@byte:
                case SharerType.@short:
                    return 1;
                case SharerType.@int:
                case SharerType.@uint:
                    return 2;
                case SharerType.@long:
                case SharerType.@ulong:
                case SharerType.@float:
                    return 4;
                case SharerType.int64:
                case SharerType.uint64:
                case SharerType.@double:
                    return 8;
                default:
                    return 0;
            }
        }

        public static void Encode(SharerType type, BinaryWriter writer, object value)
        {
            if (value == null) throw new ArgumentNullException("value");

            switch (type)
            {
                case SharerType.@bool:
                    var boolValue = Boolean.Parse(value.ToString());
                    writer.Write((byte)(boolValue? 1:0));
                    break;
                case SharerType.@byte:
                    writer.Write(Byte.Parse(value.ToString()));
                    break;
                case SharerType.@short:
                    writer.Write(SByte.Parse(value.ToString()));
                    break;
                case SharerType.@int:
                    writer.Write(Int16.Parse(value.ToString()));
                    break;
                case SharerType.@uint:
                    writer.Write(UInt16.Parse(value.ToString()));
                    break;
                case SharerType.@long:
                    writer.Write(Int32.Parse(value.ToString()));
                    break;
                case SharerType.@ulong:
                    writer.Write(UInt32.Parse(value.ToString()));
                    break;
                case SharerType.@float:
                    writer.Write(float.Parse(value.ToString().Replace(",","."), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture));
                    break;
                case SharerType.int64:
                    writer.Write(Int64.Parse(value.ToString()));
                    break;
                case SharerType.uint64:
                    writer.Write(UInt64.Parse(value.ToString()));
                    break;
                case SharerType.@double:
                    writer.Write(double.Parse(value.ToString().Replace(",", "."), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture));
                    break;
                default:
                    throw new Exception("Type " + type.ToString() + " not supported");
            }
        }

        public static object Decode(SharerType type, byte[] buffer)
        {
            using (MemoryStream memory = new MemoryStream(buffer))
            {
                using (BinaryReader reader = new BinaryReader(memory))
                {
                    switch (type)
                    {
                        case SharerType.@bool:
                            return reader.ReadByte() != 0;
                        case SharerType.@byte:
                            return reader.ReadByte();
                        case SharerType.@short:
                            return reader.ReadSByte();
                        case SharerType.@int:
                            return reader.ReadInt16();
                        case SharerType.@uint:
                            return reader.ReadUInt16();
                        case SharerType.@long:
                            return reader.ReadInt32();
                        case SharerType.@ulong:
                            return reader.ReadUInt32();
                        case SharerType.@float:
                            return reader.ReadSingle();
                        case SharerType.int64:
                            return reader.ReadInt64();
                        case SharerType.uint64:
                            return reader.ReadUInt64();
                        case SharerType.@double:
                            return reader.ReadDouble();
                        default:
                            return null;
                    }
                }
            }
        }
    }
}
