using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace csutil.RowComp
{
    public class RowComparer
    {
        public static (T[] old, T[] neww, Dictionary<T, T> change) Compare<T>(
            List<T> prevData,
            List<T> nextData) where T : IRow<T>
        {
            for (int i = prevData.Count - 1; i >= 0; i--)
            {
                var line = prevData[i];
                if (nextData.Remove(line))
                    prevData.Remove(line);
            }

            var diffs = new Dictionary<T, T>();
            for (int i = prevData.Count - 1; i >= 0; i--)
            {
                var line = prevData[i];
                foreach (var nl in nextData.Where(nl => line.IsSame(nl)))
                {
                    prevData.Remove(line);
                    nextData.Remove(nl);
                    diffs.Add(line, nl);
                    break;
                }
            }

            return (prevData.ToArray(), nextData.ToArray(), diffs);
        }

        public static (T[] prev, T[] next) GetData<T>(string path, string cachePath) where T : class, IRow<T>
        {
            var prevData = GetSingleData<T>(cachePath);
            var nextData = GetSingleData<T>(path);
            return (prevData, nextData);
        }

        public static T[] GetSingleData<T>(string path) where T : class, IRow<T>
        {
            return File.Exists(path)
                ? File.ReadAllLines(path)
                    .Skip(1)
                    .Select(r => FromString<T>(r.Trim()))
                    .Where(r => r != null)
                    .ToArray()
                : Array.Empty<T>();
        }

        private static T FromString<T>(string row) where T : class, IRow<T>
        {
            try
            {
                // Scuffed because interfaces cannot define static methods
                if (typeof(T) == typeof(ExampleRow))
                    return ExampleRow.FromString(row) as T;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

            throw new NotImplementedException();
        }

        private static string AddSuffix(string filename, string suffix)
        {
            var path = Path.GetDirectoryName(filename);
            var fn = Path.GetFileNameWithoutExtension(filename);
            var ext = Path.GetExtension(filename);
            return Path.Combine(path!, string.Concat(fn, suffix, ext));
        }
    }
}