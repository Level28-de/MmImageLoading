using System;
using System.IO;

namespace MmImageLoading.Helpers
{
    [Preserve(AllMembers = true)]
    public interface IMD5Helper
    {
        string MD5(string input);

        string MD5(Stream stream);
    }
}
