using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMat.Core.Services
{
    public interface IShareable
    {
        void OpenShareIntent(string textToShare);
    }
}
