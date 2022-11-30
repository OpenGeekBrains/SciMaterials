using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SciMaterials.DAL.Resources.Contracts.Entities;
using File = SciMaterials.DAL.Resources.Contracts.Entities.File;

namespace SciMaterials.DAL.Resources.TestData;

internal static class AssemblyResources
{
    private class DateTimeFormatter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            var str = Encoding.UTF8.GetString(reader.ValueSpan);

            if (DateTime.TryParseExact(str, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var value))
                return value;

            //if (Utf8Parser.TryParse(reader.ValueSpan, out DateTime value, out _, 'R'))
            //    return value;

            throw new FormatException();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            Span<byte> buffer = stackalloc byte[29];

            var result = Utf8Formatter.TryFormat(value, buffer, out _, new StandardFormat('R'));
            Debug.Assert(result);

            writer.WriteStringValue(buffer);
        }
    }

    private class BooleanFormatter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            var str = Encoding.UTF8.GetString(reader.ValueSpan);

            if (bool.TryParse(str, out var value))
                return value;

            //if (Utf8Parser.TryParse(reader.ValueSpan, out DateTime value, out _, 'R'))
            //    return value;

            throw new FormatException();
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            Span<byte> buffer = stackalloc byte[29];

            var result = Utf8Formatter.TryFormat(value, buffer, out _, new StandardFormat('R'));
            Debug.Assert(result);

            writer.WriteStringValue(buffer);
        }
    }

    private class GuidFormatter : JsonConverter<Guid?>
    {
        public override Guid? Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            if (reader.ValueSpan.Length == 0)
                return null!;

            var str = Encoding.UTF8.GetString(reader.ValueSpan);

            if (Guid.TryParse(str, out var value))
                return value;

            //if (Utf8Parser.TryParse(reader.ValueSpan, out DateTime value, out _, 'R'))
            //    return value;

            throw new FormatException();
        }

        public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
        {
            //Span<byte> buffer = stackalloc byte[29];

            //var result = Utf8Formatter.TryFormat(value, buffer, out _, new StandardFormat('R'));
            //Debug.Assert(result);

            //writer.WriteStringValue(buffer);
            throw new NotSupportedException();
        }
    }

    private class ByteArrayFormatter : JsonConverter<byte[]>
    {
        public override byte[] Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            //var str = Encoding.UTF8.GetString(reader.ValueSpan);

            if (!reader.Read())
                throw new FormatException();


            var values = new List<byte>();

            while (reader.TokenType == JsonTokenType.Number)
            {
                var str = Encoding.UTF8.GetString(reader.ValueSpan);
                values.Add(byte.Parse(str));

                reader.Read();
            }

            return values.ToArray();

            //var data = JsonSerializer.Deserialize<byte[]>(ref reader);


            //if (Utf8Parser.TryParse(reader.ValueSpan, out DateTime value, out _, 'R'))
            //    return value;

            //throw new FormatException();
        }

        public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
        {
            //Span<byte> buffer = stackalloc byte[29];

            //var result = Utf8Formatter.TryFormat(value, buffer, out _, new StandardFormat('R'));
            //Debug.Assert(result);

            //writer.WriteStringValue(buffer);
            throw new NotSupportedException();
        }
    }

    private static IEnumerable<T> GetItems<T>([CallerMemberName] string ResourceName = null!)
    {
        using var stream = typeof(AssemblyResources).Assembly
               .GetManifestResourceStream(typeof(AssemblyResources), $"{ResourceName}.json")
            ?? throw new InvalidOperationException($"Не удалось получить поток данных ресурса {ResourceName}");

        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new DateTimeFormatter(),
                new BooleanFormatter(),
                new GuidFormatter(),
                new ByteArrayFormatter(),
            }
        };

        var items = JsonSerializer.Deserialize<IEnumerable<T>>(stream, options);

        return items ?? throw new InvalidOperationException($"Отсутствуют данные для запрошенного ресурса {ResourceName}");
    }

    public static IEnumerable<User> Users => GetItems<User>();

    public static IEnumerable<Author> Authors => GetItems<Author>();

    public static IEnumerable<ContentType> ContentTypes => GetItems<ContentType>();

    public static IEnumerable<FileGroup> FileGroups => GetItems<FileGroup>();

    public static IEnumerable<File> Files => GetItems<File>();

    public static IEnumerable<Url> Urls => GetItems<Url>();

    public static IEnumerable<Link> Links => GetItems<Link>();

    public static IEnumerable<Tag> Tags => GetItems<Tag>();

    public static IEnumerable<Category> Categories => GetItems<Category>();

    public static IEnumerable<Comment> Comments => GetItems<Comment>();

    public static IEnumerable<Rating> Ratings => GetItems<Rating>();
}
