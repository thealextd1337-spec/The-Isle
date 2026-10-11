#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace TheIsle.Editor
{
    public sealed class BattleSmoke : MonoBehaviour
    {
        public static void Begin() => new GameObject("AI battle acceptance").AddComponent<BattleSmoke>();
        IEnumerator Start()
        {
            var boot=UnityEngine.Object.FindAnyObjectByType<DemoBootstrap>();boot.StartDuel();yield return null;
            var a=boot.Player;var b=boot.Pirate;var av=a.GetComponent<ShipVitals>();var bv=b.GetComponent<ShipVitals>();
            var ac=a.GetComponent<BroadsideCombat>();var bc=b.GetComponent<BroadsideCombat>();
            a.Initialize(av,a.GetComponent<CrewRoster>(),boot.World,false);a.CargoMass=60;
            var pilot=a.gameObject.AddComponent<PiratePilot>();pilot.Initialize(a,av,ac,b);
            int aShots=0,bShots=0;float aDamage=0,bDamage=0;
            ac.Fired+=side=>aShots++;bc.Fired+=side=>bShots++;av.Damaged+=(module,amount)=>aDamage+=amount;bv.Damaged+=(module,amount)=>bDamage+=amount;
            float began=Time.time;Time.timeScale=6;
            try
            {
                while(Time.time-began<360&&!av.IsDestroyed&&!bv.IsDestroyed&&!pilot.Escaped&&!b.GetComponent<PiratePilot>().Escaped)yield return null;
                float duration=Time.time-began;
                if(aShots==0||bShots==0||aDamage==0||bDamage==0)throw new InvalidOperationException("AI battle did not exercise bilateral broadside damage.");
                bool ended=av.IsDestroyed||bv.IsDestroyed||pilot.Escaped||b.GetComponent<PiratePilot>().Escaped;
                string report="{\"seconds\":"+duration.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture)+",\"playerSalvos\":"+aShots+",\"pirateSalvos\":"+bShots+",\"playerDamage\":"+aDamage+",\"pirateDamage\":"+bDamage+",\"ended\":"+ended.ToString().ToLowerInvariant()+",\"status\":\"passed\",\"mode\":\"accelerated AI versus AI, not a human balancing test\"}";
                File.WriteAllText("../artifacts/ai-battle.json",report);Debug.Log("THE_ISLE_AI_BATTLE "+report);
            }
            finally { Time.timeScale=1; }
            Destroy(gameObject);
        }
        void OnDestroy() => Time.timeScale=1;
    }
}
#endif
