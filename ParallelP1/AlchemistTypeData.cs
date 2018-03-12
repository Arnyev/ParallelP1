using System.Linq;
using System.Threading;

namespace Parallel1
{
    public class AlchemistTypeData
    {
        public Semaphore ResourcesWaitingSemaphore = new Semaphore(0, 1);
        public GlobalState GlobalState;
        public int WaitingCount;
        public Resource[] ResourcesRequired;
        public AlchemistType Type;

        public AlchemistTypeData(AlchemistType type, GlobalState globalState, params Resource[] resourcesRequired)
        {
            ResourcesRequired = resourcesRequired;
            Type = type;
            GlobalState = globalState;
        }

        public void SingleTypeCheckAndWake(GlobalState globalState)
        {
            if (WaitingCount > 0 && ResourcesRequired.All(resource => globalState.ResourcesReady[(int)resource] > 0))
            {
                foreach (int requiredResource in ResourcesRequired)
                {
                    globalState.ResourcesReady[requiredResource]--;
                    globalState.MagazineSemaphores[requiredResource].Release();
                }

                WaitingCount--;
                ResourcesWaitingSemaphore.Release();
            }
        }
    }
}
