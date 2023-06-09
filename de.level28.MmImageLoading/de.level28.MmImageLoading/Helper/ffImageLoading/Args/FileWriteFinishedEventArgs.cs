﻿using System;

namespace MmImageLoading.Args
{
    
    public class FileWriteFinishedEventArgs : EventArgs
    {
        public FileWriteFinishedEventArgs(FileWriteInfo fileWriteInfo)
        {
            FileWriteInfo = fileWriteInfo;
        }

        public FileWriteInfo FileWriteInfo { get; private set; }
    }
}
