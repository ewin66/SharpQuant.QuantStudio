﻿ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SharpQuant.Common
{
    public class SettingsDictionary : Dictionary<string, string>, IXmlSerializable
    {
        protected static string _valueSeparator = "]=[";
        protected static string _itemSeparator = "\n";

        public SettingsDictionary()
        {
        }

        public void SetValue<T>(string key, T value)
        {
            base[key] = value!=null ? value.ToString() : null;
        }

        public T GetValue<T>(string key)
        {
            string value = base[key];
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public T GetOrSet<T>(string key, T defaultValue)
        {
            string value;
            if (!base.TryGetValue(key, out value))
            {
                value = defaultValue.ToString();
                base.Add(key, value);
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }


		#region simple string serialization
        public static SettingsDictionary Deserialize(string s)
        {
            SettingsDictionary list = new SettingsDictionary();
            try
            {
                if (!string.IsNullOrEmpty(s))
                {
                    string[] items = s.Split(new string[] { _itemSeparator }, StringSplitOptions.None);
                    foreach (var item in items)
                    {
                        int pos = item.IndexOf(_valueSeparator);
                        string key = item.Substring(1, pos - 1);
                        string value = item.Substring(pos + 3, item.Length - pos - 4);

                        list.Add(key,value);
                    }
                }
            }
            catch
            {           
            }
            return list;
        }

        public virtual string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in this)
            {
                sb.Append("["); 
                sb.Append(item.Key); 
                sb.Append(_valueSeparator); 
                sb.Append(item.Value);
                sb.Append("]"); 
                sb.Append(_itemSeparator);
            }
            if (sb.Length>1) return sb.ToString(0, sb.Length - 1);
            return string.Empty;
        }
		
		#endregion
		
		#region IXmlSerializable
		public void ReadXml(System.Xml.XmlReader reader)
	    {
	        XmlSerializer keySerializer = new XmlSerializer(typeof(string));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(string));

	        bool wasEmpty = reader.IsEmptyElement;
	        reader.Read();
	        if (wasEmpty) return;

	        while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
	        {
	            reader.ReadStartElement("item");
	            reader.ReadStartElement("key");
                string key = (string)keySerializer.Deserialize(reader);
	            reader.ReadEndElement();

	            reader.ReadStartElement("value");
                string value = (string)valueSerializer.Deserialize(reader);
	            reader.ReadEndElement();
	            this.Add(key, value);

	            reader.ReadEndElement();
	            reader.MoveToContent();
	        }
	        reader.ReadEndElement();
	    }


	    public void WriteXml(System.Xml.XmlWriter writer)
	    {
            XmlSerializer keySerializer = new XmlSerializer(typeof(string));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(string));

            foreach (var kv in this)
	        {
	            writer.WriteStartElement("item");
	            writer.WriteStartElement("key");
	            keySerializer.Serialize(writer, kv.Key);
	            writer.WriteEndElement();

	            writer.WriteStartElement("value");
	            valueSerializer.Serialize(writer, kv.Value);
	            writer.WriteEndElement();
	            writer.WriteEndElement();
	        }
	    }
		
		public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
		#endregion
		
    }
}

 
 
