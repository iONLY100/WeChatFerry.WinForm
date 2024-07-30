using System.Text;

namespace WeChatFerry.WinForm.WeChatFerry;

public class DatabaseTypeConverter
{
    private static readonly Dictionary<int, Func<byte[], object?>> SqlTypes = new(){
        { 1, bytes => BitConverter.ToInt32(bytes) },
        { 2, bytes => BitConverter.ToSingle(bytes) },
        { 3, bytes => Encoding.UTF8.GetString(bytes) },
        { 4, bytes => bytes },
        { 5, _ => null }
    };

    public static object? ConvertType(int type, byte[] data)
    {
        if (SqlTypes.TryGetValue(type, out var converter))
        {
            return converter(data);
        }
        throw new ArgumentException("Unsupported type");
    }
}