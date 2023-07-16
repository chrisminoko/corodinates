using corodinates.model;
using System;
using System.Collections.Generic;

namespace corodinates.logic
{
    public class KDTree
    {
        private class KDNode
        {
            public VehiclePosition Vehicle { get; set; }
            public KDNode Left { get; set; }
            public KDNode Right { get; set; }
        }

        private KDNode root;

        public void Build(List<VehiclePosition> positions)
        {
            root = BuildKDTree(positions, 0, positions.Count - 1, 0);
        }
        private KDNode BuildKDTree(List<VehiclePosition> positions, int start, int end, int depth)
        {
            if (start > end)
                return null;

            int axis = depth % 2;
            int mid = (start + end) / 2;

            positions.Sort((a, b) => axis == 0 ? a.Longitude.CompareTo(b.Longitude) : a.Latitude.CompareTo(b.Latitude));

            var node = new KDNode
            {
                Vehicle = positions[mid],
                Left = BuildKDTree(positions, start, mid - 1, depth + 1),
                Right = BuildKDTree(positions, mid + 1, end, depth + 1)
            };

            return node;
        }
        public VehiclePosition FindNearest(float latitude, float longitude)
        {
            if (root == null)
                return null;

            float bestDistance = float.MaxValue;
            VehiclePosition nearest = null;

            FindNearest(root, latitude, longitude, 0, ref bestDistance, ref nearest);

            return nearest;
        }


        private void FindNearest(KDNode node, float latitude, float longitude, int depth, ref float bestDistance, ref VehiclePosition nearest)
        {
            if (node == null)
                return;

            float distance = CalculateDistance(latitude, longitude, node.Vehicle.Latitude, node.Vehicle.Longitude);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                nearest = node.Vehicle;
            }

            int axis = depth % 2;

            if (axis == 0 ? longitude < node.Vehicle.Longitude : latitude < node.Vehicle.Latitude)
            {
                FindNearest(node.Left, latitude, longitude, depth + 1, ref bestDistance, ref nearest);
                if (axis == 0 ? (longitude + bestDistance) >= node.Vehicle.Longitude : (latitude + bestDistance) >= node.Vehicle.Latitude)
                    FindNearest(node.Right, latitude, longitude, depth + 1, ref bestDistance, ref nearest);
            }
            else
            {
                FindNearest(node.Right, latitude, longitude, depth + 1, ref bestDistance, ref nearest);
                if (axis == 0 ? (longitude - bestDistance) <= node.Vehicle.Longitude : (latitude - bestDistance) <= node.Vehicle.Latitude)
                    FindNearest(node.Left, latitude, longitude, depth + 1, ref bestDistance, ref nearest);
            }
        }

        private float CalculateDistance(float lat1, float lon1, float lat2, float lon2)
        {
            float r = 6371; // Earth's radius in kilometers

            float dLat = DegreeToRadian(lat2 - lat1);
            float dLon = DegreeToRadian(lon2 - lon1);

            float a = MathF.Sin(dLat / 2) * MathF.Sin(dLat / 2) +
                      MathF.Cos(DegreeToRadian(lat1)) * MathF.Cos(DegreeToRadian(lat2)) *
                      MathF.Sin(dLon / 2) * MathF.Sin(dLon / 2);

            float c = 2 * MathF.Atan2(MathF.Sqrt(a), MathF.Sqrt(1 - a));

            return r * c;
        }

        private float DegreeToRadian(float degree)
        {
            return degree * (MathF.PI / 180);
        }

    }
}
