using System.Threading;

namespace Parallel1
{
    public class FactoryData
    {
        public Resource Resource;
        public Semaphore CursesSemaphore = new Semaphore(1, 1);
        public int CursesCount;
        public Semaphore NoCurseSemaphore = new Semaphore(1, 1);
        public GlobalState GlobalState;

        public FactoryData(Resource resource, GlobalState globalState)
        {
            Resource = resource;
            GlobalState = globalState;
        }
    }
}
