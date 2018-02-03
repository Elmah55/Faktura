using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;

namespace Faktura.Files
{
    class ObjectSerializer : ISerializer
    {
        private IFormatter BinaryFormatter;
        private Stream FileStream;
        //Indicates whether object serializer has access permision to write and read
        private bool HasAccessPermisions;

        public ObjectSerializer()
        {
            this.BinaryFormatter = new BinaryFormatter();
        }

        ~ObjectSerializer()
        {
            CloseFileStream();
        }

        public bool SerializeObject(Object objectToSerialize, string filePath)
        {
            bool result = false;

            if (null != objectToSerialize && null != BinaryFormatter
                && null != filePath && OpenFileStream(filePath, FileMode.Create))
            {
                try
                {
                    BinaryFormatter.Serialize(FileStream, objectToSerialize);
                    result = true;
                }
                catch (SerializationException)
                {
                    result = false;
                }
            }

            CloseFileStream();

            return result;
        }

        /// <summary>
        /// Deserializes object of given file path
        /// </summary>
        /// <returns>Instance of deserialized object or null if file was not found</returns>
        public Object DeserializeObject(string filePath)
        {
            Object deserializedObject = null;

            if (null != BinaryFormatter && null != filePath && OpenFileStream(filePath, FileMode.Open))
            {
                try
                {
                    deserializedObject = BinaryFormatter.Deserialize(FileStream);
                }
                catch (SerializationException)
                {
                    deserializedObject = null;
                }
            }

            CloseFileStream();

            return deserializedObject;
        }

        private bool OpenFileStream(string filePath, FileMode mode)
        {
            bool result = false;
            FileStream = null; //Reset variable to null in case it is still poiting to old stream

            if (null != filePath)
            {
                try
                {
                    if (FileMode.Create == mode && !Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }

                    FileStream = new FileStream(filePath, mode);

                    if (null != FileStream)
                    {
                        result = true;
                    }
                }
                catch (Exception)
                {
                    result = false;
                }
            }

            return result;
        }

        private void CloseFileStream()
        {
            if (null != FileStream)
            {
                FileStream.Close();
                FileStream.Dispose();
            }
        }
    }
}
