using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Corno.Services.Helper;

/// <summary>
/// Summary description for SingleApp.
/// </summary>
public static class ExtensionMethods
{
    // Deep clone
    public static T DeepClone<T>(this T a)
    {
        using (var stream = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, a);
            stream.Position = 0;
            return (T)formatter.Deserialize(stream);
        }
    }

    public static void CopyPropertiesTo<T, TU>(this T source, TU dest)
    {
        var sourceProps = typeof(T).GetProperties().Where(x => x.CanRead).ToList();
        var destProps = typeof(TU).GetProperties()
            .Where(x => x.CanWrite)
            .ToList();

        foreach (var sourceProp in sourceProps)
        {
            if (destProps.All(x => x.Name != sourceProp.Name))
                continue;

            var p = destProps.First(x => x.Name == sourceProp.Name);
            if (p.CanWrite)
                p.SetValue(dest, sourceProp.GetValue(source, null), null);
        }
    }
}