using Moq;

namespace Aff12B.Common.Tests.Mocks
{
    /// <summary>
    /// Helper class for creating common mock repository setups
    /// </summary>
    public static class MockRepositoryHelper
    {
        /// <summary>
        /// Creates a mock that returns a successful result for async operations
        /// </summary>
        /// <typeparam name="T">Repository interface type</typeparam>
        /// <returns>Mock repository</returns>
        public static Mock<T> CreateSuccessfulAsyncMock<T>() where T : class
        {
            return new Mock<T>();
        }

        /// <summary>
        /// Sets up a mock to return a specific result for an async method
        /// </summary>
        /// <typeparam name="TMock">Mock type</typeparam>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="mock">Mock instance</param>
        /// <param name="methodCall">Method call expression</param>
        /// <param name="result">Result to return</param>
        public static void SetupAsyncResult<TMock, TResult>(
            Mock<TMock> mock,
            System.Linq.Expressions.Expression<Func<TMock, Task<TResult>>> methodCall,
            TResult result) where TMock : class
        {
            mock.Setup(methodCall).ReturnsAsync(result);
        }

        /// <summary>
        /// Sets up a mock to return a list result for an async method
        /// </summary>
        /// <typeparam name="TMock">Mock type</typeparam>
        /// <typeparam name="TItem">List item type</typeparam>
        /// <param name="mock">Mock instance</param>
        /// <param name="methodCall">Method call expression</param>
        /// <param name="items">Items to return</param>
        public static void SetupAsyncListResult<TMock, TItem>(
            Mock<TMock> mock,
            System.Linq.Expressions.Expression<Func<TMock, Task<IEnumerable<TItem>>>> methodCall,
            IEnumerable<TItem> items) where TMock : class
        {
            mock.Setup(methodCall).ReturnsAsync(items);
        }

        /// <summary>
        /// Sets up a mock to throw an exception for an async method
        /// </summary>
        /// <typeparam name="TMock">Mock type</typeparam>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="mock">Mock instance</param>
        /// <param name="methodCall">Method call expression</param>
        /// <param name="exception">Exception to throw</param>
        public static void SetupAsyncException<TMock, TResult>(
            Mock<TMock> mock,
            System.Linq.Expressions.Expression<Func<TMock, Task<TResult>>> methodCall,
            Exception exception) where TMock : class
        {
            mock.Setup(methodCall).ThrowsAsync(exception);
        }

        /// <summary>
        /// Sets up a mock to return empty result for async methods
        /// </summary>
        /// <typeparam name="TMock">Mock type</typeparam>
        /// <typeparam name="TItem">List item type</typeparam>
        /// <param name="mock">Mock instance</param>
        /// <param name="methodCall">Method call expression</param>
        public static void SetupEmptyAsyncResult<TMock, TItem>(
            Mock<TMock> mock,
            System.Linq.Expressions.Expression<Func<TMock, Task<IEnumerable<TItem>>>> methodCall) where TMock : class
        {
            mock.Setup(methodCall).ReturnsAsync(new List<TItem>());
        }

        /// <summary>
        /// Verifies that an async method was called once
        /// </summary>
        /// <typeparam name="TMock">Mock type</typeparam>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="mock">Mock instance</param>
        /// <param name="methodCall">Method call expression</param>
        public static void VerifyAsyncMethodCalledOnce<TMock, TResult>(
            Mock<TMock> mock,
            System.Linq.Expressions.Expression<Func<TMock, Task<TResult>>> methodCall) where TMock : class
        {
            mock.Verify(methodCall, Times.Once);
        }

        /// <summary>
        /// Verifies that an async method was never called
        /// </summary>
        /// <typeparam name="TMock">Mock type</typeparam>
        /// <typeparam name="TResult">Result type</typeparam>
        /// <param name="mock">Mock instance</param>
        /// <param name="methodCall">Method call expression</param>
        public static void VerifyAsyncMethodNeverCalled<TMock, TResult>(
            Mock<TMock> mock,
            System.Linq.Expressions.Expression<Func<TMock, Task<TResult>>> methodCall) where TMock : class
        {
            mock.Verify(methodCall, Times.Never);
        }
    }
}
