using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UndertaleModTool.ProjectTool
{
    public class YYJson : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ICollection).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is IEnumerable<object> list)
            {
                writer.WriteStartArray();

                foreach (var item in list)
                {
                    var json = JsonConvert.SerializeObject(item, Formatting.None);
                    writer.WriteRawValue(json);
                }

                writer.WriteEndArray();
            }
        }
    }

    public class YYFlatJson : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject jo = new JObject();
            Type type = value.GetType();

            var sortedProperties = type.GetProperties();
            foreach (PropertyInfo prop in sortedProperties)
            {
                if (!prop.CanRead || prop.GetIndexParameters().Length > 0)
                    continue;

                object propVal = prop.GetValue(value, null);

                //Dump.GetMainWindow().ScriptMessage($"{prop} - {propVal}");

                bool allow_null = true;
                bool allow_default = true;

                var jsonPropertyAttribute = prop.GetCustomAttribute<JsonPropertyAttribute>();
                if (jsonPropertyAttribute != null)
                {
                    if (jsonPropertyAttribute.NullValueHandling == NullValueHandling.Ignore)
                        allow_null = false;
                    if (jsonPropertyAttribute.DefaultValueHandling == DefaultValueHandling.Ignore)
                        allow_default = false;
                }

                if (propVal == null)
                {
                    if (allow_null)
                        jo.Add(prop.Name, null);
                    continue;
                }
                if (!allow_default)
                {
                    // This is fucking terrible.
                    if (propVal is IList && ((IList)propVal).Count == 0)
                        continue;
                }

                jo.Add(prop.Name, JToken.FromObject(propVal, serializer));
            }

            jo.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsClass && objectType != typeof(string) && !typeof(ICollection).IsAssignableFrom(objectType);
        }
    }
}
