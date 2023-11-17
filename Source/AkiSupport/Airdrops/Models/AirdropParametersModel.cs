﻿using Aki.Custom.Airdrops.Models;

namespace SIT.Core.AkiSupport.Airdrops.Models
{
    public class AirdropParametersModel
    {
        public AirdropConfigModel Config;

        public bool AirdropAvailable;
        public bool PlaneSpawned;
        public bool BoxSpawned;
        public float DistanceTraveled;
        public float DistanceToTravel;
        public float DistanceToDrop;
        public float Timer;
        public int DropHeight;
        public int TimeToStart;

        public UnityEngine.Vector3 RandomAirdropPoint;
    }
}