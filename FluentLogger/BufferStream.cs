using System;
using System.IO;
using System.Text;
using System.Threading;

namespace FluentLogger
{
    public class BufferWriter : IDisposable
    {
        private StreamWriter sw;
        private MemoryStream ms = new MemoryStream();
        private readonly string filePath;
        private readonly int modWrite;
        private int countDown;
        private readonly Mutex writerMutex = new Mutex();

        public BufferWriter(string filePath, int modWrite=5)
        {
            sw = new StreamWriter(ms, Encoding.UTF8);
            this.filePath = filePath;
            this.modWrite = modWrite;
            this.countDown = modWrite;
        }


        public void Dispose()
        {
            Flush();
            sw.Dispose();
            ms.Dispose();
        }
        public void Append(string s)
        {
            try
            {
                sw.Write(s);
            } catch (OutOfMemoryException ex)
            {
                Flush();
                sw.Write("Out of memory exception", ex.Message);
            }

            
            if (--countDown <= 0)
            {
                Flush();
                countDown = modWrite;
            }
        }
        public void Flush()
        {
            try
            {
                writerMutex.WaitOne(700);
                using (var fs = File.Open(this.filePath, FileMode.Append, FileAccess.Write))
                {
                    sw.Flush();
                    ms.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.CopyTo(fs);
                    ms.Dispose();
                    ms = new MemoryStream();
                    sw.Dispose();
                    sw = new StreamWriter(ms);
                }
            }
            finally
            {
                writerMutex.ReleaseMutex();
            }
            
        }
    }
}
