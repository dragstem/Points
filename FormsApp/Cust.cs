using PointLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FormsApp
{
    public class CustomFormatter : IFormatter
    {
        public ISurrogateSelector SurrogateSelector { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SerializationBinder Binder { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public StreamingContext Context { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public object Deserialize(Stream serializationStream)
        {
            Point[] points = new Point[5];
            using (var reader = new StreamReader(serializationStream))
            {
                string line;
                int i = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    
                    var parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        var x = int.Parse(parts[0]);
                        var y = int.Parse(parts[1]);
                        points[i] = new Point(x, y);
                    }
                    else if (parts.Length == 3)
                    {
                        var x = int.Parse(parts[0]);
                        var y = int.Parse(parts[1]);
                        var z = int.Parse(parts[2]);
                        points[i] = new Point3D(x, y,z);
                    }
                    i++;
                }
            }
            return points;
        }

        public void Serialize(Stream serializationStream, object graph)
        {
            var points = (Point[])graph;
            using (var writer = new StreamWriter(serializationStream))
            {
                foreach (var point in points)
                {
                    if (point is Point3D point3D)
                    {
                        writer.WriteLine($"{point3D.X},{point3D.Y},{point3D.Z}");
                    }
                    else
                    {
                        writer.WriteLine($"{point.X},{point.Y}");
                    }
                }
            }
        }
    }

}
