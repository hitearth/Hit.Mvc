using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace Hit.Mvc
{
    /// <summary>
    /// Converts a <see cref="DateTime"/> to the ISO 8601 date format (e.g. 2008-04-12T12:53Z).
    /// </summary>
    public class WriteDateTimeConverter : DateTimeConverterBase
    {
        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
        private string _dateTimeFormat;
        private CultureInfo _culture;


        /// <summary>
        /// Gets or sets the date time styles used when converting a date to JSON.
        /// </summary>
        /// <value>The date time styles used when converting a date to JSON.</value>
        public DateTimeStyles DateTimeStyles
        {
            get { return _dateTimeStyles; }
            set { _dateTimeStyles = value; }
        }

        /// <summary>
        /// Gets or sets the date time format used when converting a date to JSON.
        /// </summary>
        /// <value>The date time format used when converting a date to JSON.</value>
        public string DateTimeFormat
        {
            get { return _dateTimeFormat ?? string.Empty; }
            set { _dateTimeFormat = (string.IsNullOrEmpty(value)) ? null : value; }
        }

        /// <summary>
        /// Gets or sets the culture used when converting a date to JSON.
        /// </summary>
        /// <value>The culture used when converting a date to JSON.</value>
        public CultureInfo Culture
        {
            get { return _culture ?? CultureInfo.CurrentCulture; }
            set { _culture = value; }
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string text;

            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;

                if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                    || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                {
                    dateTime = dateTime.ToUniversalTime();
                }

                text = dateTime.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);
            }
#if !NET20
            else if (value is DateTimeOffset)
            {
                DateTimeOffset dateTimeOffset = (DateTimeOffset)value;
                if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                    || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                {
                    dateTimeOffset = dateTimeOffset.ToUniversalTime();
                }

                text = dateTimeOffset.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);
            }
#endif
            else
            {
                throw new JsonSerializationException("Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {0}.");
            }

            writer.WriteValue(text);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException("DateTimeConverter should only be used while deserializing.");
        }
        /// <summary>
        /// Gets a value indicating whether this Newtonsoft.Json.JsonConverter can read JSON.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return false;
            }
        }
    }
}