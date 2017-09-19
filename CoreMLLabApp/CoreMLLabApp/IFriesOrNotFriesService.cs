using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreMLLabApp
{
    public interface IFriesOrNotFriesService
    {
        Task<string> DetectAsync(byte[] image);
    }
}
