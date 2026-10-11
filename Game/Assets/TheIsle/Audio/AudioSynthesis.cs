using System;

namespace TheIsle
{
    // Original synthesized demo sounds; generated WAVs are checked into the project.
    public static class AudioSynthesis
    {
        public const int SampleRate=22050;
        public static readonly string[] Names={"wind","engine","cannon","hit","repair","harbor","transition","success","ui"};
        public static float[] Generate(string name)
        {
            float duration=name=="wind"?4:name=="engine"||name=="repair"?2:name=="transition"?1.2f:name=="success"?1.1f:name=="ui"?.12f:.7f;
            var data=new float[(int)(SampleRate*duration)];
            var random=new Random(741+name.Length); float smooth=0;
            for(int i=0;i<data.Length;i++)
            {
                float t=(float)i/SampleRate; float noise=(float)random.NextDouble()*2-1;
                smooth=smooth*.97f+noise*.03f; float value=0;
                float attack=Math.Min(1,t*80),fade=(float)Math.Exp(-t*6);
                switch(name)
                {
                    case "wind": value=smooth*1.4f+noise*.008f; break;
                    case "engine": value=(float)(Math.Sin(t*2*Math.PI*44)*.18+Math.Sin(t*2*Math.PI*88)*.08+Math.Sin(t*2*Math.PI*132)*.025); break;
                    case "cannon": value=attack*fade*(noise*.65f+(float)Math.Sin(t*2*Math.PI*(80-t*35))*.35f); break;
                    case "hit": value=attack*fade*(noise*.4f+(float)Math.Sin(t*2*Math.PI*168)*.18f); break;
                    case "repair": value=noise*.06f+(float)Math.Sin(t*2*Math.PI*320)*.05f*(float)Math.Pow(Math.Max(0,Math.Sin(t*2*Math.PI*3)),12); break;
                    case "harbor": value=(float)(Math.Sin(t*2*Math.PI*440)+Math.Sin(t*2*Math.PI*660))*.2f*attack*fade; break;
                    case "transition": value=(float)Math.Sin(t*2*Math.PI*(150+t*270))*.20f*(float)Math.Sin(t/duration*Math.PI); break;
                    case "success": float frequency=t<.25f?392:t<.5f?494:t<.75f?587:784; value=(float)Math.Sin(t*2*Math.PI*frequency)*.24f*attack*(1-t/duration); break;
                    default: value=(float)Math.Sin(t*2*Math.PI*620)*.2f*attack*(1-t/duration); break;
                }
                if(name=="wind"||name=="engine"||name=="repair")
                {
                    // Cosine taper at both ends prevents a seam pop in the looping clip.
                    float edge=Math.Min(1,Math.Min(t,duration-t)*30);
                    value*=.5f-.5f*(float)Math.Cos(edge*Math.PI);
                }
                data[i]=Math.Max(-.95f,Math.Min(.95f,value));
            }
            return data;
        }
    }
}
