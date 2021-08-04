using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace API.Code {

    // Remove navigation properties from JSON
	public class JSONNoNavigation {

        public static object NoNavigation(object value) {
            string objStr = Serialize(value);
            return JsonConvert.DeserializeObject(objStr);
		}

        public static string Serialize(object value) {
            // Serializer settings
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CustomResolver();
            settings.PreserveReferencesHandling = PreserveReferencesHandling.None;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            // Do the serialization and output to the console
            string json = JsonConvert.SerializeObject(value, settings);
            return json;
        }

        class CustomResolver : DefaultContractResolver {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
                JsonProperty prop = base.CreateProperty(member, memberSerialization);

                if (prop.PropertyType.IsClass &&
                    prop.PropertyType != typeof(string)) {
                    prop.ShouldSerialize = obj => false;
                }

                return prop;
            }
        }

    }

}
