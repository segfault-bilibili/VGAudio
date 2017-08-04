﻿using System.IO;
using VGAudio.Formats;

namespace VGAudio.Containers.Bxstm
{
    public class BfstmReader : AudioReader<BfstmReader, BfstmStructure, BfstmConfiguration>
    {
        protected override BfstmStructure ReadFile(Stream stream, bool readAudioData = true)
        {
            return (BfstmStructure)new BCFstmReader().ReadFile(stream, readAudioData);
        }

        protected override IAudioFormat ToAudioStream(BfstmStructure structure) => Common.ToAudioStream(structure);

        protected override BfstmConfiguration GetConfiguration(BfstmStructure structure)
        {
            var configuration = new BfstmConfiguration();
            if (structure.StreamInfo.Codec == BxstmCodec.Adpcm)
            {
                configuration.SamplesPerSeekTableEntry = structure.StreamInfo.SamplesPerSeekTableEntry;
            }
            configuration.Codec = structure.StreamInfo.Codec;
            configuration.Endianness = structure.Endianness;
            configuration.SamplesPerInterleave = structure.StreamInfo.SamplesPerInterleave;
            return configuration;
        }
    }
}