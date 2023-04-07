using System;

namespace MmImageLoading.Work
{
	public enum ImageSource
	{
		Url = 3,

		Filepath = 10,
		ApplicationBundle = 11,
		CompiledResource = 12,
        EmbeddedResource = 13,

		Stream = 20,
	}
}

