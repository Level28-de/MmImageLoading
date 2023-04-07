using System;

namespace MmImageLoading.Mock
{
    public class MockBitmap
    {
        public MockBitmap()
        {
        }

        public Guid Id { get; } = Guid.NewGuid();
    }
}
