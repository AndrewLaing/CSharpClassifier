using System.Runtime.Serialization.Json;  // Add to Project from nuget if it doesn't exist
using System.Text;

namespace BayesianClassifier
{
    public static class JsonSerialise
    {
        /// <summary>
        /// Serialises an object as a json file
        /// </summary>
        /// <param name="filename">Path to file.</param>
        /// <param name="obj">Object to serialise to file.</param>
        /// 
        /// <exception cref="System.ArgumentException">Path passed to WriteStringToFile() is an empty string (""). -or- path contains the name of a system device =or= The byte array passed to GetString() in Serialise() contains invalid Unicode code points.</exception>
        /// <exception cref="System.ArgumentNullException">Path passed to WriteStringToFile() is null =or= Bytes passed to GetString() in Serialise() is null.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path passed to WriteStringToFile() is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="System.IO.IOException">Path passed to WriteStringToFile() includes an incorrect or invalid syntax for file name, directory name, or volume label syntax.</exception>
        /// <exception cref="System.IO.PathTooLongException">The specified path, file name, or both passed to WriteStringToFile() exceed the system-defined maximum length.</exception>
        /// <exception cref="System.Runtime.Serialization.InvalidDataContractException">The type passed to Serialise() does not conform to data contract rules.</exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">There is a problem with the instance being written in Serialise().</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission in WriteStringToFile().</exception>
        /// <exception cref="System.ServiceModel.QuotaExceededException"> The maximum number of objects to serialize has been exceeded. Check the System.Runtime.Serialization.DataContractSerializer.MaxItemsInObjectGraph property.</exception>
        /// <exception cref="System.Text.DecoderFallbackException">A fallback occurred to GetString() in Serialise() (for more information, see Character Encoding in .NET) -and- System.Text.Encoding.DecoderFallback is set to System.Text.DecoderExceptionFallback.</exception>
        /// <exception cref="System.Text.EncoderFallbackException">The current encoding in WriteStringToFile() does not support displaying half of a Unicode surrogate pair.</exception>
        /// <exception cref="System.UnauthorizedAccessException">Access is denied in WriteStringToFile().</exception>
        public static void SaveObject<T>(string filename, T obj)
        {
            string json = "";
            Serialise<T>(obj, ref json);
            WriteStringToFile(filename, json);
        }

        /// <summary>
        /// Deserialises object stored in a json file to a referenced object
        /// </summary>
        /// <param name="filename">Path to file.</param>
        /// <param name="obj">Object to contain deserialised file contents.</param>
        /// 
        /// <exception cref="System.ArgumentException">Stream in Deserialise() is not writable =or= Path passed to ReadStringFromFile() is an empty string.</exception>
        /// <exception cref="System.ArgumentNullException">Stream in Deserialise() is null =or= Path passed to ReadStringFromFile() is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The position in Deserialise() is set to a negative value or a value greater than System.Int32.MaxValue.</exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">The specified path passed to ReadStringFromFile() is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="System.IO.FileNotFoundException">The file opened in ReadStringFromFile() cannot be found.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occured in Deserialise() =or= An I/O error occured in ReadStringFromFile() =or Path passed to ReadStringFromFile() includes an incorrect or invalid syntax for file name, directory name, or volume label.</exception>
        /// <exception cref="System.NotSupportedException">System.IO.StreamWriter.AutoFlush in Deserialise() is true or the System.IO.StreamWriter buffer is full, and the contents of the buffer cannot be written to the underlying fixed size stream because the System.IO.StreamWriter is at the end the stream.</exception>
        /// <exception cref="System.ObjectDisposedException">The stream in Deserialise() is closed =or= System.IO.StreamWriter.AutoFlush in Deserialise() is true or the System.IO.StreamWriter buffer is full, and current writer is closed.</exception>
        /// <exception cref="System.OutOfMemoryException">There is insufficient memory to allocate a buffer for the returned string in ReadStringFromFile() .</exception>
        /// <exception cref="System.Text.EncoderFallbackException">.The current encoding in Deserialise() does not support displaying half of a Unicode surrogate pair</exception>
        public static bool LoadObject<T>(string filename, ref T obj)
        {
            string? json = "";
            if (File.Exists(filename) && ReadStringFromFile(filename, ref json))
            {
                Deserialise<T>(json, ref obj);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Write a string to a file
        /// </summary>
        /// <param name="filename">Path to file.</param>
        /// <param name="s">String to write to file.</param>
        private static void WriteStringToFile(string filename, string s)
        {
            StreamWriter sw = new(filename);
            sw.WriteLine(s);
            sw.Close();
        }

        /// <summary>
        /// Read a string from a file
        /// </summary>
        /// <param name="filename">Path to file.</param>
        /// <param name="s">String to read file contents into.</param>
        private static bool ReadStringFromFile(string filename, ref string? s)
        {
            bool stringRead = false;

            StreamReader sr = new(filename);
            s = sr.ReadLine();
            if (s?.Length > 0)
            {
                stringRead = true;
            }
            sr.Close();

            return stringRead;
        }

        /// <summary>
        /// Deserialise an object from a json string
        /// </summary>
        /// <param name="json">String containing JSON to deserialise into object.</param>
        /// <param name="obj">Object to deserialise string into.</param>
        private static void Deserialise<T>(string? json, ref T obj)
        {
            MemoryStream ms = new();
            StreamWriter sw = new(ms);

            sw.Write(json);
            sw.Flush();
            ms.Position = 0;

            T? t = (T?)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            if(t != null)
            {
                obj = t;
            }

            sw.Close();
            ms.Close();
        }

        /// <summary>
        /// Serialise an object to a referenced json string
        /// </summary>
        /// <param name="obj">Object to serialise.</param>
        /// <param name="s">Reference to string which will contain the serialised object.</param>
        private static void Serialise<T>(T obj, ref string s)
        {
            MemoryStream ms = new();
            new DataContractJsonSerializer(typeof(T)).WriteObject(ms, obj);
            s = Encoding.Default.GetString(ms.ToArray());
        }
    }
}
