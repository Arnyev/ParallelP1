using System.Linq;
using System.Threading;

namespace Parallel1
{
    public class AlchemistTypeData
    {
        public Semaphore ResourcesWaitingSemaphore = new Semaphore(0, int.MaxValue);
        public Semaphore WaitingCountSemaphore = new Semaphore(1, 1);
        public Semaphore ChangeSemaphore;
        public int WaitingCount;
        public Resource[] ResourcesRequired;
        public AlchemistType Type;

        public AlchemistTypeData(AlchemistType type, Semaphore changeSemaphore, params Resource[] resourcesRequired)
        {
            ResourcesRequired = resourcesRequired;
            Type = type;
            ChangeSemaphore = changeSemaphore;
        }

        public void CheckAndWake(int[] ResourcesReady, Semaphore[] magazineSemaphores)
        {
            if (WaitingCount > 0 && ResourcesRequired.All(resource => ResourcesReady[(int)resource] > 0))
            {
                foreach (int requiredResource in ResourcesRequired)
                {
                    ResourcesReady[requiredResource]--;
                    magazineSemaphores[requiredResource].Release();
                }

                WaitingCount--;
                ResourcesWaitingSemaphore.Release();
            }
        }
    }
}
