using System;
using System.IO;
using UniHumanoid;
using UnityEngine;
using UnityEngine.UI;
using VRM;
using VRM.Samples;

namespace SimplestarGame
{
    public class VRMLoader : MonoBehaviour
    {
        [SerializeField, Tooltip("VRM")] Button buttonOpenVRM = null;

        internal Action<Button, GameObject, string> onLoadVRM;

        private void Start()
        {
            this.buttonOpenVRM.onClick.AddListener(this.OnOpenClicked);
        }

        void OnOpenClicked()
        {
#if UNITY_STANDALONE_WIN
            var path = FileDialogForWindows.FileDialog("open VRM", "vrm", "glb");
#else
            var path = Application.dataPath + "/default.vrm";
#endif
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var ext = Path.GetExtension(path).ToLower();
            switch (ext)
            {
                case ".gltf":
                case ".glb":
                case ".vrm":
                    LoadModel(path);
                    break;
            }
        }

        void LoadModel(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            Debug.LogFormat("{0}", path);
            this.filePath = Path.GetFileNameWithoutExtension(path);
            var ext = Path.GetExtension(path).ToLower();
            switch (ext)
            {
                case ".vrm":
                    {
                        var context = new VRMImporterContext();
                        var file = File.ReadAllBytes(path);
                        context.ParseGlb(file);
                        context.Load();
                        context.ShowMeshes();
                        context.EnableUpdateWhenOffscreen();
                        context.ShowMeshes();
                        SetModel(context.Root);
                        break;
                    }

                case ".glb":
                    {
                        var context = new UniGLTF.ImporterContext();
                        var file = File.ReadAllBytes(path);
                        context.ParseGlb(file);
                        context.Load();
                        context.ShowMeshes();
                        context.EnableUpdateWhenOffscreen();
                        context.ShowMeshes();
                        SetModel(context.Root);
                        break;
                    }

                default:
                    Debug.LogWarningFormat("unknown file type: {0}", path);
                    break;
            }
        }

        void SetModel(GameObject go)
        {
            // cleanup
            var loaded = m_loaded;
            m_loaded = null;

            if (loaded != null)
            {
                Debug.LogFormat("destroy {0}", loaded);
                GameObject.Destroy(loaded.gameObject);
            }

            if (go != null)
            {
                go.transform.rotation = Quaternion.Euler(0, 180, 0);
                go.transform.position = new Vector3(0, 0, 1.5f);

                var lookAt = go.GetComponent<VRMLookAtHead>();
                if (lookAt != null)
                {
                    m_loaded = go.AddComponent<HumanPoseTransfer>();
                    m_loaded.SourceType = HumanPoseTransfer.HumanPoseTransferSourceType.HumanPoseTransfer;

                    lookAt.UpdateType = UpdateType.LateUpdate; // after HumanPoseTransfer's setPose
                }

                var animation = go.GetComponent<Animation>();
                if (animation && animation.clip != null)
                {
                    animation.Play(animation.clip.name);
                }

                this.onLoadVRM?.Invoke(this.buttonOpenVRM, go, this.filePath);
            }
        }

        string filePath;
        HumanPoseTransfer m_loaded;
    }
}