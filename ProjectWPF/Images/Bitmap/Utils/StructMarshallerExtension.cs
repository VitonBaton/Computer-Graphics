using System.IO;
using System.Runtime.InteropServices;

namespace ProjectWPF.Images.Bitmap.Utils
{
    public static class StructMarshallerExtension
    {
        public static T ReadStruct<T> (this FileStream fs)
            where T: struct
        {
            var buffer = new byte[Marshal.SizeOf(typeof(T))];
            fs.Read(buffer, 0,	Marshal.SizeOf(typeof(T)));
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var temp = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return temp;
        }

        public static void WriteStruct<T>(this FileStream fs, T obj)
            where T : struct
        {
            int rawsize = Marshal.SizeOf(obj);
            byte[] rawdata = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdata, GCHandleType.Pinned);
            Marshal.StructureToPtr(obj, handle.AddrOfPinnedObject(), false);
            handle.Free();

            fs.Write(rawdata, 0, rawsize);
        }
    }
}