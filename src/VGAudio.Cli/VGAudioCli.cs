using System;

namespace VGAudio.Cli
{
    public static class VGAudioCli
    {
        private static readonly string[] tableNames = {
            "QuantizeSpectrumBits",
            "QuantizeSpectrumValue",
            "QuantizedSpectrumBits",
            "QuantizedSpectrumMaxBits",
            "QuantizedSpectrumValue",
            "ScaleToResolutionCurve",
            "AthCurve",
            "MdctWindow",
            "DefaultChannelMapping",
            "ValidChannelMappings"
        };
        public static int Main(string[] args)
        {
            Array[] arrays = Utilities.ArrayUnpacker.UnpackArrays(Codecs.CriHca.CriHcaTables.PackedTables);
            var i = 0;
            foreach (var name in tableNames)
            {
                writeToConsole(name, "", arrays[i++], false);
            }
            writeToConsole("DequantizerScalingTable", "", Codecs.CriHca.CriHcaTables.DequantizerScalingTable, false);
            writeToConsole("QuantizerStepSize", "", Codecs.CriHca.CriHcaTables.QuantizerStepSize, false);
            writeToConsole("QuantizerDeadZone", "", Codecs.CriHca.CriHcaTables.QuantizerDeadZone, false);
            writeToConsole("QuantizerScalingTable", "", Codecs.CriHca.CriHcaTables.QuantizerScalingTable, false);
            writeToConsole("QuantizerInverseStepSize", "", Codecs.CriHca.CriHcaTables.QuantizerInverseStepSize, false);
            writeToConsole("ResolutionMaxValues", "", Codecs.CriHca.CriHcaTables.ResolutionMaxValues, false);
            writeToConsole("IntensityRatioTable", "", Codecs.CriHca.CriHcaTables.IntensityRatioTable, false);
            writeToConsole("IntensityRatioBoundsTable", "", Codecs.CriHca.CriHcaTables.IntensityRatioBoundsTable, false);
            writeToConsole("ScaleConversionTable", "", Codecs.CriHca.CriHcaTables.ScaleConversionTable, false);
            return Converter.RunConverterCli(args) ? 0 : 1;
        }

        private static string getTypeName(Type t)
        {
            if (t.Equals(typeof(sbyte))) return "Int8Array";
            else if (t.Equals(typeof(byte))) return "Uint8Array";
            else if (t.Equals(typeof(double))) return "Float64Array";
            else if (t.Equals(typeof(int))) return "Int32Array";
            else throw new ArgumentOutOfRangeException();
        }

        private static void writeToConsole(string name, string indent, Array a, bool isElement)
        {
            Type type = a.GetType().GetElementType();
            string ending;
            if (type.IsArray)
            {
                Type type_ = ((Array[])a)[0].GetType().GetElementType();
                string typename = getTypeName(type_);
                if (name == null) throw new ArgumentOutOfRangeException();
                Console.WriteLine(indent + "static readonly " + name + ": " + typename + "[] = [");
                ending = "]";
            }
            else
            {
                string typename = getTypeName(type);
                Console.WriteLine(indent + (name != null ? "static readonly " + name + " = " : "") + "new " + typename + "(" + (typename == "Float64Array" ? "this.adaptEndianness6432(new Uint32Array([" : "["));
                ending = (typename == "Float64Array" ? "])).buffer)" : "])");
            }

            if (type.IsArray)
            {
                foreach (var aa in a)
                {
                    writeToConsole(null, indent + "    ", (Array) aa, true);
                }
            }
            else
            {
                int colNum = type.Equals(typeof(double)) ? 4 : type.Equals(typeof(int)) ? 8 : 16;
                for (var line = 0; line * colNum < a.Length; line++)
                {
                    Console.Write(indent + "    ");
                    for (var col = 0; col < colNum && line * colNum + col < a.Length; col++)
                    {
                        var i = line * colNum + col;
                        if (type.Equals(typeof(sbyte)))
                        {
                            byte e = ((byte[])a)[i];
                            Console.Write(e > 127 ? "-0x" + ((byte)(256 - e)).ToString("X2") : "0x" + e.ToString("X2"));
                            //Console.Write("0x" + e.ToString("X2"));
                        }
                        else if (type.Equals(typeof(byte)))
                        {
                            byte e = ((byte[])a)[i];
                            Console.Write("0x" + e.ToString("X2"));
                        }
                        else if (type.Equals(typeof(double)))
                        {
                            double[] e = { ((double[])a)[i] };
                            uint[] e_ = new uint[2];
                            Buffer.BlockCopy(e, 0, e_, 0, sizeof(double));
                            Console.Write("0x" + e_[0].ToString("X8") + ", 0x" + e_[1].ToString("X8"));
                        }
                        else if (type.Equals(typeof(int)))
                        {
                            uint e = ((uint[])a)[i];
                            //Console.Write(e > Int32.MaxValue ? "-0x" + ((uint)((ulong)UInt32.MaxValue + 1 - e)).ToString("X8") : "0x" + e.ToString("X8"));
                            Console.Write("0x" + e.ToString("X8"));
                        }
                        else throw new ArgumentOutOfRangeException();
                        if (i + 1 < a.Length)
                        {
                            Console.Write("," + ((col + 1 == colNum) ? "" : " "));
                        }
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine(indent + ending + (name == null ? "," : "") + (isElement ? "" : ";"));
        }
    }
}
