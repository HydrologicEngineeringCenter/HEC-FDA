using HEC.FDA.Model.paireddata;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;

namespace HEC.FDA.Model.utilities
{
    /// <summary>
    /// Thread-safe CSV writer for paired data from Monte Carlo computations.
    /// Supports concurrent enqueueing of paired data with batched, asynchronous writing to disk.
    /// </summary>
    public class PairedDataCsvWriter : IDisposable
    {
        #region Fields
        private readonly ConcurrentQueue<PairedData> _dataQueue;
        private readonly BackgroundWorker _writer;
        private readonly object _writerLock = new object();
        private readonly string _filePath;
        private readonly int _batchSize;
        private int _enqueuedCount;
        private int _writtenCount;
        private bool _disposed;
        private bool _includeHeader;
        private string _headerText;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of paired data items enqueued for writing.
        /// </summary>
        public int EnqueuedCount => _enqueuedCount;

        /// <summary>
        /// Gets the number of paired data items successfully written to the CSV file.
        /// </summary>
        public int WrittenCount => _writtenCount;

        /// <summary>
        /// Gets whether there are pending items in the queue waiting to be written.
        /// </summary>
        public bool HasPendingWrites => !_dataQueue.IsEmpty;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new thread-safe CSV writer for paired data.
        /// </summary>
        /// <param name="filePath">The full file path where the CSV will be written.</param>
        /// <param name="batchSize">Number of items to accumulate before triggering a write operation. Default is 100.</param>
        /// <param name="includeHeader">Whether to include a header row (X,Y) in the CSV. Default is true.</param>
        /// <exception cref="ArgumentException">Thrown when filePath is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when batchSize is less than 1.</exception>
        public PairedDataCsvWriter(string filePath, int batchSize = 100, bool includeHeader = true)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
            }

            if (batchSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize), "Batch size must be at least 1.");
            }

            _filePath = filePath;
            _batchSize = batchSize;
            _includeHeader = includeHeader;
            _headerText = "X,Y";
            _dataQueue = new ConcurrentQueue<PairedData>();
            _enqueuedCount = 0;
            _writtenCount = 0;

            _writer = new BackgroundWorker();
            _writer.DoWork += WriteQueuedDataToFile;

            // Ensure the directory exists
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Initialize the file with header if requested
            if (_includeHeader)
            {
                WriteHeader();
            }
        }

        /// <summary>
        /// Creates a new thread-safe CSV writer for paired data with a custom header.
        /// </summary>
        /// <param name="filePath">The full file path where the CSV will be written.</param>
        /// <param name="headerText">Custom header text to write as the first line.</param>
        /// <param name="batchSize">Number of items to accumulate before triggering a write operation. Default is 100.</param>
        /// <exception cref="ArgumentException">Thrown when filePath or headerText is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when batchSize is less than 1.</exception>
        public PairedDataCsvWriter(string filePath, string headerText, int batchSize = 100)
            : this(filePath, batchSize, false)
        {
            if (string.IsNullOrWhiteSpace(headerText))
            {
                throw new ArgumentException("Header text cannot be null or empty.", nameof(headerText));
            }

            _includeHeader = true;
            _headerText = headerText;
            WriteHeader();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Enqueues a paired data object for writing to the CSV file.
        /// Thread-safe and can be called from parallel Monte Carlo iterations.
        /// </summary>
        /// <param name="data">The paired data to write.</param>
        /// <exception cref="ObjectDisposedException">Thrown when the writer has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
        public void Enqueue(PairedData data)
        {
            ThrowIfDisposed();

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Paired data cannot be null.");
            }

            _dataQueue.Enqueue(data);
            int currentCount = Interlocked.Increment(ref _enqueuedCount);

            // Trigger batch write if we've reached the batch size
            if (currentCount % _batchSize == 0)
            {
                TriggerWrite();
            }
        }

        /// <summary>
        /// Forces all queued data to be written to the file immediately.
        /// Blocks until all pending writes are complete.
        /// Call this before closing the application or when you need to ensure all data is persisted.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown when the writer has been disposed.</exception>
        public void Flush()
        {
            ThrowIfDisposed();

            // Wait for any ongoing background write to complete
            while (_writer.IsBusy)
            {
                Thread.Sleep(10);
            }

            lock (_writerLock)
            {
                // If there's pending data, write it now
                if (!_dataQueue.IsEmpty)
                {
                    WriteQueuedDataToFile(null, null);
                }
            }
        }

        /// <summary>
        /// Disposes the writer and ensures all pending data is written to disk.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            // Flush any remaining data
            if (!_dataQueue.IsEmpty)
            {
                Flush();
            }

            _writer?.Dispose();
            _disposed = true;
        }
        #endregion

        #region Private Methods
        private void WriteHeader()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(_filePath, append: false, Encoding.UTF8))
                {
                    sw.WriteLine(_headerText);
                }
            }
            catch (Exception ex)
            {
                throw new IOException($"Failed to write header to CSV file: {_filePath}", ex);
            }
        }

        private void TriggerWrite()
        {
            lock (_writerLock)
            {
                // Only start a new background write if one isn't already running
                if (!_writer.IsBusy)
                {
                    _writer.RunWorkerAsync();
                }
            }
        }

        private void WriteQueuedDataToFile(object sender, DoWorkEventArgs e)
        {
            if (_dataQueue.IsEmpty)
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            int itemsProcessed = 0;

            // Dequeue all available items
            while (_dataQueue.TryDequeue(out PairedData data))
            {
                // Write each X-Y pair on separate rows
                for (int i = 0; i < data.Xvals.Length; i++)
                {
                    double xVal = data.Xvals[i];
                    double yVal = i < data.Yvals.Length ? data.Yvals[i] : double.NaN;

                    sb.AppendLine($"{xVal},{yVal}");
                }

                itemsProcessed++;
            }

            // Write accumulated data to file
            if (sb.Length > 0)
            {
                try
                {
                    lock (_writerLock)
                    {
                        using (StreamWriter sw = new StreamWriter(_filePath, append: true, Encoding.UTF8))
                        {
                            sw.Write(sb.ToString());
                            sw.Flush();
                        }

                        Interlocked.Add(ref _writtenCount, itemsProcessed);
                    }
                }
                catch (Exception ex)
                {
                    // Re-enqueue failed items (simplified error handling)
                    throw new IOException($"Failed to write paired data to CSV file: {_filePath}", ex);
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(PairedDataCsvWriter));
            }
        }
        #endregion
    }
}
