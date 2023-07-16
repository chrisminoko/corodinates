using corodinates.logic;
using corodinates.model;
using System;
using System.Collections.Generic;
using System.IO;

namespace corodinates
{
    public static class Program
    {
        static void Main(string[] args)
        {
            // Read and parse the binary data file
            List<VehiclePosition> positions = ReadBinaryDataFile("vehicle_positions.bin");

            // Build the KD tree
            KDTree kdTree = new KDTree();
            kdTree.Build(positions);

            // Find the nearest vehicle positions for the given coordinates
            List<(float, float)> coordinates = new List<(float, float)>
            {
                (34.544909f, -102.100843f),
                (32.345544f, -99.123124f),
                (33.234235f, -100.214124f),
                (35.195739f, -95.348899f),
                (31.895839f, -97.789573f),
                (32.895839f, -101.789573f),
                (34.115839f, -100.225732f),
                (32.335839f, -99.992232f),
                (33.535339f, -94.792232f),
                (32.234235f, -100.222222f)
            };

            foreach (var (latitude, longitude) in coordinates)
            {
                VehiclePosition nearest = kdTree.FindNearest(latitude, longitude);
                Console.WriteLine($"Nearest vehicle position: {nearest.VehicleId}, {nearest.VehicleRegistration}");
            }
        }

        static List<VehiclePosition> ReadBinaryDataFile(string filePath)
        {
            List<VehiclePosition> positions = new List<VehiclePosition>();

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
                    {
                        VehiclePosition position = new VehiclePosition
                        {
                            VehicleId = binaryReader.ReadInt32(),
                            VehicleRegistration = binaryReader.ReadCString(),
                            Latitude = binaryReader.ReadSingle(),
                            Longitude = binaryReader.ReadSingle(),
                            RecordedTimeUTC = binaryReader.ReadUInt64()
                        };

                        positions.Add(position);
                    }
                }
            }

            return positions;
        }

        static string ReadCString(this BinaryReader binaryReader)
        {
            List<byte> bytes = new List<byte>();
            byte b;

            while ((b = binaryReader.ReadByte()) != 0)
            {
                bytes.Add(b);
            }

            return System.Text.Encoding.ASCII.GetString(bytes.ToArray());
        }
    }
}



