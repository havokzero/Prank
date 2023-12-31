using System;
using System.Threading;
using NAudio.Wave;

public class Audio : IDisposable
{
    private SemaphoreSlim playbackSemaphore = new SemaphoreSlim(1, 1); // Ensure one audio at a time
    private IWavePlayer waveOutDevice;
    private AudioFileReader audioFileReader;
    private string[] audioFilePaths; // Array of audio file paths
    private int currentAudioIndex; // To track which audio is playing

    public Audio()
    {
        // Initialize the array of audio file paths
        audioFilePaths = new string[] { @"Media\getajob.wav", @"Media\psyops.wav" };
        currentAudioIndex = 0; // Start with the first file

        // Initialize the wave output device
        waveOutDevice = new WaveOutEvent();
        waveOutDevice.PlaybackStopped += OnPlaybackStopped;

        // Begin playback of the first audio file
        PlayNextAudioFile();
    }

    private void PlayNextAudioFile()
    {
        // Wait for any previous audio playback to complete
        playbackSemaphore.Wait();

        try
        {
            // Dispose of the previous AudioFileReader if it exists
            audioFileReader?.Dispose();

            // Initialize the next AudioFileReader
            audioFileReader = new AudioFileReader(audioFilePaths[currentAudioIndex]);
            waveOutDevice.Init(audioFileReader);
            waveOutDevice.Play();

            // Update the index for the next audio file
            currentAudioIndex = (currentAudioIndex + 1) % audioFilePaths.Length;
        }
        finally
        {
            // Release the semaphore when playback of this file is initialized
            playbackSemaphore.Release();
        }
    }

    private void OnPlaybackStopped(object sender, StoppedEventArgs args)
    {
        // Start playing the next audio file once the current one stops
        PlayNextAudioFile();
    }

    public void Start()
    {
        // If playback is stopped, start or resume playback
        if (waveOutDevice.PlaybackState != PlaybackState.Playing)
        {
            waveOutDevice.Play();
        }
    }

    public void Stop()
    {
        // Stop playback immediately
        waveOutDevice?.Stop();
    }

    public void Dispose()
    {
        // Clean up resources
        waveOutDevice?.Stop();
        waveOutDevice?.Dispose();
        waveOutDevice = null;

        audioFileReader?.Dispose();
        audioFileReader = null;

        playbackSemaphore?.Dispose();
    }
}