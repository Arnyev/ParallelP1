using System;
using System.Collections.Generic;
using System.Threading;

namespace Parallel1
{
    public static class ThreadFunctions
    {
        public static void Factory(FactoryData data, int minTime, int maxTime, int seed)
        {
            var resource = (int)data.Resource;
            var magazineSemaphore = data.GlobalState.MagazineSemaphores[resource];
            var noCurseSemaphore = data.NoCurseSemaphore;
            var changeSemaphore = data.GlobalState.ChangeSemaphore;

            var rand = new Random(seed);
            while (true)
            {
                magazineSemaphore.WaitOne();

                noCurseSemaphore.WaitOne();
                noCurseSemaphore.Release();

                Thread.Sleep(rand.Next(minTime, maxTime));

                changeSemaphore.WaitOne();
                data.GlobalState.ResourcesReady[resource]++;
                Console.WriteLine("Produced " + data.Resource);
                data.GlobalState.CheckAndWake();
                changeSemaphore.Release();
            }
        }

        public static void Alchemist(AlchemistTypeData data, int[] resourcesReady, Semaphore[] magazineSemaphores)
        {
            data.WaitingCountSemaphore.WaitOne();
            data.WaitingCount++;
            data.WaitingCountSemaphore.Release();
            Console.WriteLine("Alchemist of type " + data.Type + " started waiting.");

            data.ChangeSemaphore.WaitOne();
            data.CheckAndWake(resourcesReady, magazineSemaphores);
            data.ChangeSemaphore.Release();

            data.ResourcesWaitingSemaphore.WaitOne();
            Console.WriteLine("Alchemist of type " + data.Type + " finished.");
        }

        public static void Sorcerer(FactoryData[] data, int minTime, int maxTime, int seed)
        {
            var rand = new Random(seed);
            while (true)
            {
                Thread.Sleep(rand.Next(minTime, maxTime));

                foreach (var factory in data)
                {
                    factory.CursesSemaphore.WaitOne();
                    if (factory.CursesCount > 0)
                    {
                        factory.CursesCount--;
                        Console.WriteLine("Sorcerer releases factory " + factory.Resource + " now has " + factory.CursesCount + " curses.");

                        if (factory.CursesCount == 0)
                            factory.NoCurseSemaphore.Release();
                    }

                    factory.CursesSemaphore.Release();
                }
            }
        }

        public static void Warlock(FactoryData[] data, int minTime, int maxTime, int seed)
        {
            var rand = new Random(seed);
            while (true)
            {
                var sleepTime = rand.Next(minTime, maxTime);
                Thread.Sleep(sleepTime);

                var factoryNr = rand.Next(Constants.ResourcesCount);
                var factory = data[factoryNr];

                factory.CursesSemaphore.WaitOne();
                factory.CursesCount++;

                Console.WriteLine("Factory " + factory.Resource + " cursed, has " + factory.CursesCount + " curses.");
                if (factory.CursesCount == 1)
                    factory.NoCurseSemaphore.WaitOne();

                factory.CursesSemaphore.Release();
            }
        }

        public static void AlchemistsSpawner(AlchemistTypeData[] data, int[] resourcesReady, Semaphore[] magazineSemaphores,
            int alchemistDelayMin, int alchemistDelayMax, int seed, Semaphore threadSemaphore, IList<Thread> threads)
        {
            var rand = new Random(seed);

            while (true)
            {
                Thread.Sleep(rand.Next(alchemistDelayMin, alchemistDelayMax));
                var alchType = rand.Next(Constants.AlchemistTypesCount);
                var alchemistThread = new Thread(() => Alchemist(data[alchType], resourcesReady, magazineSemaphores));
                threadSemaphore.WaitOne();
                alchemistThread.Start();
                threads.Add(alchemistThread);
                threadSemaphore.Release();
            }
        }
    }
}
