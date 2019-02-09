using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;

// From DEngine.Data.Yaml
namespace SimpleGui
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal class YamlIgnoreAttribute : Attribute
    {
        public YamlIgnoreAttribute() { }
    }

    internal class TinyYamlNode
    {
        public string Name;
        public string Value;
        public string Comment;
        
        public List<TinyYamlNode> Nodes = new List<TinyYamlNode>();
    }

    internal static class TinyYaml
    {
        private static Assembly DataAssembly = Assembly.Load("SimpleGui");

        // Convert an object to TinyYaml string
        public static string ToYaml<T>(this T obj)
        {
            var nodes = ObjectToNodes(obj);
            return NodesToYaml(nodes);
        }

        // Write an object to TinyYaml file
        public static void ToYamlFile<T>(this T obj, string filename)
        {
            var yaml = ToYaml(obj);

            if (string.IsNullOrWhiteSpace(yaml))
                return;

            try
            {
                File.WriteAllText(filename, yaml);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    
        // Read a Yaml file and create an object
        public static T FromYamlFile<T>(string filename) where T : new()
        {
            var nodes = FileToNodes(filename);
            var result = new T();
            var type = typeof(T);

            if (IsDictionary(type))
            {
                return (T)NodesToDictionary(nodes, type.GenericTypeArguments[0], type.GenericTypeArguments[1]);
            }
            else if (IsList(type))
            {
                return (T)NodesToList(nodes, type.GenericTypeArguments[0]);
            }
            
            PopulateObject(result, nodes);
            return result;
        }

        // Read a Yaml file and create an object
        public static IList FromYamlFile(string filename, Type t)
        {
            var nodes = FileToNodes(filename);
            return (IList)NodesToList(nodes, t);
        }

        // Convert a dictionary object into yaml nodes
        private static List<TinyYamlNode> DictionaryToNodes(IDictionary obj)
        {
            var dataTypes = DataAssembly.DefinedTypes;
            var list = new List<TinyYamlNode>();
            foreach (DictionaryEntry f in obj)
            {
                var type = f.Value.GetType();
                var strVal = TypeToString(f.Value, type, type.BaseType);

                var node = new TinyYamlNode()
                {
                    Name = f.Key.ToString(),
                    Value = strVal,
                    // TODO: Comment
                };
                list.Add(node);

                if (string.IsNullOrWhiteSpace(strVal))
                {
                    // It might be a type from DEngine.Data
                    // TODO: Repeated from ObjectToNodes
                    if (dataTypes.Any(x => x.Name == type.Name))
                    {
                        // It's a custom object, traverse
                        var childList = ObjectToNodes(f.Value);
                        node.Nodes.AddRange(childList);
                    }
                }

            }

            return list;
        }

        // Convert a list object into yaml nodes
        private static List<TinyYamlNode> ListToNodes(IList obj)
        {
            var dataTypes = DataAssembly.DefinedTypes;
            var result = new List<TinyYamlNode>();
            int index = 0;
            foreach (var f in obj)
            {
                var type = f.GetType();
                var strVal = TypeToString(f, type, type.BaseType);

                var node = new TinyYamlNode()
                {
                    Name = string.Empty,
                    Value = strVal,
                    // TODO: Comment
                };
                result.Add(node);
                index++;

                if (string.IsNullOrWhiteSpace(strVal))
                {
                    // It might be a type from DEngine.Data
                    // TODO: Repeated from ObjectToNodes
                    if (dataTypes.Any(x => x.Name == type.Name))
                    {
                        // It's a custom object, traverse
                        var childList = ObjectToNodes(f);
                        node.Nodes.AddRange(childList);
                    }
                }

            }

            return result;
        }

        // Convert a list of yaml nodes to a dictionary object of the desired types
        private static IDictionary NodesToDictionary(List<TinyYamlNode> list, Type dType1, Type dType2)
        {
            var dataTypes = DataAssembly.DefinedTypes;
            var dictType = typeof(Dictionary<,>).MakeGenericType(dType1, dType2);
            var dict = (IDictionary)Activator.CreateInstance(dictType);

            foreach (var f in list)
            {
                var nameIsEnum = dType1.BaseType.Name == "Enum";
                var valueIsEnum = dType2.BaseType.Name == "Enum";
                var valueIsData = dataTypes.Contains(dType2);
                object dataValue = null;
                if (valueIsData)
                {
                    dataValue = Activator.CreateInstance(dType2);
                    PopulateObject(dataValue, f.Nodes);
                }

                var key = nameIsEnum ? Convert.ChangeType(Enum.Parse(dType1, f.Name), dType1) : Convert.ChangeType(f.Name, dType1);
                var value = valueIsEnum ? Convert.ChangeType(Enum.Parse(dType2, f.Value), dType2) :
                        valueIsData ? dataValue : Convert.ChangeType(f.Value, dType2);

                dict.Add(key, value);
            }

            return dict;
        }

        // Convert a list of yaml nodes to a list object of the desired types
        // Only supports classes
        private static IList NodesToList(List<TinyYamlNode> list, Type dType)
        {
            var dataTypes = DataAssembly.DefinedTypes;
            var dictType = typeof(List<>).MakeGenericType(dType);
            var dict = (IList)Activator.CreateInstance(dictType);

            foreach (var f in list)
            {
                var isEnum = dType.BaseType.Name == "Enum";
                var isData = dataTypes.Contains(dType);
                object dataValue = null;
                if (isData)
                {
                    dataValue = Activator.CreateInstance(dType);
                    PopulateObject(dataValue, f.Nodes);
                    dict.Add(dataValue);
                }
            }

            return dict;
        }

        private static bool IsDictionary(Type type) { return type.Name.StartsWith("Dictionary") || type.Name.StartsWith("IDictionary"); }

        private static bool IsList(Type type) { return type.Name.StartsWith("List") || type.Name.StartsWith("IList"); }

        // Convert an object to yaml nodes
        private static List<TinyYamlNode> ObjectToNodes(object obj)
        {
            var list = new List<TinyYamlNode>();
            var type = obj.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var dataTypes = DataAssembly.DefinedTypes;

            if (IsDictionary(type))
                return DictionaryToNodes(obj as IDictionary);
            else if (IsList(type))
                return ListToNodes(obj as IList);

            var objToNodes = new Action<Type, object, string>((fieldType, value, name) =>
            {
                var baseType = fieldType.BaseType;
                var valueStr = TypeToString(value, fieldType, baseType);

                var node = new TinyYamlNode()
                {
                    Name = name,
                    Value = valueStr,
                    // TODO: Comment
                };
                list.Add(node);
                if (string.IsNullOrWhiteSpace(valueStr))
                {
                    if (IsDictionary(fieldType))
                    {
                        var childList = DictionaryToNodes(value as IDictionary);
                        node.Nodes.AddRange(childList);
                    }
                    else if (IsList(fieldType))
                    {
                        var childList = ListToNodes(value as IList);
                        node.Nodes.AddRange(childList);
                    }
                    // It might be a type from DEngine.Data
                    else if (dataTypes.Any(x => x.Name == fieldType.Name))
                    {
                        if (value != null)
                        {
                            var childList = ObjectToNodes(value);
                            node.Nodes.AddRange(childList);
                        }
                    }
                }
            });

            foreach (var f in fields)
            {
                if (Attribute.IsDefined(f, typeof(YamlIgnoreAttribute)))
                    continue;

                var value = f.GetValue(obj);
                objToNodes(f.FieldType, value, f.Name);
            }
            foreach (var f in properties)
            {
                if (Attribute.IsDefined(f, typeof(YamlIgnoreAttribute)))
                    continue;

                if (!f.CanWrite)
                    continue;

                var value = f.GetValue(obj);
                objToNodes(f.PropertyType, value, f.Name);
            }
            return list;
        }

        /// <summary>
        /// Convert a file to yaml nodes.
        /// Parses the file line by line.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static List<TinyYamlNode> FileToNodes(string filename)
        {
            TinyYamlNode rootNode = new TinyYamlNode();

            var lastNodeAtLevel = new Func<int, TinyYamlNode>(desiredLevel =>
            {
                TinyYamlNode lastNode = null;
                int currentLevel = 0;

                if (desiredLevel == currentLevel)
                    return rootNode;

                if (desiredLevel < 0)
                    return null;

                lastNode = rootNode;
                while (currentLevel < desiredLevel)
                {
                    if (lastNode.Nodes.Count == 0)
                        return null;

                    lastNode = lastNode.Nodes.Last();
                    currentLevel++;
                }
                return lastNode;
            });


            var lines = File.ReadAllLines(filename);
            int lineNum = 0;
            int level = 0;
            foreach (var line in lines)
            {
                ++lineNum;
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string valBuf = string.Empty;
                string name = string.Empty;
                string value = string.Empty;
                string comment = string.Empty;

                var newLine = line.TrimEnd();
                int indexOfComment = newLine.IndexOf('#');

                if (indexOfComment >= 0)
                {
                    var commentLine = newLine.Substring(indexOfComment).Trim();
                    comment = commentLine.Substring(1).Trim();
                    newLine = newLine.Replace(commentLine, string.Empty).TrimEnd();
                }

                // Get depth
                var tabCount = newLine.Length - newLine.Replace("\t", string.Empty).Length;
                if (tabCount > level + 1)
                    throw new ArgumentException("Error at line " + lineNum + ": Improper indent.");

                level = tabCount;

                newLine = newLine.Trim();


                int indexOfColon = newLine.IndexOf(':');

                //if (indexOfColon == 0)
                //    throw new ArgumentException("Error at line " + lineNum + ": Line can't begin with a ':'.");

                //if (newLine.Length - newLine.Replace(":", "").Length > 1)
                //    throw new ArgumentException("Error at line " + lineNum + ": Too many colons.");

                if (indexOfColon > 0)
                {
                    name = newLine.Substring(0, indexOfColon).Trim();
                    value = newLine.Substring(indexOfColon + 1).Trim();
                }
                else
                {
                    // Name only
                    name = newLine;
                }

                //if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(comment))
                //    throw new ArgumentException("Error at line " + lineNum + ": Both name and comment was missing.");

                TinyYamlNode node = new TinyYamlNode()
                {
                    Name = name,
                    Value = value,
                    Comment = comment,
                };
                
                if (tabCount == 0)
                {
                    rootNode.Nodes.Add(node);
                }
                else
                {
                    var parentNode = lastNodeAtLevel(level);
                    parentNode.Nodes.Add(node);
                }
            }
            
            return rootNode.Nodes;
        }

        /// <summary>
        /// Convert a list of yaml nodes to yaml string output
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static string NodesToYaml(List<TinyYamlNode> list)
        {
            var lineList = new List<string>();

            foreach (var n in list)
            {
                IterateNodesAsYaml(n, 0, ref lineList);
            }

            var result = String.Join("\r\n", lineList);
            return result;
        }

        /// <summary>
        /// Populate an object from yaml nodes
        /// </summary>
        /// <param name="result"></param>
        /// <param name="list"></param>
        private static void PopulateObject(object result, List<TinyYamlNode> list)
        {
            var type = result.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var dataTypes = DataAssembly.DefinedTypes;

            foreach (var n in list)
            {
                var populateObjectField = new Func<Type, object>((fieldType) =>
                {
                    var val = ParseField(n, fieldType);
                    if (val != null)
                    {
                        return val;
                    }
                    else if (IsDictionary(fieldType)) // It might be a dictionary type
                    {
                        var dict = NodesToDictionary(n.Nodes, fieldType.GenericTypeArguments[0], fieldType.GenericTypeArguments[1]);
                        return dict;
                    }
                    else if (IsList(fieldType))
                    {
                        var dict = NodesToList(n.Nodes, fieldType.GenericTypeArguments[0]);
                        return dict;
                    }
                    else
                    {
                        // It might be a type from DEngine.Data
                        if (dataTypes.Any(x => x.Name == fieldType.Name))
                        {
                            var newObj = Activator.CreateInstance(fieldType);
                            PopulateObject(newObj, n.Nodes);
                            return newObj;
                        }
                    }
                    return null;
                });

                var matchingProperty = properties.FirstOrDefault(x => x.Name == n.Name);
                if (matchingProperty != null && matchingProperty.CanWrite)
                {
                    matchingProperty.SetValue(result, populateObjectField(matchingProperty.PropertyType));
                }

                var matchingField = fields.FirstOrDefault(x => x.Name == n.Name);
                if (matchingField != null)
                {
                    matchingField.SetValue(result, populateObjectField(matchingField.FieldType));
                }

                if (matchingField == null && matchingProperty == null)
                    throw new ArgumentException("Couldn't find property or field: " + n.Name);
            }
        }

        private static float[] ParseVectorString(string vecString)
        {
            var strVal = vecString.Replace("<", string.Empty).Replace(">", string.Empty);
            return strVal.Split(new char[] { ',' }).Select(x => Convert.ToSingle(x.Trim())).ToArray();
        }

        private static string TypeToString(object val, Type type, Type baseType)
        {
            if (val == null)
                return null;

            if (type == typeof(bool))
            {
                return (bool)val ? "true" : "false";
            }
            else if (baseType == typeof(Enum))
            {
                return val.ToString();
            }
            else if (type == typeof(string))
            {
                return val.ToString().Trim();
            }
            else if (type == typeof(int))
            {
                return val.ToString();
            }
            else if (type == typeof(float))
            {
                return val.ToString();
            }
            else if (type == typeof(double))
            {
                return val.ToString();
            }
            else if (type == typeof(long))
            {
                return val.ToString();
            }
            else if (type == typeof(Vector2))
            {
                return val.ToString();
            }
            else if (type == typeof(Vector3))
            {
                return val.ToString();
            }
            else if (type == typeof(Vector4))
            {
                return val.ToString();
            }
            else if (type == typeof(Matrix4x4))
            {
                return val.ToString();
            }
            else if (type == typeof(string[]))
            {
                return string.Join(", ", (string[])val);
            }
            else if (type == typeof(int[]))
            {
                return string.Join(", ", (int[])val);
            }

            return null;
        }

        private static object ParseField(TinyYamlNode n, Type type)
        {
            var val = n.Value;
            var baseType = type.BaseType;

            if (type == typeof(string))
            {
                return n.Value.Trim();
            }
            else if (baseType == typeof(Enum))
            {
                return Enum.Parse(type, n.Value);
            }
            else if (type == typeof(bool))
            {
                return n.Value.ToLowerInvariant() == "true" ? true : false;
            }
            else if (type == typeof(int))
            {
                return Convert.ToInt32(n.Value);
            }
            else if (type == typeof(long))
            {
                return Convert.ToInt64(n.Value);
            }
            else if (type == typeof(float))
            {
                return Convert.ToSingle(n.Value);
            }
            else if (type == typeof(double))
            {
                return Convert.ToDouble(n.Value);
            }
            else if (type == typeof(Vector2))
            {
                var v = ParseVectorString(n.Value);
                return new Vector2(v[0], v[1]);
            }
            else if (type == typeof(Vector3))
            {
                var v = ParseVectorString(n.Value);
                return new Vector3(v[0], v[1], v[2]);
            }
            else if (type == typeof(Vector4))
            {
                var v = ParseVectorString(n.Value);
                return new Vector4(v[0], v[1], v[2], v[3]);
            }
            else if (type == typeof(List<string>))
            {
                return n.Nodes.Select(x => x.Name.Substring(1).Trim()).ToList();
            }
            else if (type == typeof(string[]))
            {
                return n.Value.Split(new[] { ',' }).Select(x => x.Trim()).ToArray();
            }
            else if (type == typeof(int[]))
            {
                return n.Value.Split(new[] { ',' }).Select(x => Convert.ToInt32(n.Value)).ToArray();
            }
            else if (type == typeof(float[]))
            {
                return n.Value.Split(new[] { ',' }).Select(x => Convert.ToSingle(n.Value)).ToArray();
            }

            return null;
        }

        static void IterateNodesAsYaml(TinyYamlNode n, int level, ref List<string> list)
        {
            var sb = new StringBuilder();
            sb.Append(string.Concat(Enumerable.Repeat("\t", level)));
            sb.Append(n.Name);
            sb.Append(": ");
            sb.Append(n.Value);
            sb.Append(string.IsNullOrWhiteSpace(n.Comment) ? string.Empty : " # " + n.Comment);

            list.Add(sb.ToString());
            foreach (var child in n.Nodes)
            {
                IterateNodesAsYaml(child, level + 1, ref list);
            }
        }
    }
}
