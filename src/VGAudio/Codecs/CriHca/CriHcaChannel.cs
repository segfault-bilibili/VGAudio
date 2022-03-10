using System;
using System.Collections.Generic;
using VGAudio.Utilities;
using static VGAudio.Codecs.CriHca.CriHcaConstants;

namespace VGAudio.Codecs.CriHca
{
    public class CriHcaChannel
    {
        public ChannelType Type { get; set; }
        public int CodedScaleFactorCount { get; set; }
        public double[][] PcmFloat { get; } = Helpers.CreateJaggedArray<double[][]>(SubframesPerFrame, SamplesPerSubFrame);
        public double[][] Spectra { get; } = Helpers.CreateJaggedArray<double[][]>(SubframesPerFrame, SamplesPerSubFrame);
        public double[][] ScaledSpectra { get; } = Helpers.CreateJaggedArray<double[][]>(SamplesPerSubFrame, SubframesPerFrame);
        public int[][] QuantizedSpectra { get; } = Helpers.CreateJaggedArray<int[][]>(SubframesPerFrame, SamplesPerSubFrame);
        public double[] Gain { get; } = new double[SamplesPerSubFrame];
        public int[] Intensity { get; } = new int[SubframesPerFrame];
        public int[] HfrScales { get; } = new int[8];
        public double[] HfrGroupAverageSpectra { get; } = new double[8];
        public Mdct Mdct { get; } = new Mdct(SubFrameSamplesBits, CriHcaTables.MdctWindow, Math.Sqrt(2.0 / SamplesPerSubFrame));
        public int[] ScaleFactors { get; } = new int[SamplesPerSubFrame];
        public int[] Resolution { get; } = new int[SamplesPerSubFrame];
        public int HeaderLengthBits { get; set; }
        public int ScaleFactorDeltaBits { get; set; }

        public static string dumpArray(string indent, List<double[]> a)
        {
            var str = "\n" + indent + "[\n";
            var type = a.GetType().GetElementType();
            foreach (var e in a)
            {
                str += CriHcaChannel.dumpArray(indent + "    ", (Array)e);
            }
            return str + indent + "]\n";
        }
        public static string dumpArray(string indent, List<int[]> a)
        {
            var str = "\n" + indent + "[\n";
            var type = a.GetType().GetElementType();
            foreach (var e in a)
            {
                str += CriHcaChannel.dumpArray(indent + "    ", (Array)e);
            }
            return str + indent + "]\n";
        }
        public static string dumpArray(string indent, Array a){
            var str = "\n" + indent + "[\n";
            var type = a.GetType().GetElementType();
            if (type.Equals(typeof(double)) || type.Equals(typeof(uint)) || type.Equals(typeof(int)) || type.Equals(typeof(byte))) {
                foreach (var e in a) {
                    string v;
                    if (type.Equals(typeof(double)))
                    {
                        var v1 = (double)e;
                        v = v1.ToString("f8");
                    } else
                    {
                        v = e.ToString();
                    }
                    str += indent + "    " + v + ",\n";
                }
            }
            else if (type.IsArray)
            {
                foreach (var e in a)
                {
                    str += CriHcaChannel.dumpArray(indent + "    ", (Array)e);
                }
            }
            else throw new ArgumentOutOfRangeException("a.buffer not null");
            str += indent + "]\n";
            return str;
        }
        public string dump(string indent)
        {
            var str = "\n" + indent + "{\n";
            str += indent + "    " + "Type = " + ((int)this.Type).ToString() + "\n";
            str += indent + "    " + "CodedScaleFactorCount = " + this.CodedScaleFactorCount.ToString() + "\n";
            str += indent + "    " + "PcmFloat =" + CriHcaChannel.dumpArray(indent + "    ", this.PcmFloat);
            str += indent + "    " + "Spectra =" + CriHcaChannel.dumpArray(indent + "    ", this.Spectra);
            str += indent + "    " + "ScaledSpectra =" + CriHcaChannel.dumpArray(indent + "    ", this.ScaledSpectra);
            str += indent + "    " + "QuantizedSpectra =" + CriHcaChannel.dumpArray(indent + "    ", this.QuantizedSpectra);
            str += indent + "    " + "Gain =" + CriHcaChannel.dumpArray(indent + "    ", this.Gain);
            str += indent + "    " + "Intensity =" + CriHcaChannel.dumpArray(indent + "    ", this.Intensity);
            str += indent + "    " + "HfrScales =" + CriHcaChannel.dumpArray(indent + "    ", this.HfrScales);
            str += indent + "    " + "HfrGroupAverageSpectra =" + CriHcaChannel.dumpArray(indent + "    ", this.HfrGroupAverageSpectra);
            str += indent + "    " + "Mdct: HCAMdct = " + this.Mdct.dump(indent);
            str += indent + "    " + "ScaleFactors =" + CriHcaChannel.dumpArray(indent + "    ", this.ScaleFactors);
            str += indent + "    " + "Resolution =" + CriHcaChannel.dumpArray(indent + "    ", this.Resolution);
            str += indent + "    " + "HeaderLengthBits = " + this.HeaderLengthBits.ToString() + "\n";
            str += indent + "    " + "ScaleFactorDeltaBits = " + this.ScaleFactorDeltaBits.ToString() + "\n";
            str += indent + "}\n";
            return str;
        }
    }
}
