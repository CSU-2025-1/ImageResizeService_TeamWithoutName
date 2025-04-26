using MessageRouter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Services.Contracts
{
    public interface ICacheService
    {
        bool SaveImage(ImageCache imageCache);

    }
}
