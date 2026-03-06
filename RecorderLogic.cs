using System;
using System.Drawing;
using System.Windows.Forms;
using NAudio.Wave;
using SharpAvi;
using SharpAvi.Codecs;
using SharpAvi.Output;

namespace NekoBeats
{
    public class RecorderLogic : IDisposable
    {
        private bool isRecording = false;
        private int frameCount = 0;
        private string outputPath;
        
        public int RecordingWidth { get; set; } = 1920;
        public int RecordingHeight { get; set; } = 1080;
        public int RecordingFPS { get; set; } = 60;
        public int MaxDurationSeconds { get; set; } = 300;
        
        private Bitmap frameBuffer;
        private Graphics frameGraphics;
        private VisualizerForm visualizerForm;
        private VisualizerLogic visualizerLogic;
        private AviWriter aviWriter;
        private IAviVideoStream videoStream;
        
        public RecorderLogic(VisualizerForm form)
        {
            visualizerForm = form;
            visualizerLogic = form.Logic;
        }
        
        public bool StartRecording(string outputFilePath, int width, int height, int durationSeconds, int fps)
        {
            if (isRecording) return false;
            
            try
            {
                RecordingWidth = width;
                RecordingHeight = height;
                RecordingFPS = fps;
                MaxDurationSeconds = durationSeconds;
                outputPath = outputFilePath;
                frameCount = 0;
                
                // Initialize AVI writer
                aviWriter = new AviWriter(outputPath);
                
                // Add video stream with MotionJpeg codec
                videoStream = aviWriter.AddVideoStream();
                videoStream.Width = width;
                videoStream.Height = height;
                videoStream.FramesPerSecond = fps;
                videoStream.Codec = KnownFourCCs.Codecs.MotionJpeg;
                
                // Initialize frame buffer
                frameBuffer = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                frameGraphics = Graphics.FromImage(frameBuffer);
                
                isRecording = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start recording: {ex.Message}", "Recording Error");
                aviWriter?.Dispose();
                return false;
            }
        }
        
        public void StopRecording()
        {
            if (!isRecording) return;
            isRecording = false;
        }
        
        public void CaptureFrame()
        {
            if (!isRecording) return;
            
            if (frameCount >= MaxDurationSeconds * RecordingFPS)
            {
                StopRecording();
                FinalizeRecording();
                return;
            }
            
            try
            {
                frameGraphics.Clear(Color.Magenta);
                visualizerLogic.Render(frameGraphics, new Size(RecordingWidth, RecordingHeight));
                
                // Write frame to AVI video stream
                videoStream.WriteFrame(true, frameBuffer, 0, frameBuffer.Width * frameBuffer.Height * 4);
                frameCount++;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Frame capture error: {ex.Message}", "Recording Error");
                StopRecording();
            }
        }
        
        private void FinalizeRecording()
        {
            try
            {
                if (aviWriter != null)
                {
                    aviWriter.Close();
                    MessageBox.Show($"Recording saved to:\n{outputPath}", "Success!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error finalizing recording: {ex.Message}", "Error");
            }
        }
        
        public bool IsRecording => isRecording;
        public int FramesRecorded => frameCount;
        public int MaxFrames => MaxDurationSeconds * RecordingFPS;
        public double RecordingProgress => MaxFrames > 0 ? (double)frameCount / MaxFrames : 0;
        
        public void Dispose()
        {
            StopRecording();
            frameGraphics?.Dispose();
            frameBuffer?.Dispose();
            aviWriter?.Dispose();
        }
    }
}
