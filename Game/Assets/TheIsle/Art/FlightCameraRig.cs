using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace TheIsle
{
    [RequireComponent(typeof(Camera))]
    public sealed class FlightCameraRig : MonoBehaviour
    {
        Camera view;ShipMotor player,opponent;ShipVitals opponentHealth;PiratePilot pilot;DemoSession voyage;
        float userZoom=34,yaw=45;bool duel;
        public bool Automatic { get; private set; }=true;
        public Vector3 FocusPoint { get; private set; }

        public void Initialize(ShipMotor ship,ShipMotor enemy,ShipVitals health,PiratePilot enemyPilot,DemoSession session,bool duelMode)
        {
            view=GetComponent<Camera>();player=ship;opponent=enemy;opponentHealth=health;pilot=enemyPilot;voyage=session;duel=duelMode;
            ResetView();FocusPoint=player.transform.position;
            transform.position=FocusPoint+Vector3.up*3-transform.forward*100;
        }
        public void ResetView() { Automatic=true;userZoom=duel?48:34;yaw=45; }
        public void SetManualZoom(float size) { userZoom=Mathf.Clamp(size,24,85);Automatic=false; }
        void Update()
        {
            if(!player||Time.timeScale==0)return;
            var keyboard=Keyboard.current;
            if(keyboard!=null)
            {
                if(keyboard.cKey.wasPressedThisFrame)ResetView();
                if(keyboard.zKey.wasPressedThisFrame)yaw-=45;
                if(keyboard.xKey.wasPressedThisFrame)yaw+=45;
                if(keyboard.minusKey.wasPressedThisFrame)SetManualZoom(view.orthographicSize+5);
                if(keyboard.equalsKey.wasPressedThisFrame)SetManualZoom(view.orthographicSize-5);
            }
            bool overUi=EventSystem.current!=null&&EventSystem.current.IsPointerOverGameObject();
            if(Mouse.current!=null&&!overUi&&Mathf.Abs(Mouse.current.scroll.ReadValue().y)>.01f)
                SetManualZoom(view.orthographicSize-Mouse.current.scroll.ReadValue().y*.04f);
        }
        void LateUpdate()
        {
            if(!player)return;
            float smoothing=1-Mathf.Exp(-Time.deltaTime*5);
            transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.Euler(55,yaw,0),smoothing);
            bool threat=opponent&&opponent.Layer==player.Layer&&!pilot.Escaped&&!opponentHealth.IsDestroyed&&FlightPlane.Distance(player.transform.position,opponent.transform.position)<120;
            Vector3 lead=Vector3.ClampMagnitude(player.Velocity*1.1f+player.transform.forward*(Mathf.Abs(player.Throttle)*3),12);
            float desiredZoom=userZoom;
            Vector3 target=player.transform.position+lead;
            if(voyage.IsDocked)
            {
                target=Vector3.Lerp(player.transform.position,voyage.DockedPort.Position,.22f);
                if(Automatic)desiredZoom=30;
            }
            else if(threat)
            {
                Vector3 separation=FlightPlane.Flat(opponent.transform.position-player.transform.position);
                target=player.transform.position+separation*.35f+lead*.2f;
                if(Automatic)
                {
                    // Leave room above/below the ships for the HUD and the opponent's hull label.
                    float vertical=Mathf.Abs(Vector3.Dot(separation,transform.up))*.72f+9;
                    float horizontal=Mathf.Abs(Vector3.Dot(separation,transform.right))*.72f/Mathf.Max(.7f,view.aspect)+9;
                    desiredZoom=Mathf.Clamp(Mathf.Max(duel?48:38,Mathf.Max(vertical,horizontal)/.64f),34,80);
                }
            }
            else if(Automatic)
            {
                desiredZoom=34+Mathf.Min(10,player.Speed*.45f);
                if(player.Speed<4&&voyage.CanDock)target=player.transform.position+lead*.25f;
            }
            FocusPoint=Vector3.Lerp(FocusPoint,target,smoothing);
            transform.position=FocusPoint+Vector3.up*3-transform.forward*100;
            view.orthographicSize=Mathf.Lerp(view.orthographicSize,desiredZoom,smoothing);
        }
    }
}
