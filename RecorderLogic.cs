using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
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
        private string tempAudioPath;
        
        public int RecordingWidth { get; set; } = 1920;
        public int RecordingHeight { get; set; } = 1080;
        public int RecordingFPS { get; set; } = 60;
        public int MaxDurationSeconds { get; set; } = 300;
        
        private WasapiLoopbackCapture audioCapture;
        private WaveFileWriter audioFileWriter;
        private Bitmap frameBuffer;
        private Graphics frameGraphics;
        private VisualizerForm visualizerForm;
        private VisualizerLogic visualizerLogic;
        private AviWriter aviWriter;
        
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
                
                // Create temp audio file
                string tempDir = Path.Combine(Path.GetTempPath(), "NekoBeatsRecording");
                Directory.CreateDirectory(tempDir);
                tempAudioPath = Path.Combine(tempDir, Guid.NewGuid().ToString() + ".wav");
                
                // Initialize AVI writer
                aviWriter = new AviWriter(outputPath)
                {
                    FramesPerSecond = fps,
                    Width = width,
                    Height = height
                };
                
                // Initialize frame buffer
                frameBuffer = new Bitmap(RecordingWidth, RecordingHeight, PixelFormat.Format32bppRgb);
                frameGraphics = Graphics.FromImage(frameBuffer);
                
                // Start audio capture
                InitializeAudioCapture();
                
                isRecording = true;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start recording: {ex.Message}", "Recording Error");
                return false;
            }
        }
        
        public void StopRecording()
        {
            if (!isRecording) return;
            isRecording = false;
            
            try
            {
                audioCapture?.StopRecording();
                audioFileWriter?.Dispose();
            }
            catch { }
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
                
                // Write frame to AVI
                BitmapData bmpData = frameBuffer.LockBits(
                    new Rectangle(0, 0, frameBuffer.Width, frameBuffer.Height),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppRgb);
                
                byte[] frameData = new byte[Math.Abs(bmpData.Stride) * frameBuffer.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, frameData, 0, frameData.Length);
                frameBuffer.UnlockBits(bmpData);
                
                aviWriter.AddFrame(frameData);
                frameCount++;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Frame capture error: {ex.Message}", "Recording Error");
                StopRecording();
            }
        }
        
        private void InitializeAudioCapture()
        {
            try
            {
                audioCapture = new WasapiLoopbackCapture();
                audioFileWriter = new WaveFileWriter(tempAudioPath, audioCapture.WaveFormat);
                audioCapture.DataAvailable += (s, e) =>
                {
                    if (isRecording)
                        audioFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
                };
                audioCapture.StartRecording();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Audio capture failed: {ex.Message}", "Audio Error");
            }
        }
        
        private void FinalizeRecording()
        {
            try
            {
                aviWriter?.Close();
                MessageBox.Show($"Recording saved to:\n{outputPath}", "Success!");
                
                // Clean up temp audio file
                try
                {
                    if (File.Exists(tempAudioPath))
                        File.Delete(tempAudioPath);
                }
                catch { }
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
            audioCapture?.Dispose();
            audioFileWriter?.Dispose();
            aviWriter?.Dispose();
            
            try
            {
                if (File.Exists(tempAudioPath))
                    File.Delete(tempAudioPath);
            }
            catch { }
        }
    }
}
