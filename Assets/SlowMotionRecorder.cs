/* 
*   Slow Motion Camera
*   Copyright (c) 2023 NatML Inc. All Rights Reserved.
*/

namespace VideoKit.Examples {

    using UnityEngine;
    using VideoKit;
    using VideoKit.Assets;
    using VideoKit.Devices;
    using VideoKit.Devices.Outputs;
    using VideoKit.Recorders.Clocks;

    public sealed class SlowMotionRecorder : MonoBehaviour {

        #region --Inspector--
        public VideoKitCameraManager cameraManager;
        #endregion

    
        #region --Operations--
        private PixelBufferOutput output;
        private MediaRecorder recorder;
        private IClock clock;
        private bool recording;

        private void Start () {
            output = new PixelBufferOutput();
            cameraManager.OnCameraImage += OnCameraImage;
        }

        private void OnCameraImage (CameraImage image) {
            output.Update(image);
            if (recording)
                recorder.CommitFrame(output.pixelBuffer, clock.timestamp);
        }

        private void OnDisable () {
            cameraManager.OnCameraImage -= OnCameraImage;
        }
        #endregion


        #region --UI handlers--

        public async void StartRecording () {
            // Start recording
            clock = new RealtimeClock();
            recorder = await MediaRecorder.Create(MediaFormat.MP4, output.width, output.height, 240);
            recording = true;
        }

        public async void StopRecording () {
            // Stop recording
            recording = false;
            var path = await recorder.FinishWriting();
            // Save to the camera roll
            var asset = await MediaAsset.FromFile(path);
            await asset.SaveToCameraRoll();
        }
        #endregion
    }
}