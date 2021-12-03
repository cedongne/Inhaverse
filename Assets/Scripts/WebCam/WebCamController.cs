//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;

//namespace Unity.WebRTC.Samples
//{
//    class WebCamController : MonoBehaviour
//    {
//#pragma warning disable 0649
//        WebCamTexture camTexture;
//        [SerializeField] private Vector2Int streamingSize;
//        [SerializeField] private Camera cam;
//        [SerializeField] private RawImage sourceImage;
//        [SerializeField] private RawImage receiveImage1;
//      //  [SerializeField] private RawImage receiveImage2;
//#pragma warning restore 0649

//        private static RTCConfiguration configuration = new RTCConfiguration
//        {
//            iceServers = new[] { new RTCIceServer { urls = new[] { "stun:stun.l.google.com:19302" } } }
//        };

//        private RTCPeerConnection pc1Local;//, pc1Remote; /*pc2Local, pc2Remote;*/
//        private MediaStream sourceStream;

//        private void Awake()
//        {
//            streamingSize.x = 1280;
//            streamingSize.y = 720;
//            //    Debug.Log(gameObject.name);
//            WebRTC.Initialize(EncoderType.Software/*WebRTCSettings.EncoderType*/, WebRTCSettings.LimitTextureSize);
//        }

//        private void OnDestroy()
//        {
//            WebRTC.Dispose();
//            HangUp();
//        }

//        private void Start()
//        {
//            StartCoroutine(WebRTC.Update());

//            Setup();
//            Call();

//        }
//        private void Setup()
//        {
//            Debug.Log("Set up source/receive streams");
//            sourceStream = new MediaStream();

//            if (camTexture != null)
//            {
//                sourceImage.texture = null;
//                camTexture.Stop();
//                camTexture = null;
//            }
//            if (WebCamTexture.devices.Length != 0)
//            {
//                WebCamDevice device = WebCamTexture.devices[0];
//                camTexture = new WebCamTexture(device.name);
//            }
//            camTexture.Play();

//            var videoTrack = new VideoStreamTrack(camTexture);
//            sourceStream.AddTrack(videoTrack);
//            sourceImage.texture = camTexture;

//        }

//        private void Call()
//        {
//            Debug.Log("Starting calls");

//            pc1Local = new RTCPeerConnection(ref configuration);
//            pc1Remote = new RTCPeerConnection(ref configuration);
//            pc1Remote.OnTrack = e =>
//            {
//                if (e.Track is VideoStreamTrack videoTrack && !videoTrack.IsDecoderInitialized)
//                {
//                    receiveImage1.texture = videoTrack.InitializeReceiver(streamingSize.x, streamingSize.y);
//                }
//            };
//            pc1Local.OnIceCandidate = candidate => pc1Remote.AddIceCandidate(candidate);
//            pc1Remote.OnIceCandidate = candidate => pc1Local.AddIceCandidate(candidate);
//            Debug.Log("pc1: created local and remote peer connection object");

//            //pc2Local = new RTCPeerConnection(ref configuration);
//            //pc2Remote = new RTCPeerConnection(ref configuration);
//            //pc2Remote.OnTrack = e =>
//            //{
//            //    if (e.Track is VideoStreamTrack videoTrack && !videoTrack.IsDecoderInitialized)
//            //    {
//            //        receiveImage2.texture = videoTrack.InitializeReceiver(streamingSize.x, streamingSize.y);
//            //    }

//            //};
//            //pc2Local.OnIceCandidate = candidate => pc2Remote.AddIceCandidate(candidate);
//            //pc2Remote.OnIceCandidate = candidate => pc2Local.AddIceCandidate(candidate);
//            //Debug.Log("pc2: created local and remote peer connection object");

//            foreach (var track in sourceStream.GetTracks())
//            {
//                pc1Local.AddTrack(track, sourceStream);
//                //    pc2Local.AddTrack(track, sourceStream);
//            }

//            Debug.Log("Adding local stream to pc1Local/pc2Local");

//            StartCoroutine(NegotiationPeer(pc1Local, pc1Remote));
//            // StartCoroutine(NegotiationPeer(pc2Local, pc2Remote));
//        }

//        private void HangUp()
//        {
//            foreach (var track in sourceStream.GetTracks())
//            {
//                track.Dispose();
//            }
//            sourceStream.Dispose();
//            sourceStream = null;
//            pc1Local.Close();
//            pc1Remote.Close();
//            //pc2Local.Close();
//            //pc2Remote.Close();
//            pc1Local.Dispose();
//            pc1Remote.Dispose();
//            //pc2Local.Dispose();
//            //pc2Remote.Dispose();
//            pc1Local = null;
//            pc1Remote = null;
//            //pc2Local = null;
//            //pc2Remote = null;

//            sourceImage.texture = null;
//            receiveImage1.texture = null;
//        }

//        private static void OnCreateSessionDescriptionError(RTCError error)
//        {
//            Debug.LogError($"Failed to create session description: {error.message}");
//        }

//        private static IEnumerator NegotiationPeer(RTCPeerConnection localPeer, RTCPeerConnection remotePeer)
//        {
//            var opCreateOffer = localPeer.CreateOffer();
//            yield return opCreateOffer;

//            if (opCreateOffer.IsError)
//            {
//                OnCreateSessionDescriptionError(opCreateOffer.Error);
//                yield break;
//            }

//            var offerDesc = opCreateOffer.Desc;
//            yield return localPeer.SetLocalDescription(ref offerDesc);
//            Debug.Log($"Offer from LocalPeer \n {offerDesc.sdp}");
//            yield return remotePeer.SetRemoteDescription(ref offerDesc);

//            var opCreateAnswer = remotePeer.CreateAnswer();
//            yield return opCreateAnswer;

//            if (opCreateAnswer.IsError)
//            {
//                OnCreateSessionDescriptionError(opCreateAnswer.Error);
//                yield break;
//            }

//            var answerDesc = opCreateAnswer.Desc;
//            yield return remotePeer.SetLocalDescription(ref answerDesc);
//            Debug.Log($"Answer from RemotePeer \n {answerDesc.sdp}");
//            yield return localPeer.SetRemoteDescription(ref answerDesc);
//        }
//    }
//}