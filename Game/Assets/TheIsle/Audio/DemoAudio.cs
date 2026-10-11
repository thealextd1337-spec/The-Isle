using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheIsle
{
    public sealed class DemoAudio : MonoBehaviour
    {
        public static float Volume => PlayerPrefs.GetFloat("TheIsle.Volume",.65f);
        public static bool Muted => PlayerPrefs.GetInt("TheIsle.Muted",0)==1;
        readonly Dictionary<string,AudioClip> clips=new Dictionary<string,AudioClip>();
        readonly List<AudioSource> effects=new List<AudioSource>();
        readonly List<AudioClip> generatedClips=new List<AudioClip>();
        ShipMotor motor; ShipVitals vitals; WorldMap world; BroadsideCombat combat; DemoSession session;
        AudioSource wind,engine,repair; int effectIndex,lastLayer; bool wasDocked,wasComplete;

        public static void SetVolume(float volume)
        {
            PlayerPrefs.SetFloat("TheIsle.Volume",Mathf.Clamp01(volume)); PlayerPrefs.Save();
            AudioListener.volume=Muted?0:Volume;
        }
        public static void ToggleMute()
        {
            PlayerPrefs.SetInt("TheIsle.Muted",Muted?0:1);PlayerPrefs.Save();AudioListener.volume=Muted?0:Volume;
        }
        public void Initialize(ShipMotor ship,ShipVitals health,WorldMap map,BroadsideCombat guns,DemoSession voyage)
        {
            motor=ship;vitals=health;world=map;combat=guns;session=voyage;lastLayer=ship.Layer;
            foreach(string name in AudioSynthesis.Names)
            {
                var clip=Resources.Load<AudioClip>("Audio/"+name);
                if(!clip) { var samples=AudioSynthesis.Generate(name);clip=AudioClip.Create(name,samples.Length,1,AudioSynthesis.SampleRate,false);clip.SetData(samples,0);generatedClips.Add(clip); }
                clips[name]=clip;
            }
            wind=Loop("wind",0);engine=Loop("engine",0);repair=Loop("repair",0);
            for(int i=0;i<6;i++) effects.Add(gameObject.AddComponent<AudioSource>());
            combat.Fired+=OnFired;vitals.Damaged+=OnDamaged;vitals.Destroyed+=OnDestroyed;
            AudioListener.volume=Muted?0:Volume;
        }
        AudioSource Loop(string name,float volume)
        {
            var source=gameObject.AddComponent<AudioSource>();source.clip=clips[name];source.loop=true;source.volume=volume;source.Play();return source;
        }
        public void Play(string name,float volume=.65f)
        {
            if(!clips.TryGetValue(name,out var clip)||effects.Count==0)return;
            var source=effects[effectIndex++%effects.Count];source.pitch=1;source.PlayOneShot(clip,volume);
        }
        void OnFired(Broadside side) => Play("cannon",.75f);
        void OnDamaged(ShipModule module,float amount) => Play("hit",.55f);
        void OnDestroyed() => Play("hit",.9f);
        void Update()
        {
            if(!motor)return;
            if(Keyboard.current!=null&&Keyboard.current.mKey.wasPressedThisFrame)ToggleMute();
            wind.volume=Mathf.Lerp(wind.volume,.20f+Mathf.Clamp01((world.Wind.magnitude+motor.Speed)/30)*.27f,Time.deltaTime*2);
            engine.volume=Mathf.Lerp(engine.volume,vitals.IsDestroyed?0:.18f+Mathf.Abs(motor.Throttle)*.36f,Time.deltaTime*3);
            engine.pitch=.7f+Mathf.Abs(motor.Throttle)*.5f+motor.Speed*.01f;
            repair.volume=vitals.IsRepairing?.5f:0;
            if(motor.Layer!=lastLayer) { Play("transition");lastLayer=motor.Layer; }
            if(session.IsDocked&&!wasDocked)Play("harbor");wasDocked=session.IsDocked;
            if(session.DeliveryComplete&&!wasComplete)Play("success");wasComplete=session.DeliveryComplete;
        }
        void OnDestroy()
        {
            if(combat)combat.Fired-=OnFired;
            if(vitals) { vitals.Damaged-=OnDamaged;vitals.Destroyed-=OnDestroyed; }
            foreach(var clip in generatedClips)if(clip)Destroy(clip);
        }
    }
}
