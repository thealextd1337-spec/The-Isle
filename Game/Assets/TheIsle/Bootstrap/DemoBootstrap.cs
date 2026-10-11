using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheIsle
{
    public sealed class DemoBootstrap : MonoBehaviour
    {
        readonly Dictionary<WorldSite,GameObject> sites=new Dictionary<WorldSite,GameObject>();
        GameObject voyage; PaintedFactory art; Camera view; ShipMotor player,pirate; ShipVitals health,pirateHealth;
        WorldMap world; DemoSession session; BroadsideCombat combat; PiratePilot pilot;
        Transform playerModel,pirateModel,modules; LineRenderer portArc,starboardArc;
        TextMesh pirateLabel;
        bool duel,enemyNotified,destructionNotified;int shownUpgradeMask,lastLayer;float zoom=34;
        public ShipMotor Player => player;
        public ShipMotor Pirate => pirate;
        public WorldMap World => world;
        public DemoSession Session => session;

        void Start() => NewVoyage(Environment.TickCount);

        public void NewVoyage(int seed) => CreateVoyage(seed,false);
        public void StartDuel() => CreateVoyage(74133,true);

        void CreateVoyage(int seed,bool fight)
        {
            if(voyage) { voyage.SetActive(false);Destroy(voyage); }
            if(art!=null)art.Dispose();
            sites.Clear();art=new PaintedFactory();duel=fight;zoom=fight?48:34;enemyNotified=destructionNotified=false;shownUpgradeMask=-1;
            voyage=new GameObject(fight?"Breitseitenduell":"Neue Expedition");voyage.transform.SetParent(transform,false);
            world=Add<WorldMap>("Seed world");
            player=CreateShip("Abendfalke",false,out health,out var crew,out combat);
            Vector3 startPosition=fight?new Vector3(-180,0,-120):new Vector3(0,0,18);
            world.Initialize(seed,player,crew);player.Initialize(health,crew,world,true);
            SetPose(player,startPosition);
            session=Add<DemoSession>("Voyage session");session.Initialize(player,health,crew,world,combat);
            if(fight) { world.WindMultiplier=0;session.SetDuelMode();player.CargoMass=60; }
            session.NewVoyageRequested+=NewRandomVoyage;session.DuelRequested+=StartDuel;
            CreateCamera();
            foreach(var site in world.Sites)
            {
                GameObject visual;
                if(site.IsGate)visual=art.Gate(voyage.transform,site);
                else if(site.IsSalvage)
                {
                    visual=art.Ship(voyage.transform,true);visual.name=site.Name;visual.transform.position=site.Position;visual.transform.localRotation=Quaternion.Euler(0,-30,16);visual.transform.localScale=Vector3.one*.7f;
                }
                else visual=art.Island(voyage.transform,site,seed);
                art.Label(visual.transform,site.Name,new Vector3(0,10,0));sites.Add(site,visual);
            }
            playerModel=art.Ship(player.transform,false).transform;
            modules=new GameObject("Visible modular upgrades").transform;modules.SetParent(playerModel,false);
            portArc=CreateArc("Backbord",Broadside.Port);starboardArc=CreateArc("Steuerbord",Broadside.Starboard);
            pirate=CreateShip("Wolkenkorsar",true,out pirateHealth,out var pirateCrew,out var pirateGuns);
            pirate.Initialize(pirateHealth,pirateCrew,world,false);pirate.CargoMass=60;
            if(fight)SetPose(pirate,player.transform.position+Vector3.right*60);
            else { pirate.SetLayer(1);SetPose(pirate,world.Sites[1].Position+new Vector3(45,0,-65)); }
            pirateModel=art.Ship(pirate.transform,true).transform;
            pirateLabel=art.Label(pirateModel,"Wolkenkorsar",new Vector3(0,9.5f,0));
            pilot=pirate.gameObject.AddComponent<PiratePilot>();pilot.Initialize(pirate,pirateHealth,pirateGuns,player);
            view.gameObject.AddComponent<FlightCameraRig>().Initialize(player,pirate,pirateHealth,pilot,session,fight);
            var clouds=Add<CloudField>("Moving painted cloud deck");clouds.Initialize(player,world,seed);
            var audio=Add<DemoAudio>("Soundscape");audio.Initialize(player,health,world,combat,session);
            pirateGuns.Fired+=side=>audio.Play("cannon",.38f);pirateHealth.Damaged+=(module,damage)=>audio.Play("hit",.25f);
            var hud=Add<DemoHud>("Captain's instruments");hud.Initialize(session,player,health,crew,world,combat);hud.UiActivated+=()=>audio.Play("ui",.35f);
            session.Notify(fight?"Gleichwertiges Duell · ruhiger Wind · Q/R feuert seitlich. Die Maus bestimmt das Ziel im Feuerbogen.":"Vier Auftragsteile an Bord. E legt am Startanleger an. Entdecke eine Passage zur unteren Ebene.");
            lastLayer=player.Layer;
            view.transform.position=player.transform.position+new Vector3(0,3,0)-view.transform.forward*100;
            UpdatePresentation();
        }

        void NewRandomVoyage() => NewVoyage(Environment.TickCount);
        public static void SetPose(ShipMotor ship,Vector3 position)
        {
            ship.Stop();var body=ship.GetComponent<Rigidbody>();body.position=position;ship.transform.position=position;
        }
        T Add<T>(string name) where T:Component
        {
            var obj=new GameObject(name);obj.transform.SetParent(voyage.transform,false);return obj.AddComponent<T>();
        }
        ShipMotor CreateShip(string name,bool enemy,out ShipVitals vitals,out CrewRoster crew,out BroadsideCombat guns)
        {
            var obj=new GameObject(name);obj.transform.SetParent(voyage.transform,false);
            var box=obj.AddComponent<BoxCollider>();box.size=new Vector3(4.5f,3,10);box.center=Vector3.zero;
            var body=obj.AddComponent<Rigidbody>();body.useGravity=false;
            crew=obj.AddComponent<CrewRoster>();crew.Initialize();
            vitals=obj.AddComponent<ShipVitals>();vitals.Initialize(crew,enemy?ShipTeam.Pirate:ShipTeam.Player);
            var motor=obj.AddComponent<ShipMotor>();guns=obj.AddComponent<BroadsideCombat>();guns.Initialize(motor,vitals,crew);return motor;
        }
        void CreateCamera()
        {
            view=Add<Camera>("Isometric camera");view.tag="MainCamera";view.orthographic=true;view.orthographicSize=zoom;
            view.nearClipPlane=.1f;view.farClipPlane=600;view.backgroundColor=new Color(.63f,.77f,.81f);view.clearFlags=CameraClearFlags.SolidColor;
            view.transform.rotation=Quaternion.Euler(55,45,0);view.gameObject.AddComponent<AudioListener>();
            var light=Add<Light>("Warm afternoon");light.type=LightType.Directional;light.color=new Color(1,.88f,.69f);light.intensity=1.3f;light.transform.rotation=Quaternion.Euler(50,-30,0);
            RenderSettings.ambientLight=new Color(.63f,.71f,.72f);
        }
        LineRenderer CreateArc(string name,Broadside side)
        {
            var arc=Add<LineRenderer>(name+" Feuerbogen");arc.transform.SetParent(player.transform,false);arc.useWorldSpace=false;arc.loop=false;arc.positionCount=19;
            for(int i=0;i<19;i++)
            {
                float angle=(-30+i*60f/18)*Mathf.Deg2Rad;float sign=side==Broadside.Port?-1:1;
                arc.SetPosition(i,new Vector3(Mathf.Cos(angle)*sign*26,.1f,Mathf.Sin(angle)*26));
            }
            arc.startWidth=arc.endWidth=.11f;arc.sharedMaterial=art.Paint(new Color(.83f,.67f,.35f));return arc;
        }
        void Update()
        {
            if(!player||!world)return;
            if(lastLayer!=player.Layer)
            {
                foreach(var shell in voyage.GetComponentsInChildren<Cannonball>())Destroy(shell.gameObject);
                lastLayer=player.Layer;
            }
            UpdatePresentation();
            if(health.IsDestroyed&&!destructionNotified)
            {
                destructionNotified=true;session.Notify("Komplett zerstört: Hauptbesatzung verloren. Starte eine neue Reise oder das Duell erneut.");
            }
            if((pirateHealth.IsDestroyed||pilot.Escaped)&&!enemyNotified)
            {
                enemyNotified=true;session.Notify(pirateHealth.IsDestroyed?"Korsar zerstört. Deine Reise kann weitergehen.":"Begegnung beendet: Die Schiffe haben sich getrennt.");
            }
            if(duel&&pirateHealth.Integrity(ShipModule.Hull)<.25f&&!enemyNotified)
                session.Notify("Korsar schwer beschädigt und auf dem Rückzug. Verfolgen oder ziehen lassen.");
        }
        void UpdatePresentation()
        {
            foreach(var pair in sites)pair.Value.SetActive(!duel&&pair.Key.Discovered&&(pair.Key.Layer==-1||pair.Key.Layer==player.Layer));
            bool sameLayer=pirate.Layer==player.Layer&&!pilot.Escaped;
            pirateModel.gameObject.SetActive(sameLayer);
            pirateLabel.text="Wolkenkorsar · "+Mathf.RoundToInt(pirateHealth.Integrity(ShipModule.Hull)*100)+"%";
            pirate.GetComponent<Collider>().enabled=sameLayer&&!pirateHealth.IsDestroyed;
            if(!sameLayer)pirate.Stop();
            playerModel.gameObject.SetActive(!health.IsDestroyed);pirateModel.gameObject.SetActive(sameLayer&&!pirateHealth.IsDestroyed);
            foreach(Transform part in playerModel)if(part.name=="Propeller")part.Rotate(0,0,(90+Mathf.Abs(player.Throttle)*600)*Time.deltaTime,Space.Self);
            if(sameLayer)foreach(Transform part in pirateModel)if(part.name=="Propeller")part.Rotate(0,0,(90+Mathf.Abs(pirate.Throttle)*600)*Time.deltaTime,Space.Self);
            portArc.enabled=starboardArc.enabled=!session.IsDocked&&!health.IsDestroyed;
            int mask=(session.HasUpgrade(UpgradeKind.Cargo)?1:0)|(session.HasUpgrade(UpgradeKind.Engine)?2:0)|(session.HasWinch?4:0);
            if(mask!=shownUpgradeMask)
            {
                foreach(Transform child in modules)Destroy(child.gameObject);
                if((mask&1)!=0)for(int i=0;i<3;i++)art.Shape(modules,"Freight crate",PrimitiveType.Cube,new Vector3(i-1,1.4f,1.5f),Vector3.one*.9f,PaintedFactory.Wood);
                if((mask&2)!=0)art.Shape(modules,"Extra motor",PrimitiveType.Capsule,new Vector3(0,1.4f,-3.8f),new Vector3(1,1.2f,1),PaintedFactory.Brass);
                if((mask&4)!=0) { art.Ring(modules,"Salvage winch",new Vector3(1.7f,.5f,0),.75f,.16f,PaintedFactory.Brass,Quaternion.Euler(90,0,0));art.Shape(modules,"Winch cable",PrimitiveType.Cylinder,new Vector3(1.7f,-.7f,0),new Vector3(.07f,.7f,.07f),PaintedFactory.Wood); }
                shownUpgradeMask=mask;
            }
            view.backgroundColor=player.Layer==0?new Color(.63f,.77f,.81f):new Color(.34f,.47f,.57f);
        }
        void OnDestroy() { if(art!=null)art.Dispose(); }
    }
}
