using MessageRouter.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageRouter.Services.Contracts
{
    public interface IImageProcessingService
    {
        Task<bool> ProcessingImageAsync(ImageMessage imageMessage);
        //ImageMessage GetResult();
    }
}
