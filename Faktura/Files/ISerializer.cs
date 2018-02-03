using System;

namespace Faktura.Files
{
    interface ISerializer
    {
        bool SerializeObject(Object objectToSerialize,string filePath);
        Object DeserializeObject(string filePath);
    }
}
