using System;
using System.Runtime.InteropServices;

namespace AMDVBIOSChecksumTool
{
    class Struct
    {
        public static T FromBytes<T>(byte[] bytes) where T : struct
        {
            T obj = default(T);
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                obj = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                handle.Free();
            }
            return obj;
        }
    }
}
