using System.Collections.Generic;
using UnityEngine;

namespace TheIsle
{
    public sealed class CloudField : MonoBehaviour
    {
        readonly List<Transform> clouds=new List<Transform>();
        readonly List<Material> materials=new List<Material>();
        Texture2D texture; ShipMotor player;WorldMap world;
        public void Initialize(ShipMotor ship,WorldMap map,int seed)
        {
            player=ship;world=map;
            texture=new Texture2D(128,128,TextureFormat.RGBA32,false) { name="Soft painted cloud",wrapMode=TextureWrapMode.Clamp };
            var pixels=new Color[128*128];
            for(int y=0;y<128;y++)for(int x=0;x<128;x++)
            {
                float px=(x-64)/64f,py=(y-64)/64f;
                float radius=Mathf.Sqrt(px*px+py*py);
                float brush=Mathf.PerlinNoise(x*.085f,y*.09f);
                float alpha=Mathf.SmoothStep(0,1,Mathf.Clamp01((1-radius)*2.7f+brush*.22f-.12f));
                pixels[y*128+x]=new Color(.91f+brush*.09f,.89f+brush*.10f,.84f+brush*.13f,alpha*.85f);
            }
            texture.SetPixels(pixels);texture.Apply();
            var template=Resources.Load<Material>("Art/Cloud");
            var random=new System.Random(seed);
            for(int i=0;i<48;i++)
            {
                var obj=GameObject.CreatePrimitive(PrimitiveType.Quad);obj.name="Gouache cloud";obj.transform.SetParent(transform,false);
                var collider=obj.GetComponent<Collider>();collider.enabled=false;Destroy(collider);
                obj.transform.rotation=Quaternion.Euler(90,0,0);
                float size=12+(float)random.NextDouble()*26;
                obj.transform.localScale=new Vector3(size*1.8f,size,1);
                obj.transform.position=new Vector3((float)random.NextDouble()*220-110,-8-i*.015f,(float)random.NextDouble()*220-110);
                var material=new Material(template);material.SetTexture("_BaseMap",texture);material.SetColor("_BaseColor",new Color(1,1,1,.7f));
                obj.GetComponent<Renderer>().sharedMaterial=material;materials.Add(material);clouds.Add(obj.transform);
            }
        }
        void Update()
        {
            if(!player)return;
            Vector3 motion=FlightPlane.Flat(world.Wind)*Time.deltaTime*.22f;
            foreach(var cloud in clouds)
            {
                var pos=cloud.position+motion;
                if(pos.x-player.transform.position.x>110)pos.x-=220;
                if(pos.x-player.transform.position.x< -110)pos.x+=220;
                if(pos.z-player.transform.position.z>110)pos.z-=220;
                if(pos.z-player.transform.position.z< -110)pos.z+=220;
                cloud.position=pos;
            }
        }
        void OnDestroy() { if(texture)Destroy(texture);foreach(var mat in materials)if(mat)Destroy(mat); }
    }
}
