using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brickficiency.Services
{
    public interface IMatchAlgorithm
    {
        void PreProcess();

        void Run(int numberOfStores);

        void Cancel();

        bool IsCancellationPending { get; }
    }
}
