using System;
using System.Collections.Generic;
using System.Threading;

namespace Parallel1
{
    class Program
    {
        static void Main(string[] args)
        {
            var globalState = new GlobalState();

            var factories = StartFactories(globalState, out FactoryData[] factoryDatas);
            var sorcerers = StartSorcerers(factoryDatas);
            var warlocks = StartWarlocks(factoryDatas);

            var threads = new List<Thread>();
            var threadSemaphore = new Semaphore(1, 1);

            var alchemistsSpawner = new Thread(() => ThreadFunctions.AlchemistsSpawner(globalState.AlchemistTypes, globalState.ResourcesReady,
                globalState.MagazineSemaphores, Constants.AlchemistDelayMin, Constants.AlchemistDelayMax, Constants.AlchemistSpawnerSeed, threadSemaphore, threads));

            threads.AddRange(factories);
            threads.AddRange(sorcerers);
            threads.AddRange(warlocks);
            threads.Add(alchemistsSpawner);

            alchemistsSpawner.Start();

            Console.Read();
            threadSemaphore.WaitOne();
            foreach (var thread in threads)
            {
                thread.Abort();
                thread.Join();
            }
        }

        private static Thread[] StartWarlocks(FactoryData[] factoryDatas)
        {
            var warlocks = new Thread[Constants.WarlocksCount];
            for (int i = 0; i < Constants.WarlocksCount; i++)
            {
                var localI = i;
                warlocks[i] = new Thread(() => ThreadFunctions.Warlock(factoryDatas, Constants.WarlockMinTime, Constants.WarlockMaxTime, Constants.BaseWarlockSeed + localI));
                warlocks[i].Start();
            }

            return warlocks;
        }

        private static Thread[] StartSorcerers(FactoryData[] factoryDatas)
        {
            var sorcerers = new Thread[Constants.SorcerersCount];
            for (int i = 0; i < Constants.SorcerersCount; i++)
            {
                var localI = i;
                sorcerers[i] = new Thread(() => ThreadFunctions.Sorcerer(factoryDatas, Constants.SorcererMinTime, Constants.SorcererMaxTime, Constants.BaseSorcererSeed + localI));
                sorcerers[i].Start();
            }

            return sorcerers;
        }

        private static Thread[] StartFactories(GlobalState globalState, out FactoryData[] factoryDatas)
        {
            var factoryDataLead = new FactoryData(Resource.Lead, globalState);
            var factoryDataMercury = new FactoryData(Resource.Mercury, globalState);
            var factoryDataSulfur = new FactoryData(Resource.Sulfur, globalState);

            factoryDatas = new[] { factoryDataLead, factoryDataMercury, factoryDataSulfur };
            var factoryLead = new Thread(() => ThreadFunctions.Factory(factoryDataLead, Constants.FactoryMinTime, Constants.FactoryMaxTime, Constants.FactoryLeadSeed));
            var factoryMercury = new Thread(() => ThreadFunctions.Factory(factoryDataMercury, Constants.FactoryMinTime, Constants.FactoryMaxTime, Constants.FactoryMercurySeed));
            var factorySulfur = new Thread(() => ThreadFunctions.Factory(factoryDataSulfur, Constants.FactoryMinTime, Constants.FactoryMaxTime, Constants.FactorySulfurSeed));
            factoryLead.Start();
            factoryMercury.Start();
            factorySulfur.Start();

            return new[] { factoryLead, factoryMercury, factorySulfur };
        }
    }

    public enum Resource
    {
        Lead = 0,
        Mercury = 1,
        Sulfur = 2
    }

    public enum AlchemistType
    {
        D = 0,
        A = 1,
        B = 2,
        C = 3
    }
}
