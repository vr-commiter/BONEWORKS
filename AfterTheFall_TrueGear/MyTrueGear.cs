using System.Collections.Generic;
using System.Threading;
using System.IO;
using System;
using TrueGearSDK;
using System.Linq;


namespace MyTrueGear
{
    public class TrueGearMod
    {
        private static TrueGearPlayer _player = null;

        private static ManualResetEvent heartbeatMRE = new ManualResetEvent(false);


        public void HeartBeat()
        {
            while (true)
            {
                heartbeatMRE.WaitOne();
                _player.SendPlay("HeartBeat");
                Thread.Sleep(1000);
            }
        }

        public TrueGearMod()
        {
            _player = new TrueGearPlayer("823500","BoneWork");
            _player.Start();
            new Thread(new ThreadStart(this.HeartBeat)).Start();
        }


        public void Play(string Event)
        {
            _player.SendPlay(Event);
        }


        public void StartHeartBeat()
        {
            heartbeatMRE.Set();
        }

        public void StopHeartBeat()
        {
            heartbeatMRE.Reset();
        }


    }
}
