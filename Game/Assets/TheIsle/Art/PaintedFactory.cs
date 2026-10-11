using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TheIsle
{
    // Temporary hand-painted-look geometry. All gameplay colliders belong to the simulation.
    public sealed class PaintedFactory
    {
        readonly List<Object> owned = new List<Object>();
        readonly Dictionary<Color, Material> palette = new Dictionary<Color, Material>();
        readonly Texture2D brush;
        readonly Material template;
        public static readonly Color Cream = new Color(.91f,.79f,.53f);
        public static readonly Color Wood = new Color(.40f,.24f,.16f);
        public static readonly Color Brass = new Color(.77f,.51f,.20f);
        public static readonly Color Teal = new Color(.22f,.48f,.45f);

        public PaintedFactory()
        {
            template = Resources.Load<Material>("Art/Painted");
            brush = new Texture2D(128,128,TextureFormat.RGB24,false) { name="Gouache brush grain", wrapMode=TextureWrapMode.Repeat };
            var pixels = new Color[128*128];
            for(int y=0;y<128;y++) for(int x=0;x<128;x++)
            {
                float broad = Mathf.PerlinNoise(x*.09f,y*.09f);
                float stroke = Mathf.PerlinNoise(x*.035f,y*.5f);
                pixels[y*128+x]=Color.white*(.78f + .17f*broad + .08f*stroke);
            }
            brush.SetPixels(pixels); brush.Apply(); owned.Add(brush);
        }

        public Material Paint(Color pigment)
        {
            if(palette.TryGetValue(pigment,out var found)) return found;
            var mat=template ? new Material(template) : new Material(Shader.Find("TheIsle/Painted Surface"));
            mat.name="Pigment "+pigment; mat.SetColor("_BaseColor",pigment); mat.SetTexture("_BaseMap",brush);
            palette.Add(pigment,mat); owned.Add(mat); return mat;
        }

        public GameObject Shape(Transform parent,string name,PrimitiveType type,Vector3 pos,Vector3 scale,Color color)
        {
            var obj=GameObject.CreatePrimitive(type); obj.name=name; obj.transform.SetParent(parent,false);
            obj.transform.localPosition=pos; obj.transform.localScale=scale;
            var collider=obj.GetComponent<Collider>(); if(collider) { collider.enabled=false; Object.Destroy(collider); }
            var renderer=obj.GetComponent<Renderer>(); renderer.sharedMaterial=Paint(color);
            renderer.shadowCastingMode=ShadowCastingMode.Off; return obj;
        }

        public GameObject Ship(Transform parent,bool pirate)
        {
            var model=new GameObject(pirate?"Corsair silhouette":"Zeppelin — painted prototype"); model.transform.SetParent(parent,false);
            var t=model.transform;
            Shape(t,"Gondola",PrimitiveType.Capsule,new Vector3(0,.3f,0),new Vector3(2.5f,3.6f,1.6f),Wood).transform.localRotation=Quaternion.Euler(90,0,0);
            Shape(t,"Deck",PrimitiveType.Cube,new Vector3(0,.9f,0),new Vector3(2.2f,.25f,5.8f),Cream*.8f);
            Shape(t,"Lifting envelope",PrimitiveType.Sphere,new Vector3(0,4.9f,0),new Vector3(4.4f,4.2f,10.2f),pirate?new Color(.65f,.27f,.23f):Cream);
            for(int i=-2;i<=2;i++)
            {
                float z=i*1.8f; float radius=Mathf.Sqrt(1-z*z/26)*2.22f;
                Ring(t,"Canvas rib",new Vector3(0,4.9f,z),radius,.065f,Wood*.8f,Quaternion.Euler(90,0,0),.95f);
            }
            foreach(float x in new[]{-1.45f,1.45f}) foreach(float z in new[]{-2.3f,2.3f})
            {
                var rope=Shape(t,"Suspension rope",PrimitiveType.Cylinder,new Vector3(x,2.1f,z),new Vector3(.07f,1.25f,.07f),Wood);
                rope.transform.localRotation=Quaternion.Euler(0,0,x>0?-12:12);
                var gun=Shape(t,"Broadside cannon",PrimitiveType.Cylinder,new Vector3(x,.9f,z*.65f),new Vector3(.34f,.75f,.34f),new Color(.24f,.30f,.33f));
                gun.transform.localRotation=Quaternion.Euler(0,0,90);
            }
            Shape(t,"Wheelhouse",PrimitiveType.Cube,new Vector3(0,1.4f,-1.7f),new Vector3(1.6f,1.1f,1.5f),Teal);
            Shape(t,"Forward window",PrimitiveType.Cube,new Vector3(0,1.55f,-.93f),new Vector3(1.2f,.45f,.03f),new Color(.55f,.76f,.77f));
            Shape(t,"Tail fin",PrimitiveType.Cube,new Vector3(0,4.9f,-5.0f),new Vector3(.18f,2.4f,1.7f),pirate?Wood:Teal);
            var flag=Shape(t,"Pennant",PrimitiveType.Cube,new Vector3(0,7.55f,-2.9f),new Vector3(.08f,1,1.2f),pirate?new Color(.16f,.19f,.22f):Teal);
            Shape(t,"Flagstaff",PrimitiveType.Cylinder,new Vector3(0,6.7f,-2.9f),new Vector3(.05f,1.3f,.05f),Wood);
            for(int side=-1;side<=1;side+=2)
            {
                Shape(t,"Engine pod",PrimitiveType.Capsule,new Vector3(side*2.05f,1.1f,-2.8f),new Vector3(.8f,.9f,.8f),Brass).transform.localRotation=Quaternion.Euler(90,0,0);
                var prop=new GameObject("Propeller"); prop.transform.SetParent(t,false); prop.transform.localPosition=new Vector3(side*2.05f,1.1f,-3.7f);
                Shape(prop.transform,"Blade A",PrimitiveType.Cube,Vector3.zero,new Vector3(1.9f,.14f,.12f),Wood);
                Shape(prop.transform,"Blade B",PrimitiveType.Cube,Vector3.zero,new Vector3(.14f,1.9f,.12f),Wood);
            }
            return model;
        }

        public GameObject Island(Transform parent,WorldSite site,int seed)
        {
            var obj=new GameObject(site.Name); obj.transform.SetParent(parent,false); obj.transform.position=site.Position;
            const int n=24; float radius=site.IsPort?11:8;
            var vertices=new Vector3[1+n*3]; var uv=new Vector2[vertices.Length]; var triangles=new List<int>();
            vertices[0]=new Vector3(0,.4f,0); uv[0]=Vector2.one*.5f;
            var random=new System.Random(seed+site.Id*83);
            for(int ring=0;ring<3;ring++) for(int i=0;i<n;i++)
            {
                float angle=i*Mathf.PI*2/n; float r=radius*(ring==0?1: ring==1?.85f:.13f);
                r*=.88f+(float)random.NextDouble()*.22f;
                int index=1+ring*n+i;
                vertices[index]=new Vector3(Mathf.Cos(angle)*r,ring==0?.1f:ring==1?-5.5f:-14.0f,Mathf.Sin(angle)*r);
                uv[index]=new Vector2(vertices[index].x,vertices[index].z)*.09f;
                int next=(i+1)%n;
                if(ring==0) { triangles.Add(0); triangles.Add(1+next); triangles.Add(1+i); }
                if(ring<2)
                {
                    triangles.Add(index); triangles.Add(1+ring*n+next); triangles.Add(index+n);
                    triangles.Add(index+n); triangles.Add(1+ring*n+next); triangles.Add(1+(ring+1)*n+next);
                }
            }
            var mesh=new Mesh { name="Suspended island" }; mesh.vertices=vertices; mesh.uv=uv; mesh.triangles=triangles.ToArray(); mesh.RecalculateNormals(); owned.Add(mesh);
            obj.AddComponent<MeshFilter>().sharedMesh=mesh; obj.AddComponent<MeshRenderer>().sharedMaterial=Paint(new Color(.49f,.44f,.34f));
            Shape(obj.transform,"Meadow",PrimitiveType.Sphere,new Vector3(0,.25f,0),new Vector3(radius*1.9f,1.3f,radius*1.85f),site.Layer==0?new Color(.43f,.57f,.34f):new Color(.37f,.46f,.44f));
            var obstacle=obj.AddComponent<SphereCollider>(); obstacle.radius=radius*.82f; obstacle.center=new Vector3(0,-1.5f,0);
            for(int i=0;i<7;i++)
            {
                float angle=i*2.399f; var pos=new Vector3(Mathf.Cos(angle)*radius*.6f,.8f,Mathf.Sin(angle)*radius*.6f);
                Shape(obj.transform,"Tree trunk",PrimitiveType.Cylinder,pos+Vector3.up,new Vector3(.32f,1,.32f),Wood);
                Shape(obj.transform,"Painted tree crown",PrimitiveType.Sphere,pos+Vector3.up*2.5f,new Vector3(2.4f,3,2.1f),new Color(.25f,.43f,.34f));
            }
            if(site.IsPort)
            {
                for(int i=-1;i<=1;i++)
                {
                    var pos=new Vector3(i*3.6f,1.6f,-2);
                    Shape(obj.transform,"Harbor house",PrimitiveType.Cube,pos,new Vector3(2.7f,2.3f,2.7f),Cream);
                    Shape(obj.transform,"Terracotta roof",PrimitiveType.Cube,pos+Vector3.up*1.45f,new Vector3(3.1f,.9f,3.1f),new Color(.67f,.32f,.23f)).transform.localRotation=Quaternion.Euler(0,0,8);
                }
                Shape(obj.transform,"Dock",PrimitiveType.Cube,new Vector3(0,.8f,12),new Vector3(3.7f,.35f,10),Wood);
                Shape(obj.transform,"Harbor lamp",PrimitiveType.Sphere,new Vector3(2,2.4f,13),Vector3.one*.65f,new Color(1,.79f,.3f));
                Ring(obj.transform,"Dock approach marker",new Vector3(0,-.1f,0),19,.16f,Brass,Quaternion.identity);
            }
            else
            {
                for(int i=0;i<4;i++) Shape(obj.transform,"Cloud crystal",PrimitiveType.Cube,new Vector3(i-1.5f,1,-1),new Vector3(.7f,2,.7f),new Color(.48f,.72f,.77f)).transform.localRotation=Quaternion.Euler(0,25,i*12);
            }
            return obj;
        }

        public GameObject Gate(Transform parent,WorldSite site)
        {
            var obj=new GameObject(site.Name); obj.transform.SetParent(parent,false); obj.transform.position=site.Position;
            for(int i=0;i<3;i++) Ring(obj.transform,"Cloud passage ring",new Vector3(0,-1+i*.5f,0),9-i*1.5f,.17f,new Color(.39f,.67f,.70f),Quaternion.identity);
            for(int i=0;i<6;i++)
            {
                float a=i*Mathf.PI/3;
                Shape(obj.transform,"Ancient passage buoy",PrimitiveType.Capsule,new Vector3(Mathf.Cos(a)*10,0,Mathf.Sin(a)*10),new Vector3(.6f,1.4f,.6f),Brass);
            }
            return obj;
        }

        public GameObject Ring(Transform parent,string name,Vector3 pos,float radius,float width,Color color,Quaternion rotation,float squeeze=1)
        {
            const int n=48; var vertices=new Vector3[n*2]; var uv=new Vector2[n*2]; var triangles=new int[n*6];
            for(int i=0;i<n;i++)
            {
                float a=i*Mathf.PI*2/n;
                for(int k=0;k<2;k++) { float r=radius+(k==0?-width:width); vertices[i*2+k]=new Vector3(Mathf.Cos(a)*r,0,Mathf.Sin(a)*r*squeeze); uv[i*2+k]=new Vector2((float)i/n,k); }
                int j=(i+1)%n*2; int t=i*6; triangles[t]=i*2;triangles[t+1]=j;triangles[t+2]=i*2+1;triangles[t+3]=i*2+1;triangles[t+4]=j;triangles[t+5]=j+1;
            }
            var mesh=new Mesh { name=name };mesh.vertices=vertices;mesh.uv=uv;mesh.triangles=triangles;mesh.RecalculateNormals();owned.Add(mesh);
            var obj=new GameObject(name);obj.transform.SetParent(parent,false);obj.transform.localPosition=pos;obj.transform.localRotation=rotation;
            obj.AddComponent<MeshFilter>().sharedMesh=mesh;obj.AddComponent<MeshRenderer>().sharedMaterial=Paint(color);return obj;
        }

        public TextMesh Label(Transform parent,string text,Vector3 pos)
        {
            var obj=new GameObject("Map label");obj.transform.SetParent(parent,false);obj.transform.localPosition=pos;obj.transform.rotation=Quaternion.Euler(55,45,0);
            var label=obj.AddComponent<TextMesh>();label.text=text;label.fontSize=48;label.characterSize=.18f;label.anchor=TextAnchor.MiddleCenter;label.color=new Color(.96f,.89f,.71f);
            return label;
        }

        public void Dispose() { foreach(var item in owned) if(item) Object.Destroy(item); owned.Clear(); palette.Clear(); }
    }
}
