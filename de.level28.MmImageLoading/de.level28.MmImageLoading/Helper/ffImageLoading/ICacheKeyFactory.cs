using System;

namespace MmImageLoading.MAUI
{
	public interface ICacheKeyFactory
	{
		string GetKey(ImageSource imageSource, object bindingContext);
	}
}

