using System.Text.Json.Serialization;

namespace ZoomNet.Models
{
	/// <summary>
	/// Premium event.
	/// </summary>
	public class PremiumEvent
	{
		/// <summary>Gets or sets the premium event id.</summary>
		[JsonPropertyName("event")]
		public string Id { get; set; }

		/// <summary>Gets or sets the name of the premium event.</summary>
		[JsonPropertyName("event_name")]
		public string Name { get; set; }
	}
}
