using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class XmlSave
{
    public static void Save(string _savePath, string _xmlFileName, Dictionary<string, object> _content)
    {
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement root = xmlDocument.DocumentElement;

        Dictionary<string, object> _dict = new Dictionary<string, object>();
        _dict.Add("body", _content);

        BuildTree(xmlDocument, _dict, null);

        xmlDocument.Save(_savePath + "/" + _xmlFileName);
    }

    static void BuildTree(XmlDocument _xmlDoc, Dictionary<string, object> _content, XmlElement _parent)
    {
        foreach (KeyValuePair<string, object> _kvp in _content)
        {
            //Debug.Log($"key: {_kvp.Key}, value: {_kvp.Value}");

            XmlElement _child = _xmlDoc.CreateElement(_kvp.Key);

            if (_parent != null)
            {
                _parent.AppendChild(_child);
            }

            else
            {
                _xmlDoc.AppendChild(_child);
            }

            try
            {
                BuildTree(_xmlDoc, (Dictionary<string, object>)_kvp.Value, _child);
            }

            catch
            {
                XmlText _valueText = _xmlDoc.CreateTextNode(_kvp.Value.ToString());
                _child.AppendChild(_valueText);
            }
        }
    }

    static void code()
    {
        XmlDocument xmlDocument = new XmlDocument();
        XmlElement root = xmlDocument.DocumentElement;

        XmlElement newBody = xmlDocument.CreateElement("body");

        XmlElement newSection = xmlDocument.CreateElement("section");
        XmlElement newTitle = xmlDocument.CreateElement("Title");

        XmlText title = xmlDocument.CreateTextNode("here we put the title");

        newSection.AppendChild(newTitle);
        newTitle.AppendChild(title);

        xmlDocument.AppendChild(newSection);


        //xmlDocument.Save(_savePath + "/" + "Parameters.xml");
    }
}
