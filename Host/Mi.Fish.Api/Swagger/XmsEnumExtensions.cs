using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Xml.XPath;

namespace Mi.Fish.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class XmsEnumExtensions
    {
        private const string MemberXPath = "/doc/members/member[@name='{0}']";
        private const string SummaryXPath = "summary";
        private readonly Dictionary<string, XPathNavigator> _xPath;

        /// <summary>
        /// ctor
        /// </summary>
        public XmsEnumExtensions()
        {
            _xPath = new Dictionary<string, XPathNavigator>();
            foreach (var path in GetXmls())
            {
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    var xmlDoc = new XPathDocument(fs);
                    var key = path.Substring(AppContext.BaseDirectory.Length).Replace(".xml", string.Empty);
                    _xPath.Add(key, xmlDoc.CreateNavigator());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IReadOnlyList<string> GetXmls()
        {
            var baseDir = AppContext.BaseDirectory;

            return Directory.GetFiles(baseDir, "Mi.*.xml").ToImmutableList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeInfo"></param>
        /// <returns></returns>
        public List<XmsEnumValues> GetXmsEnumValues(TypeInfo typeInfo)
        {
            List<XmsEnumValues> list = new List<XmsEnumValues>();
            var assemblyName = typeInfo.Assembly.FullName.Split(',')[0];
            foreach (var memberInfo in typeInfo.DeclaredMembers)
            {
                var memberName = XmlCommentsMemberNameHelper.GetMemberNameForMember(memberInfo);
                XPathNavigator memberNode = null;
                if (_xPath.TryGetValue(assemblyName, out memberNode))
                {
                    memberNode = memberNode.SelectSingleNode(string.Format(MemberXPath, memberName));
                }
                if (memberNode == null)
                {
                    continue;
                }
                var summaryNode = memberNode.SelectSingleNode(SummaryXPath);
                if (summaryNode != null)
                {
                    var xms = new XmsEnumValues();
                    xms.Value = memberInfo.Name;
                    xms.Description = XmlCommentsTextHelper.Humanize(summaryNode.InnerXml);
                    list.Add(xms);
                }
            }
            return list;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class XmsEnumValues
    {
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
    }
}
