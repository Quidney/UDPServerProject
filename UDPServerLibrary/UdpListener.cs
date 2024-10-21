using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using UDPServerLibrary.Model;

namespace UDPServerLibrary
{
    public class UdpListener : IListener, IDisposable
    {
        public bool IsListening { get; private set; }
        public event Action<IPEndPoint, string>? MessageReceived;

        private CancellationTokenSource? _cts;
        private readonly IEncoder _encoder;
        private UdpClient? _client;

        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private bool _disposed;

        public UdpListener(IEncoder encoder) => _encoder = encoder;

        public async Task StartListening(int port)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            await _semaphore.WaitAsync();
            try
            {
                if (IsListening)
                    await StopListening();

                _cts = new();
                _client = new(port);
                IsListening = true;
            }
            finally
            {
                _semaphore.Release();
            }

            while (_cts?.IsCancellationRequested == false)
            {
                try
                {
                    UdpReceiveResult result = await _client.ReceiveAsync(_cts.Token);
                    string message = _encoder.BytesToString(result.Buffer);
                    MessageReceived?.Invoke(result.RemoteEndPoint, message);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Socket exception: {ex.SocketErrorCode} - \"{ex.Message}\"");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unhandled Exception: \"{ex.Message}\"");
                    break;
                }
            }
        }

        public async Task StopListening()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);

            if (!IsListening)
                return;

            await _semaphore.WaitAsync();
            try
            {
                _client?.Dispose();
                _client = null;

                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;

                IsListening = false;
            }
            finally
            {
                _semaphore.Release();
            }
        }


        protected virtual async void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    await StopListening();
                    _semaphore.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}