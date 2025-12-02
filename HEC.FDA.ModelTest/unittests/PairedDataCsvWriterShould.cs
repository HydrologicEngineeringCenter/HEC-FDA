using Xunit;
using HEC.FDA.Model.paireddata;
using HEC.FDA.Model.utilities;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HEC.FDA.ModelTest.unittests
{
    [Trait("RunsOn", "Remote")]
    public class PairedDataCsvWriterShould : IDisposable
    {
        private readonly string _testDirectory;

        public PairedDataCsvWriterShould()
        {
            // Create a unique test directory for this test run
            _testDirectory = Path.Combine(Path.GetTempPath(), $"PairedDataCsvWriterTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);
        }

        public void Dispose()
        {
            // Clean up test directory after tests
            if (Directory.Exists(_testDirectory))
            {
                try
                {
                    Directory.Delete(_testDirectory, recursive: true);
                }
                catch
                {
                    // Best effort cleanup
                }
            }
        }

        [Fact]
        public void CreateFileWithDefaultHeader()
        {
            string filePath = Path.Combine(_testDirectory, "test_header.csv");

            using (var writer = new PairedDataCsvWriter(filePath))
            {
                writer.Flush();
            }

            Assert.True(File.Exists(filePath));
            string[] lines = File.ReadAllLines(filePath);
            Assert.Single(lines);
            Assert.Equal("X,Y", lines[0]);
        }

        [Fact]
        public void CreateFileWithCustomHeader()
        {
            string filePath = Path.Combine(_testDirectory, "test_custom_header.csv");
            string customHeader = "Stage,Damage";

            using (var writer = new PairedDataCsvWriter(filePath, customHeader))
            {
                writer.Flush();
            }

            Assert.True(File.Exists(filePath));
            string[] lines = File.ReadAllLines(filePath);
            Assert.Single(lines);
            Assert.Equal("Stage,Damage", lines[0]);
        }

        [Fact]
        public void WriteSinglePairedDataCorrectly()
        {
            string filePath = Path.Combine(_testDirectory, "test_single_paired_data.csv");
            double[] xVals = { 1.0, 2.0, 3.0 };
            double[] yVals = { 10.0, 20.0, 30.0 };
            PairedData data = new PairedData(xVals, yVals);

            using (var writer = new PairedDataCsvWriter(filePath))
            {
                writer.Enqueue(data);
                writer.Flush();
            }

            string[] lines = File.ReadAllLines(filePath);
            Assert.Equal(4, lines.Length); // Header + 3 data rows
            Assert.Equal("X,Y", lines[0]);
            Assert.Equal("1,10", lines[1]);
            Assert.Equal("2,20", lines[2]);
            Assert.Equal("3,30", lines[3]);
        }

        [Fact]
        public void WriteMultiplePairedDataCorrectly()
        {
            string filePath = Path.Combine(_testDirectory, "test_multiple_paired_data.csv");

            PairedData data1 = new PairedData(new double[] { 1.0, 2.0 }, new double[] { 10.0, 20.0 });
            PairedData data2 = new PairedData(new double[] { 3.0, 4.0 }, new double[] { 30.0, 40.0 });

            using (var writer = new PairedDataCsvWriter(filePath))
            {
                writer.Enqueue(data1);
                writer.Enqueue(data2);
                writer.Flush();
            }

            string[] lines = File.ReadAllLines(filePath);
            Assert.Equal(5, lines.Length); // Header + 4 data rows
            Assert.Equal("X,Y", lines[0]);
            Assert.Equal("1,10", lines[1]);
            Assert.Equal("2,20", lines[2]);
            Assert.Equal("3,30", lines[3]);
            Assert.Equal("4,40", lines[4]);
        }

        [Fact]
        public void HandleParallelEnqueuesCorrectly()
        {
            string filePath = Path.Combine(_testDirectory, "test_parallel_enqueues.csv");
            int numThreads = 10;
            int itemsPerThread = 5;

            using (var writer = new PairedDataCsvWriter(filePath, batchSize: 10))
            {
                Parallel.For(0, numThreads, i =>
                {
                    for (int j = 0; j < itemsPerThread; j++)
                    {
                        double baseVal = i * 100 + j;
                        PairedData data = new PairedData(
                            new double[] { baseVal, baseVal + 0.5 },
                            new double[] { baseVal * 2, (baseVal + 0.5) * 2 }
                        );
                        writer.Enqueue(data);
                    }
                });

                writer.Flush();
            }

            string[] lines = File.ReadAllLines(filePath);
            // Header + (numThreads * itemsPerThread * 2) data rows
            int expectedLines = 1 + (numThreads * itemsPerThread * 2);
            Assert.Equal(expectedLines, lines.Length);

            // Verify header
            Assert.Equal("X,Y", lines[0]);

            // Verify all data rows have valid numeric values
            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(',');
                Assert.Equal(2, parts.Length);
                Assert.True(double.TryParse(parts[0], out _), $"Line {i} X value is not a valid double: {parts[0]}");
                Assert.True(double.TryParse(parts[1], out _), $"Line {i} Y value is not a valid double: {parts[1]}");
            }
        }

        [Fact]
        public void UpdateEnqueuedCountCorrectly()
        {
            string filePath = Path.Combine(_testDirectory, "test_enqueued_count.csv");

            using (var writer = new PairedDataCsvWriter(filePath, batchSize: 100))
            {
                Assert.Equal(0, writer.EnqueuedCount);

                writer.Enqueue(new PairedData(new double[] { 1.0 }, new double[] { 1.0 }));
                Assert.Equal(1, writer.EnqueuedCount);

                writer.Enqueue(new PairedData(new double[] { 2.0 }, new double[] { 2.0 }));
                Assert.Equal(2, writer.EnqueuedCount);

                writer.Enqueue(new PairedData(new double[] { 3.0 }, new double[] { 3.0 }));
                Assert.Equal(3, writer.EnqueuedCount);
            }
        }

        [Fact]
        public void UpdateWrittenCountAfterFlush()
        {
            string filePath = Path.Combine(_testDirectory, "test_written_count.csv");

            using (var writer = new PairedDataCsvWriter(filePath, batchSize: 100))
            {
                writer.Enqueue(new PairedData(new double[] { 1.0 }, new double[] { 1.0 }));
                writer.Enqueue(new PairedData(new double[] { 2.0 }, new double[] { 2.0 }));
                writer.Enqueue(new PairedData(new double[] { 3.0 }, new double[] { 3.0 }));

                Assert.Equal(0, writer.WrittenCount);

                writer.Flush();

                Assert.Equal(3, writer.WrittenCount);
            }
        }

        [Fact]
        public void TriggerAutomaticBatchWrite()
        {
            string filePath = Path.Combine(_testDirectory, "test_batch_write.csv");
            int batchSize = 5;

            using (var writer = new PairedDataCsvWriter(filePath, batchSize: batchSize))
            {
                // Enqueue exactly batch size items
                for (int i = 0; i < batchSize; i++)
                {
                    writer.Enqueue(new PairedData(new double[] { i }, new double[] { i * 2 }));
                }

                // Give background worker time to process
                Thread.Sleep(500);

                // Should have automatically written
                Assert.Equal(batchSize, writer.WrittenCount);
            }
        }

        [Fact]
        public void ThrowArgumentExceptionForEmptyFilePath()
        {
            Assert.Throws<ArgumentException>(() => new PairedDataCsvWriter(""));
            Assert.Throws<ArgumentException>(() => new PairedDataCsvWriter(null));
            Assert.Throws<ArgumentException>(() => new PairedDataCsvWriter("   "));
        }

        [Fact]
        public void ThrowArgumentOutOfRangeExceptionForInvalidBatchSize()
        {
            string filePath = Path.Combine(_testDirectory, "test.csv");
            Assert.Throws<ArgumentOutOfRangeException>(() => new PairedDataCsvWriter(filePath, batchSize: 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new PairedDataCsvWriter(filePath, batchSize: -1));
        }

        [Fact]
        public void ThrowArgumentNullExceptionForNullPairedData()
        {
            string filePath = Path.Combine(_testDirectory, "test_null_data.csv");

            using (var writer = new PairedDataCsvWriter(filePath))
            {
                Assert.Throws<ArgumentNullException>(() => writer.Enqueue(null));
            }
        }

        [Fact]
        public void ThrowObjectDisposedExceptionAfterDisposal()
        {
            string filePath = Path.Combine(_testDirectory, "test_disposed.csv");
            var writer = new PairedDataCsvWriter(filePath);
            writer.Dispose();

            Assert.Throws<ObjectDisposedException>(() => writer.Enqueue(new PairedData(new double[] { 1.0 }, new double[] { 1.0 })));
            Assert.Throws<ObjectDisposedException>(() => writer.Flush());
        }

        [Fact]
        public void CreateDirectoryIfNotExists()
        {
            string subDirectory = Path.Combine(_testDirectory, "new_subdir", "nested");
            string filePath = Path.Combine(subDirectory, "test.csv");

            Assert.False(Directory.Exists(subDirectory));

            using (var writer = new PairedDataCsvWriter(filePath))
            {
                writer.Flush();
            }

            Assert.True(Directory.Exists(subDirectory));
            Assert.True(File.Exists(filePath));
        }

        [Fact]
        public void HandleMismatchedArrayLengths()
        {
            string filePath = Path.Combine(_testDirectory, "test_mismatched.csv");

            // X has 3 values, Y has 2 values
            PairedData data = new PairedData(
                new double[] { 1.0, 2.0, 3.0 },
                new double[] { 10.0, 20.0 }
            );

            using (var writer = new PairedDataCsvWriter(filePath))
            {
                writer.Enqueue(data);
                writer.Flush();
            }

            string[] lines = File.ReadAllLines(filePath);
            Assert.Equal(4, lines.Length); // Header + 3 rows
            Assert.Equal("X,Y", lines[0]);
            Assert.Equal("1,10", lines[1]);
            Assert.Equal("2,20", lines[2]);
            Assert.Contains("NaN", lines[3]); // Y value should be NaN for missing data
        }

        [Fact]
        public void SimulateMonteCarloParallelWriting()
        {
            string filePath = Path.Combine(_testDirectory, "test_monte_carlo_simulation.csv");
            int numIterations = 100;
            Random random = new Random(12345);

            using (var writer = new PairedDataCsvWriter(filePath, batchSize: 20))
            {
                // Simulate parallel Monte Carlo iterations
                Parallel.For(0, numIterations, iteration =>
                {
                    // Generate random paired data for this iteration
                    int numPoints = 5;
                    double[] xVals = new double[numPoints];
                    double[] yVals = new double[numPoints];

                    for (int i = 0; i < numPoints; i++)
                    {
                        xVals[i] = i * 10.0;
                        yVals[i] = random.NextDouble() * 100.0;
                    }

                    PairedData data = new PairedData(xVals, yVals);
                    writer.Enqueue(data);
                });

                writer.Flush();
            }

            // Verify results
            string[] lines = File.ReadAllLines(filePath);
            int expectedLines = 1 + (numIterations * 5); // Header + 100 iterations * 5 points each
            Assert.Equal(expectedLines, lines.Length);

            // Verify all data is valid
            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(',');
                Assert.Equal(2, parts.Length);
                Assert.True(double.TryParse(parts[0], out double x));
                Assert.True(double.TryParse(parts[1], out double y));
            }
        }

        [Fact]
        public void AllowMultipleFlushCalls()
        {
            string filePath = Path.Combine(_testDirectory, "test_multiple_flush.csv");

            using (var writer = new PairedDataCsvWriter(filePath, batchSize: 100))
            {
                writer.Enqueue(new PairedData(new double[] { 1.0 }, new double[] { 1.0 }));
                writer.Flush();

                writer.Enqueue(new PairedData(new double[] { 2.0 }, new double[] { 2.0 }));
                writer.Flush();

                writer.Enqueue(new PairedData(new double[] { 3.0 }, new double[] { 3.0 }));
                writer.Flush();
            }

            string[] lines = File.ReadAllLines(filePath);
            Assert.Equal(4, lines.Length); // Header + 3 data rows
            Assert.Equal("X,Y", lines[0]);
            Assert.Equal("1,1", lines[1]);
            Assert.Equal("2,2", lines[2]);
            Assert.Equal("3,3", lines[3]);
        }

        [Fact]
        public void DisposeFlushesRemainingData()
        {
            string filePath = Path.Combine(_testDirectory, "test_dispose_flush.csv");

            using (var writer = new PairedDataCsvWriter(filePath, batchSize: 100))
            {
                writer.Enqueue(new PairedData(new double[] { 1.0 }, new double[] { 1.0 }));
                writer.Enqueue(new PairedData(new double[] { 2.0 }, new double[] { 2.0 }));
                // Don't call Flush - dispose should handle it
            }

            // Verify data was written
            string[] lines = File.ReadAllLines(filePath);
            Assert.Equal(3, lines.Length); // Header + 2 data rows
        }
    }
}
