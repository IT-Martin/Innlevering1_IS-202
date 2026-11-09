using System.Buffers.Binary;

namespace Reachout.Services;
public static class PhotoUpload
{
    public static async Task<(byte[] Data, string Type)> ReadAsync(IFormFile photo)
    {
        if (photo.Length == 0 || photo.Length > 2 * 1024 * 1024)
        {
            throw new ArgumentException("Velg et PNG- eller JPEG-bilde på maks 2 MB.");
        }

        using var buffer = new MemoryStream();
        await photo.CopyToAsync(buffer);
        var data = buffer.ToArray();
        if (data.Length >= 45 && data.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }) && data.AsSpan(12, 4).SequenceEqual("IHDR"u8) && data.AsSpan(data.Length - 8, 4).SequenceEqual("IEND"u8))
        {
            CheckSize(BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(16, 4)), BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(20, 4)));
            return (data, "image/png");
        }

        if (data.Length > 4 && data[0] == 255 && data[1] == 216 && data[^2] == 255 && data[^1] == 217)
        {
            var offset = 2;
            while (offset + 4 < data.Length && data[offset] == 255)
            {
                while (offset < data.Length && data[offset] == 255)
                {
                    offset++;
                }

                if (offset + 3 >= data.Length)
                {
                    break;
                }

                var marker = data[offset++];
                var length = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset, 2));
                if (length < 2 || offset + length > data.Length)
                {
                    break;
                }

                if (marker is 192 or 193 or 194 && length >= 8)
                {
                    CheckSize(BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset + 5, 2)), BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset + 3, 2)));
                    return (data, "image/jpeg");
                }

                offset += length;
            }
        }

        throw new ArgumentException("Filen kunne ikke leses som PNG eller JPEG.");
    }

    private static void CheckSize(uint width, uint height)
    {
        if (width == 0 || height == 0 || width > 6000 || height > 6000 || (ulong)width * height > 20_000_000)
        {
            throw new ArgumentException("Bildet er for stort. Maks 6000 × 6000 piksler og 20 megapiksler.");
        }
    }
}
