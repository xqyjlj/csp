﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using CSP.Utils;
using CSP.Utils.Extensions;

namespace CSP.Modules.Pages.MCU.Models.Description
{
    [XmlRoot("Clock", IsNullable = false)]
    public class ClockModel
    {
        [XmlIgnore]
        public Dictionary<string, ControlModel> ControlMap { get; set; } = new();

        [XmlArray("Controls")]
        [XmlArrayItem("Control")]
        public List<ControlModel> Controls { get; set; }

        [XmlAttribute]
        public float Height { get; set; }

        [XmlIgnore] public Dictionary<int, RectModel> RectMap { get; set; } = new();

        [XmlArray("Rects")]
        [XmlArrayItem("Rect")]
        public List<RectModel> Rects { get; set; }

        [XmlAttribute]
        public float Width { get; set; }

        internal static ClockModel Load(string path) {
            DebugUtil.Assert(!path.IsNullOrEmpty(), new ArgumentNullException(nameof(path)), "path不能为空");
            DebugUtil.Assert(File.Exists(path), new FileNotFoundException(nameof(path)), $"{path}: 不存在");

            if (path == null) return null;
            if (!File.Exists(path)) return null;

            var deserializer = new XmlSerializer(typeof(ClockModel));
            var reader = new StreamReader(path);

            ClockModel rtn;
            try {
                rtn = (ClockModel)deserializer.Deserialize(reader);
            }
            catch (InvalidOperationException e) {
                MessageBox.Show(e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            DebugUtil.Assert(rtn != null, new ArgumentNullException(nameof(rtn)), "XML反序列化失败");
            if (rtn == null)
                return null;

            //给辅助变量赋值,将变量转化为字典形式
            foreach (var rect in rtn.Rects) {
                rtn.RectMap.Add(rect.ID, rect);
            }
            foreach (var control in rtn.Controls) {
                rtn.ControlMap.Add(control.Name, control);
            }

            return rtn;
        }

        public class ControlModel
        {
            [XmlAttribute]
            public int ID { get; set; }

            [XmlAttribute]
            public string Name { get; set; }

            [XmlAttribute]
            public string Type { get; set; }
        }

        public class RectModel
        {
            [XmlAttribute]
            public float Height { get; set; }

            [XmlAttribute]
            public int ID { get; set; }

            [XmlAttribute]
            public float Width { get; set; }

            [XmlAttribute]
            public float X { get; set; }

            [XmlAttribute]
            public float Y { get; set; }
        }
    }
}