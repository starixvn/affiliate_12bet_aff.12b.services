namespace Aff12B.Common.Tests.Fixtures
{
    /// <summary>
    /// Base class for memory-intensive tests that need explicit garbage collection
    /// </summary>
    public abstract class MemoryTestBase : IDisposable
    {
        private bool _disposed = false;

        protected MemoryTestBase()
        {
            // Force garbage collection before test to start with clean memory
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// Forces garbage collection after memory-intensive operations
        /// Call this after creating large datasets or heavy operations
        /// </summary>
        protected static void ForceGarbageCollection()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Force cleanup of any remaining objects
                    ForceGarbageCollection();
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
