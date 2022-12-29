using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ZoomNet.Json
{
	/// <summary>
	/// Converts an array of App permissions to or from JSON.
	/// </summary>
	/// <seealso cref="ZoomNetJsonConverter{T}"/>
	internal class AppPermissionsConverter : ZoomNetJsonConverter<string[]>
	{
		public override string[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			if (reader.TokenType == JsonTokenType.StartArray)
			{
				var values = new List<string>();

				while ((reader.TokenType != JsonTokenType.EndArray) && reader.Read())
				{
					if (reader.TokenType == JsonTokenType.StartObject)
					{
						while ((reader.TokenType != JsonTokenType.EndObject) && reader.Read())
						{
							if (reader.TokenType == JsonTokenType.PropertyName)
							{
								var propertyName = reader.GetString();
								if (propertyName == "name" && reader.Read())
								{
									values.Add(reader.GetString());
									reader.Read();
								}
							}
						}
					}
				}

				return values.ToArray();
			}

			return Array.Empty<string>();
		}

		public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();

			foreach (var item in value)
			{
				writer.WriteStartObject();
				writer.WriteString("name", item);
				writer.WriteEndObject();
			}

			writer.WriteEndArray();
		}
	}
}
