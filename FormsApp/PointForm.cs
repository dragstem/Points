using Newtonsoft.Json;
using PointLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Windows.Forms;
using System.Xml.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.TypeInspectors;

namespace FormsApp
{
    [Serializable]
    public partial class PointForm : Form
    {
        private Point[] points = null;
        public PointForm()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            points = new Point[5];

            var rnd = new Random();

            for (int i = 0; i < points.Length; i++)
                points[i] = rnd.Next(3) % 2 == 0 ? new Point() : new Point3D();

            listBox.DataSource = points;
        }


        private void btnSort_Click(object sender, System.EventArgs e)
        {
            if (points == null)
                return;

            Array.Sort(points);

            listBox.DataSource = null;
            listBox.DataSource = points;

        }

        private void btnSerialize_Click(object sender, EventArgs e)
        {
            var dlg = new SaveFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin|YAML|*.yaml|CUST|*.cust";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            using (var fs =
                new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        var bf = new BinaryFormatter();
                        bf.Serialize(fs, points);
                        break;
                    case ".soap":
                        var sf = new SoapFormatter();
                        sf.Serialize(fs, points);
                        break;
                    case ".xml":
                        var xf = new XmlSerializer(typeof(Point[]), new[] { typeof(Point3D) });
                        xf.Serialize(fs, points);
                        break;
                    case ".json":
                        var jf = new JsonSerializer
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        };
                        using (var w = new StreamWriter(fs))
                        using (var jw = new JsonTextWriter(w))
                        {
                            jf.Serialize(jw, points);
                        }
                        break;
                    case ".yaml":
                        var serializer = new SerializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
                        using (var writer = new StreamWriter(fs))
                        {
                            serializer.Serialize(writer, points);
                        }
                        break;
                    case ".cust":
                        var cf = new CustomFormatter();
                        cf.Serialize(fs, points);
                        break;


                }
            }
        }

        private void btnDesrialize_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "SOAP|*.soap|XML|*.xml|JSON|*.json|Binary|*.bin|YAML|*.yaml|CUST|*.cust";

            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            using (var fs =
                new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read))
            {
                switch (Path.GetExtension(dlg.FileName))
                {
                    case ".bin":
                        var bf = new BinaryFormatter();
                        points = (Point[])bf.Deserialize(fs);
                        break;
                    case ".soap":
                        var sf = new SoapFormatter();
                        points = (Point[])sf.Deserialize(fs);
                        break;
                    case ".xml":
                        var xf = new XmlSerializer(typeof(Point[]), new[] { typeof(Point3D) });
                        points = (Point[])xf.Deserialize(fs);
                        break;
                    case ".json":
                        var jf = new JsonSerializer
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        };
                        using (var r = new StreamReader(fs))
                        using (var jr = new JsonTextReader(r))
                        {
                            points = (Point[])jf.Deserialize(jr, typeof(Point[]));
                        }
                        break;
                    case ".yaml":
                        var deserializer = new DeserializerBuilder()
                            .WithNamingConvention(CamelCaseNamingConvention.Instance)
                            .Build();
                        using (var reader = new StreamReader(fs))
                        {
                            points = deserializer.Deserialize<Point[]>(reader);
                        }
                        break;
                    case ".cust":
                        var cf = new CustomFormatter();
                        points = (Point[])cf.Deserialize(fs);
                        break;


                }

                listBox.DataSource = null;
                listBox.DataSource = points;
            }



        }
    }
}
