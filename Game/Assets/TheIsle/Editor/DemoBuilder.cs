using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;

namespace TheIsle.Editor
{
    public static class DemoBuilder
    {
        public const string ScenePath="Assets/TheIsle/Scenes/FirstVoyage.unity";
        [MenuItem("The Isle/Create or refresh demo scene")]
        public static void CreateScene()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode before creating assets.");
            Directory.CreateDirectory("Assets/TheIsle/Resources/Art");Directory.CreateDirectory("Assets/TheIsle/Resources/Audio");Directory.CreateDirectory("Assets/TheIsle/Scenes");
            AssetDatabase.Refresh();
            string paintPath="Assets/TheIsle/Resources/Art/Painted.mat";
            if(!AssetDatabase.LoadAssetAtPath<Material>(paintPath))AssetDatabase.CreateAsset(new Material(Shader.Find("TheIsle/Painted Surface")),paintPath);
            string cloudPath="Assets/TheIsle/Resources/Art/Cloud.mat";
            if(!AssetDatabase.LoadAssetAtPath<Material>(cloudPath))
            {
                var cloud=new Material(Shader.Find("Universal Render Pipeline/Unlit"));cloud.SetFloat("_Surface",1);cloud.SetFloat("_Blend",0);
                cloud.SetFloat("_SrcBlend",(float)UnityEngine.Rendering.BlendMode.SrcAlpha);cloud.SetFloat("_DstBlend",(float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                cloud.SetFloat("_ZWrite",0);cloud.SetFloat("_Cull",0);cloud.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");cloud.renderQueue=3000;
                AssetDatabase.CreateAsset(cloud,cloudPath);
            }
            foreach(string name in AudioSynthesis.Names)
            {
                string path="Assets/TheIsle/Resources/Audio/"+name+".wav";
                if(!File.Exists(path))WriteWave(path,AudioSynthesis.Generate(name));
            }
            AssetDatabase.Refresh();
            foreach(string name in AudioSynthesis.Names)
            {
                var importer=AssetImporter.GetAtPath("Assets/TheIsle/Resources/Audio/"+name+".wav") as AudioImporter;
                if(importer==null)continue;
                var settings=importer.defaultSampleSettings;settings.loadType=AudioClipLoadType.DecompressOnLoad;settings.compressionFormat=AudioCompressionFormat.PCM;importer.defaultSampleSettings=settings;
                importer.forceToMono=true;importer.SaveAndReimport();
            }
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            new GameObject("The Isle Demo").AddComponent<DemoBootstrap>();
            EditorSceneManager.SaveScene(scene,ScenePath);
            EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
            PlayerSettings.companyName="The Isle";PlayerSettings.productName="The Isle — First Voyage";
            PlayerSettings.defaultScreenWidth=1600;PlayerSettings.defaultScreenHeight=900;PlayerSettings.fullScreenMode=FullScreenMode.Windowed;
            PlayerSettings.runInBackground=true;PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Standalone,ScriptingImplementation.Mono2x);
            AssetDatabase.SaveAssets();Debug.Log("THE_ISLE_SCENE_READY");
        }
        static void WriteWave(string path,float[] samples)
        {
            using(var writer=new BinaryWriter(File.Create(path)))
            {
                writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));writer.Write(36+samples.Length*2);writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));
                writer.Write(16);writer.Write((short)1);writer.Write((short)1);writer.Write(AudioSynthesis.SampleRate);writer.Write(AudioSynthesis.SampleRate*2);writer.Write((short)2);writer.Write((short)16);
                writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));writer.Write(samples.Length*2);
                foreach(float value in samples)writer.Write((short)(Mathf.Clamp(value,-1,1)*32767));
            }
        }
        public static void BuildWindows()
        {
            string output="Builds/Windows/TheIsle.exe";
            var args=Environment.GetCommandLineArgs();for(int i=0;i<args.Length-1;i++)if(args[i]=="-buildOutput")output=args[i+1];
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes=new[]{ScenePath},locationPathName=output,target=BuildTarget.StandaloneWindows64,options=BuildOptions.None });
            Debug.Log("THE_ISLE_BUILD_"+report.summary.result+" "+report.summary.totalSize);
            if(report.summary.result!=BuildResult.Succeeded)throw new InvalidOperationException("The Isle Windows build failed.");
        }
    }
}
