#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace TheIsle.Editor
{
    // Editor-only acceptance exercise of real runtime components, not production shortcuts.
    public sealed class RuntimeSmoke : MonoBehaviour
    {
        int passed;
        void Check(bool result,string name)
        {
            if(!result)throw new InvalidOperationException("Runtime smoke failed: "+name);
            passed++;Debug.Log("PASS "+name);
        }
        public static void Begin()
        {
            if(!UnityEditor.EditorApplication.isPlaying)throw new InvalidOperationException("Enter Play Mode first.");
            new GameObject("Acceptance smoke test").AddComponent<RuntimeSmoke>();
        }
        IEnumerator Start()
        {
            var boot=UnityEngine.Object.FindAnyObjectByType<DemoBootstrap>();
            boot.NewVoyage(371);
            yield return null;
            var ship=boot.Player;var session=boot.Session;var world=boot.World;
            var crew=ship.GetComponent<CrewRoster>();var vitals=ship.GetComponent<ShipVitals>();
            ship.ControlsEnabled=false;
            Check(FlightPlane.Distance(ship.transform.position,new Vector3(0,0,18))<.1f,"start position survives physics");
            Check(world.Sites[0].Discovered&&!world.Sites[1].Discovered,"initial fog preserves unknown destination");
            session.ToggleDock();Check(session.IsDocked,"start harbor docking");
            int money=session.Credits;Check(!session.Sell(GoodsKind.Parts)&&session.MissionParts==4,"mission freight cannot be sold");
            Check(session.Buy(GoodsKind.Supplies)&&session.GoodsCount(GoodsKind.Supplies)==1&&session.Credits<money,"trade purchase consumes credits");
            Check(session.PurchaseUpgrade(UpgradeKind.Winch)&&session.HasWinch,"winch purchase");
            session.ToggleDock();ship.ControlsEnabled=false;
            Check(!world.TryTransition(),"transition outside gate rejected");
            DemoBootstrap.SetPose(ship,world.Sites[5].Position);Check(world.TryTransition()&&ship.Layer==1,"gate transition lower layer");
            DemoBootstrap.SetPose(ship,world.Sites[7].Position);world.RevealAt(ship.transform.position,1,40);
            Check(session.Salvage()&&session.GoodsCount(GoodsKind.Crystal)==2,"winch salvages finite wreck");
            Check(!session.Salvage(),"same wreck cannot duplicate loot");
            DemoBootstrap.SetPose(ship,world.Sites[1].Position+Vector3.forward*18);world.RevealAt(ship.transform.position,1,40);
            session.ToggleDock();Check(session.IsDocked&&session.DockedPort.Layer==1,"lower harbor docking");
            money=session.Credits;Check(session.Deliver()&&session.DeliveryComplete&&session.Credits==money+350,"delivery reward once");
            Check(!session.Deliver()&&session.Credits==money+350,"duplicate delivery rejected");
            Check(world.StormActive&&world.Sites[5].Blocked&&!world.Sites[6].Blocked,"storm leaves alternate route available");
            session.ToggleDock();ship.ControlsEnabled=false;
            DemoBootstrap.SetPose(ship,world.Sites[5].Position);Check(!world.TryTransition()&&ship.Layer==1,"blocked passage rejects transition");
            DemoBootstrap.SetPose(ship,world.Sites[6].Position);Check(world.TryTransition()&&ship.Layer==0,"alternate passage returns home");
            DemoBootstrap.SetPose(ship,new Vector3(0,0,18));session.ToggleDock();
            Check(session.DemoComplete,"earlier upgrade is recognized on return without forcing another purchase");
            Check(session.PurchaseUpgrade(UpgradeKind.Engine)&&ship.EngineUpgrade&&session.DemoComplete,"return upgrade completes voyage");
            vitals.Damage(70,ShipModule.Engine);float beforeHull=vitals.Hull;float beforeBudget=vitals.RepairBudget;
            Check(vitals.StartRepair(ShipModule.Hull),"field repair starts");
            Check(crew.Transfer(CrewStation.Gunnery,CrewStation.Engineering),"crew transfer accepted");
            Check(crew.InTransit==1&&crew.Count(CrewStation.Gunnery)==1,"transfer removes worker during transit");
            yield return new WaitForSeconds(5.2f);
            Check(crew.InTransit==0&&crew.Count(CrewStation.Engineering)==3&&crew.WorkRate(CrewStation.Gunnery)<1,"crew arrives and gun speed loses manpower");
            Check(vitals.Hull>beforeHull&&vitals.RepairBudget<beforeBudget,"repair consumes shared field budget");
            vitals.StopRepair();money=session.Credits;Check(session.HarborService()&&session.Credits<money&&Mathf.Approximately(vitals.Hull,vitals.MaxHull)&&ship.Energy==100,"paid harbor service restores ship");
            Check(UnityEngine.Object.FindObjectsByType<AudioSource>().Length>=9,"audio sources wired");
            var engine=Resources.Load<AudioClip>("Audio/engine");var samples=new float[128];Check(engine!=null&&engine.GetData(samples,300),"real audio samples readable");
            bool signal=false;foreach(var sample in samples)if(Mathf.Abs(sample)>.01f)signal=true;Check(signal,"engine audio contains nonzero signal");
            boot.StartDuel();yield return null;
            var player=boot.Player;var enemy=boot.Pirate;var playerHealth=player.GetComponent<ShipVitals>();var enemyHealth=enemy.GetComponent<ShipVitals>();
            player.ControlsEnabled=false;enemy.ControlsEnabled=false;
            DemoBootstrap.SetPose(player,new Vector3(-180,0,-120));DemoBootstrap.SetPose(enemy,new Vector3(-120,0,-120));
            var weapons=player.GetComponent<BroadsideCombat>();player.ControlsEnabled=true;
            Check(playerHealth.Hull==enemyHealth.Hull&&player.CargoMass==enemy.CargoMass&&boot.World.Wind.magnitude==0,"equal duel mass hull and calm wind");
            Check(!weapons.TryFire(Broadside.Port,enemy.transform.position),"wrong side cannot shoot target");
            Check(weapons.TryFire(Broadside.Starboard,enemy.transform.position),"correct broadside fires");
            player.ControlsEnabled=false;
            yield return new WaitForSeconds(2.3f);
            Check(enemyHealth.Hull<enemyHealth.MaxHull,"physical projectile hits opposing hull");
            Check(weapons.Reload01(Broadside.Starboard)<1,"reload blocks immediate second volley");
            float shotDamage=enemyHealth.MaxHull-enemyHealth.Hull;Check(shotDamage>=8&&shotDamage<=16,"one broadside deals bounded damage");
            var camera=Camera.main;var rig=camera.GetComponent<FlightCameraRig>();
            var playerScreen=camera.WorldToViewportPoint(player.transform.position);var enemyScreen=camera.WorldToViewportPoint(enemy.transform.position);
            Check(playerScreen.x>.08f&&playerScreen.x<.92f&&playerScreen.y>.2f&&playerScreen.y<.85f&&enemyScreen.x>.08f&&enemyScreen.x<.92f&&enemyScreen.y>.2f&&enemyScreen.y<.85f,"combat camera keeps both ships within HUD margins");
            rig.SetManualZoom(60);yield return new WaitForSeconds(1.2f);
            Check(!rig.Automatic&&Mathf.Abs(camera.orthographicSize-60)<.2f,"manual zoom persists during combat");
            rig.ResetView();Check(rig.Automatic,"camera reset restores automatic framing");
            var hud=UnityEngine.Object.FindAnyObjectByType<DemoHud>();hud.ToggleHelp();yield return null;
            Check(hud.HelpOpen&&Time.timeScale==0&&AudioListener.pause,"help pauses simulation and sound");
            player.ControlsEnabled=true;float pausedReload=weapons.Reload01(Broadside.Port);
            Check(!weapons.TryFire(Broadside.Port,player.transform.position-player.transform.right*60)&&weapons.Reload01(Broadside.Port)==pausedReload,"pause prevents firing and preserves loaded bank");
            player.ControlsEnabled=false;
            hud.ToggleHelp();Check(Time.timeScale==1&&!AudioListener.pause,"closing help resumes simulation and sound");
            hud.ToggleHelp();
            boot.NewVoyage(372);yield return null;
            Check(Time.timeScale==1&&!AudioListener.pause,"new voyage clears previous help pause");
            Check(boot.World.Seed==372&&UnityEngine.Object.FindObjectsByType<DemoHud>().Length==1&&UnityEngine.Object.FindObjectsByType<AudioListener>().Length==1,"new voyage cleans UI audio and seed");
            boot.Player.ControlsEnabled=false;boot.Pirate.ControlsEnabled=false;boot.World.WindMultiplier=0;
            var light=Probe(boot.World,new Vector3(600,0,600));var heavy=Probe(boot.World,new Vector3(630,0,600));heavy.CargoMass=120;
            light.SetCommand(1,0,false);heavy.SetCommand(1,0,false);
            yield return new WaitForSeconds(3);
            Check(light.Speed>heavy.Speed+1&&heavy.Speed>0,"load reduces acceleration in real physics");
            Check(heavy.Energy<light.Energy,"load increases real energy use");
            float coasting=light.Speed;light.SetCommand(0,0,false);
            yield return new WaitForSeconds(.5f);
            Check(light.Speed>0&&light.Speed<coasting,"ship coasts with inertia after releasing thrust");
            light.SetCommand(0,0,true);
            yield return new WaitForSeconds(1);
            Check(light.Speed<coasting*.4f,"braking produces shorter stopping motion");
            light.Stop();light.SetCommand(0,1,false);float angle=light.transform.eulerAngles.y;
            yield return new WaitForSeconds(1);
            Check(Mathf.Abs(Mathf.DeltaAngle(angle,light.transform.eulerAngles.y))>5,"torque turns the physical ship");
            float turning=light.AngularSpeedDegrees;light.SetCommand(0,0,false);
            yield return new WaitForSeconds(.5f);
            Check(light.AngularSpeedDegrees>0&&light.AngularSpeedDegrees<turning*.6f,"released rudder smoothly reduces yaw inertia");
            light.SetCommand(0,-1,false);yield return new WaitForSeconds(.5f);
            Check(light.AngularSpeedDegrees<0,"opposite rudder counters turn promptly");
            light.SetCommand(0,0,true);yield return new WaitForSeconds(.5f);
            Check(Mathf.Abs(light.AngularSpeedDegrees)<2,"braking also settles yaw");
            light.Stop();boot.World.WindMultiplier=1;
            yield return new WaitForSeconds(2);
            Check(light.Speed>.3f,"wind drifts an unpowered ship");
            boot.World.WindMultiplier=0;light.Stop();heavy.Stop();light.ControlsEnabled=false;heavy.ControlsEnabled=false;
            var probeGuns=light.gameObject.AddComponent<BroadsideCombat>();probeGuns.Initialize(light,light.GetComponent<ShipVitals>(),light.GetComponent<CrewRoster>());
            light.ControlsEnabled=true;
            Check(probeGuns.TryFire(Broadside.Port,light.transform.position-light.transform.right*60)&&probeGuns.TryFire(Broadside.Starboard,light.transform.position+light.transform.right*60),"two initially loaded banks can fire");
            light.ControlsEnabled=false;
            yield return new WaitForSeconds(10.2f);
            Check(probeGuns.Reload01(Broadside.Port)<.6f&&probeGuns.Reload01(Broadside.Starboard)<.6f,"two banks share ten seconds of crew work");
            yield return new WaitForSeconds(10.2f);
            Check(probeGuns.Reload01(Broadside.Port)==1&&probeGuns.Reload01(Broadside.Starboard)==1,"both shared reload banks finish after twenty seconds");
            Destroy(light.gameObject);Destroy(heavy.gameObject);
            Directory.CreateDirectory("../artifacts");File.WriteAllText("../artifacts/runtime-smoke.json","{\"passed\":"+passed+",\"status\":\"passed\",\"projectileDamage\":"+shotDamage+"}");
            Debug.Log("THE_ISLE_RUNTIME_SMOKE_PASSED "+passed);Destroy(gameObject);
        }
        ShipMotor Probe(WorldMap world,Vector3 position)
        {
            var obj=new GameObject("Isolated physics probe");obj.transform.SetParent(transform,false);obj.AddComponent<Rigidbody>();
            var roster=obj.AddComponent<CrewRoster>();roster.Initialize();var health=obj.AddComponent<ShipVitals>();health.Initialize(roster,ShipTeam.Pirate);
            var ship=obj.AddComponent<ShipMotor>();ship.Initialize(health,roster,world,false);DemoBootstrap.SetPose(ship,position);return ship;
        }
    }
}
#endif
