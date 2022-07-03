﻿using CSP.Utils.Extensions;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace CSP.Database.Models.MCU
{
    [XmlRoot("IP", IsNullable = false)]
    public class IPModel
    {
        [XmlElement("GPIO")]
        public IpGpioModel GPIO { get; set; }

        internal static IPModel Load(string path)
        {
            if (path.IsNullOrEmpty())
                Log.Error(new ArgumentNullException(nameof(path)), $"路径 \"{path}\" 不存在");

            if (!File.Exists(path))
                return null;

            var deserializer = new XmlSerializer(typeof(IPModel));
            var reader = new StreamReader(path);

            IPModel rtn;
            try
            {
                rtn = (IPModel)deserializer.Deserialize(reader);
                if (rtn == null)
                {
                    Log.Error(new ArgumentNullException(nameof(rtn)), "XML反序列化失败");
                    return null;
                }
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            //给辅助变量赋值,将变量转化为字典形式
            foreach (var mode in rtn.GPIO.ModesTemp)
            {
                foreach (var parameter in mode.ParametersTemp)
                {
                    mode.Parameters.Add(parameter.Enumerate, parameter);
                }

                rtn.GPIO.Modes.Add(mode.Name, mode);
            }

            return rtn;
        }

        public class IpGpioModel
        {
            [XmlIgnore]
            public Dictionary<string, IpGpioModeModel> Modes { get; } = new();

            [XmlArray("Modes")]
            [XmlArrayItem("Mode")]
            public IpGpioModeModel[] ModesTemp { get; set; }

            public class IpGpioModeModel
            {
                [XmlAttribute("Name")]
                public string Name { get; set; }

                [XmlIgnore]
                public Dictionary<string, IpGpioModeParameterModel> Parameters { get; } = new();

                [XmlArray("Parameters")]
                [XmlArrayItem("Parameter")]
                public IpGpioModeParameterModel[] ParametersTemp { get; set; }

                [XmlAttribute("Type")]
                public string Type { get; set; }

                public class IpGpioModeParameterModel
                {
                    [XmlAttribute("Enumerate")]
                    public string Enumerate { get; set; }

                    [XmlArray("Values")]
                    [XmlArrayItem("Value")]
                    public string[] Values { get; set; }
                }
            }
        }
    }
}