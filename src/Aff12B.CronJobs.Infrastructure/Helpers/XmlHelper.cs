using System.Xml.Linq;

namespace Aff12B.CronJobs.Infrastructure.Helpers
{
    public static class XmlHelper
    {
        public static string BuildXml<T>(IEnumerable<T> data, string rootName, string tagName)
            where T : class
        {
            var xmlDoc = new XDocument(new XElement(rootName));
            var root = xmlDoc.Root;
            foreach (var item in data)
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(data), "Null item found in insert data");
                }
                var elem = new XElement(tagName);
                InsertAttribute(elem, item);
                root.Add(elem);
            }
            return xmlDoc.ToString();
        }

        private static void InsertAttribute<T>(XElement elem, T item)
            where T : class
        {
            var type = typeof(T);
            var propNames = type.GetProperties().Select(prop => prop.Name);
            foreach (var propName in propNames)
            {
                var value = type.GetProperty(propName).GetValue(item, null) ?? "";
                elem.Add(new XAttribute(propName.ToLowerInvariant(), value));
            }
        }
    }
}
