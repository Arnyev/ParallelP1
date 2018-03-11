using System.Threading;

namespace Parallel1
{
    public class GlobalState
    {
        public AlchemistTypeData[] AlchemistTypes;

        public int[] ResourcesReady = new int[3];
        public Semaphore ChangeSemaphore = new Semaphore(1, 1);

        public Semaphore[] MagazineSemaphores;

        public GlobalState()
        {
            MagazineSemaphores = new Semaphore[Constants.ResourcesCount];
            MagazineSemaphores[(int)Resource.Lead] = new Semaphore(Constants.MaxSpace, Constants.MaxSpace);
            MagazineSemaphores[(int)Resource.Mercury] = new Semaphore(Constants.MaxSpace, Constants.MaxSpace);
            MagazineSemaphores[(int)Resource.Sulfur] = new Semaphore(Constants.MaxSpace, Constants.MaxSpace);

            AlchemistTypes = new AlchemistTypeData[Constants.AlchemistTypesCount];
            AlchemistTypes[0] = new AlchemistTypeData(AlchemistType.D, ChangeSemaphore, Resource.Lead, Resource.Mercury, Resource.Sulfur);
            AlchemistTypes[1] = new AlchemistTypeData(AlchemistType.A, ChangeSemaphore, Resource.Lead, Resource.Mercury);
            AlchemistTypes[2] = new AlchemistTypeData(AlchemistType.B, ChangeSemaphore, Resource.Sulfur, Resource.Mercury);
            AlchemistTypes[3] = new AlchemistTypeData(AlchemistType.C, ChangeSemaphore, Resource.Lead, Resource.Sulfur);
        }

        public void CheckAndWake()
        {
            foreach (var type in AlchemistTypes)
                type.CheckAndWake(ResourcesReady, MagazineSemaphores);
        }
    }
}
