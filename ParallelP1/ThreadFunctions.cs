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

            var rand = new Random(seed);
            while (true)
            {
                data.GlobalState.MagazineSemaphores[resource].WaitOne();

                data.NoCurseSemaphore.WaitOne();
                data.NoCurseSemaphore.Release();

                Thread.Sleep(rand.Next(minTime, maxTime));

                data.GlobalState.ChangeSemaphore.WaitOne();
                data.GlobalState.ResourcesReady[resource]++;
                Console.WriteLine("Produced " + data.Resource);
                data.GlobalState.AllTypesCheckAndWake();
                data.GlobalState.ChangeSemaphore.Release();
            }
        }

        public static void Alchemist(GlobalState globalState, int alchemistType)
        {
            globalState.ChangeSemaphore.WaitOne();
            var myAlchType = globalState.AlchemistTypes[alchemistType];
            myAlchType.WaitingCount++;
            myAlchType.SingleTypeCheckAndWake(globalState);
            globalState.ChangeSemaphore.Release();

            Console.WriteLine("Alchemist of type " + myAlchType.Type + " started waiting.");
            globalState.AlchemistTypes[alchemistType].ResourcesWaitingSemaphore.WaitOne();
            Console.WriteLine("Alchemist of type " + myAlchType.Type + " finished.");
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

        public static void AlchemistsSpawner(GlobalState globalState,
            int alchemistDelayMin, int alchemistDelayMax, int seed, Semaphore threadSemaphore, IList<Thread> threads)
        {
            var rand = new Random(seed);

            while (true)
            {
                Thread.Sleep(rand.Next(alchemistDelayMin, alchemistDelayMax));
                var alchType = rand.Next(Constants.AlchemistTypesCount);
                var alchemistThread = new Thread(() => Alchemist(globalState, alchType));
                threadSemaphore.WaitOne();
                threads.Add(alchemistThread);
                alchemistThread.Start();
                threadSemaphore.Release();
            }
        }
    }
}
