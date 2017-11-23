using System;
using System.IO;

namespace JrzAsp.Lib.ProtoCms.FileSys {
    /// <summary>
    /// https://bitbucket.org/lorenzopolidori/http-form-parser/src/a48defaac3b2c8a4b89152bffb8a2eb33ad57e53/Misc.cs
    /// </summary>
    public static class ByteUtils {
        public static int IndexOf(byte[] searchWithin, byte[] serachFor, int startIndex) {
            var index = 0;
            var startPos = Array.IndexOf(searchWithin, serachFor[0], startIndex);

            if (startPos != -1) {
                while (startPos + index < searchWithin.Length) {
                    if (searchWithin[startPos + index] == serachFor[index]) {
                        index++;
                        if (index == serachFor.Length) {
                            return startPos;
                        }
                    } else {
                        startPos = Array.IndexOf(searchWithin, serachFor[0], startPos + index);
                        if (startPos == -1) {
                            return -1;
                        }
                        index = 0;
                    }
                }
            }

            return -1;
        }

        public static byte[] ToByteArray(Stream stream) {
            var buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream()) {
                while (true) {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0) {
                        return ms.ToArray();
                    }
                    ms.Write(buffer, 0, read);
                }
            }
        }
    }
}