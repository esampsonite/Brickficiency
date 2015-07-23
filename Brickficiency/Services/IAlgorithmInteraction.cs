using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Brickficiency.Services
{
    public interface IAlgorithmInteraction
    {
        void OnMatchFound(ICollection<string> storeNames);

        void OnProgress(long solutionsChecked);
    }
}
