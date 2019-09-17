using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonXmlUtils {
	public class ContentContainer {
		public static Action<string> LogAction;

		public ContentContainer(string value) {
			Value = Encoding.GetBytes(value);
		}

		public ContentContainer(byte[] value) {
			Value = value;
		}

		private static readonly Encoding Encoding = Encoding.UTF8;
		public byte[] Value { get; private set; }

		public string ValueString {
			get => Encoding.GetString(Value);
			private set => Value = Encoding.GetBytes(value);
		}

		public void ExecuteProcessStep(ProcessingStep processStep) {
			switch (processStep) {
				case ProcessingStep.DecodeBase64:
					DecodeBase64();
					break;
				case ProcessingStep.EncodeBase64:
					EncodeBase64();
					break;
				case ProcessingStep.DecodeXml:
					DecodeXml();
					break;
				case ProcessingStep.EncodeXml:
					EncodeXml();
					break;
				case ProcessingStep.ButifyJson:
					ButifyJson();
					break;
				case ProcessingStep.HtmlDecode:
					HtmlDecode();
					break;
				case ProcessingStep.HtmlEncode:
					HtmlEncode();
					break;
				case ProcessingStep.UrlDecode:
					UrlDecode();
					break;
				case ProcessingStep.UrlEncode:
					UrlEncode();
					break;
			}
		}

		private void UrlEncode() {
			ValueString = WebUtility.UrlEncode(ValueString);
		}

		private void UrlDecode() {
			ValueString = WebUtility.UrlDecode(ValueString);
		}

		private void HtmlEncode() {
			ValueString = WebUtility.HtmlEncode(ValueString);
		}

		private void HtmlDecode() {
			ValueString = WebUtility.HtmlDecode(ValueString);
		}

		private void DecodeBase64() {
			Value = Convert.FromBase64String(ValueString);
		}

		private void EncodeBase64() {
			ValueString = Convert.ToBase64String(Value);
		}

		private void DecodeXml() {
			ValueString = ValueString.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&apos;", "'");
		}

		private void EncodeXml() {
			ValueString = ValueString.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
		}

		private void ButifyJson() {
			ValueString = JToken.Parse(ValueString).ToString(Formatting.Indented);
		}
	}
}