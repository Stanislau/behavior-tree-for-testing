using Medbullets.CrossCutting.Extensions;
using NUnit.Framework;

namespace BehaviorTreeForTests.Tests.Common
{
    public static class Claim
    {
        public static void Equal(decimal actual, decimal expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void Equal<T>(T actual, T expected, string message = null)
        {
            Assert.AreEqual(expected, actual, message);
        }

        public static void Satisfies<T>(this T item, Func<T, bool> condition)
        {
            Assert.True(condition(item));
        }

        public static void ClaimEqual<T>(this T actual, T expected, string message = null) => Equal(actual, expected, message);

        public static void ClaimReferenceNotEqual<T>(this T actual, T expected, string message = null) => ReferenceNotEqual(actual, expected);

        public static void ClaimNotEqual<T>(this T actual, T expected) => NotEqual(actual, expected);

        public static void ClaimNotEqual<T>(this T actual, T expected, string message) => NotEqual(actual, expected, message);

        public static void ClaimFalse(this bool item) => False(item);

        public static void ClaimTrue(this bool item) => True(item);

        public static void Fail(string message)
        {
            Assert.Fail(message);
        }

        public static void NotImplemented()
        {
            Assert.Fail("Not Implemented");
        }

        public static void True(bool condition, string message = null)
        {
            Assert.True(condition, message ?? string.Empty);
        }

        public static void NotEmpty(string str)
        {
            Assert.False(string.IsNullOrWhiteSpace(str));
        }

        public static void Empty(string str)
        {
            Assert.True(string.IsNullOrWhiteSpace(str));
        }

        public static void IsNotNull(object item)
        {
            Assert.IsNotNull(item);
        }

        public static void ClaimIsNull(this object item) => IsNull(item);

        public static void ClaimForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static void ClaimIsNotNull(this object item) => IsNotNull(item);

        public static void CollectionNotEmpty<T>(IEnumerable<T> items)
        {
            Assert.IsNotEmpty(items, "It is empty, but should not.");
        }

        public static void CollectionEmpty<T>(IEnumerable<T> items)
        {
            Assert.IsEmpty(items, $"It is not empty: {items.ToFormattedString()}");
        }

        public static void ClaimCollectionEmpty<T>(this IEnumerable<T> items) => CollectionEmpty(items);

        public static void ClaimCollectionIs<T>(this IEnumerable<T> items, params T[] expected)
        {
            if (items.Count() != expected.Length)
            {
                Assert.Fail($"Actual count [{items.Count()}] is not equal expected [{expected.Length}]. Origin: {items.ToFormattedString()}, Expected: {expected.ToFormattedString()}");
            }

            foreach (var (current, index) in items.Indexed())
            {
                current.ClaimEqual(expected[index], $"Item at {index} is different. Actual: {current}, expected: {expected[index]}. Origin: {items.ToFormattedString()}, Expected: {expected.ToFormattedString()}");
            }
        }

        public static void ClaimCollectionNotEmpty<T>(this IEnumerable<T> items) => CollectionNotEmpty(items);

        public static void False(bool condition)
        {
            Assert.False(condition);
        }

        public static void ShouldContain<T>(IEnumerable<T> items, T item)
        {
            Assert.True(items.Any(x => x.Equals(item)));
        }

        public static void ShouldContain<T>(IEnumerable<T> items, Func<T, bool> predicate)
        {
            Assert.True(items.Any(predicate));
        }

        public static void ShouldNotContain<T>(IEnumerable<T> items, T item)
        {
            Assert.False(items.Any(x => x.Equals(item)));
        }

        public static void ShouldNotContain<T>(IEnumerable<T> items, Func<T, bool> predicate)
        {
            Assert.False(items.Any(predicate));
        }

        public static void NotEqual<T>(T actual, T expected)
        {
            Assert.AreNotEqual(expected, actual);
        }

        public static void NotEqual<T>(T actual, T expected, string message)
        {
            Assert.AreNotEqual(expected, actual, message);
        }

        public static void IsNull(object item)
        {
            Assert.IsNull(item);
        }

        public static async Task ExpectException(Func<Task> operation)
        {
            var exceptionOccurs = false;

            try
            {
                await operation();
            }
            catch
            {
                exceptionOccurs = true;
            }

            if (exceptionOccurs == false)
            {
                Claim.Fail("Exception is expected.");
            }
        }

        public static void ExpectException(Action operation)
        {
            var exceptionOccurs = false;

            try
            {
                operation();
            }
            catch
            {
                exceptionOccurs = true;
            }

            if (exceptionOccurs == false)
            {
                Claim.Fail("Exception is expected.");
            }
        }

        public static void ExpectExceptionOfType<T>(Action operation)
            where T : Exception
        {
            var exceptionOccurs = false;

            try
            {
                operation();
            }
            catch (T)
            {
                exceptionOccurs = true;
            }
            catch (Exception ex)
            {
                Claim.Fail($"Expected exception of type {typeof(T).Name}, but it was {ex.GetType().Name}.");
            }

            if (exceptionOccurs == false)
            {
                Claim.Fail("Exception is expected.");
            }
        }

        public static async Task ExpectExceptionOfTypeAsync<T>(Func<Task> operation)
            where T : Exception
        {
            var exceptionOccurs = false;

            try
            {
                await operation();
            }
            catch (T)
            {
                exceptionOccurs = true;
            }
            catch (Exception ex)
            {
                Claim.Fail($"Expected exception of type {typeof(T).Name}, but it was {ex.GetType().Name}.");
            }

            if (exceptionOccurs == false)
            {
                Claim.Fail("Exception is expected.");
            }
        }

        public static async Task<T> ExpectException<T>(Func<Task<T>> operation)
            where T : class
        {
            var exceptionOccurs = false;
            T result = null;

            try
            {
                result = await operation();
            }
            catch
            {
                exceptionOccurs = true;
            }

            if (exceptionOccurs == false)
            {
                Claim.Fail("Exception is expected.");
            }

            return null;
        }

        public static void Is<T>(object c3)
        {
            Claim.True(c3 is T);
        }

        public static void ClaimIs<T>(this object item) => Is<T>(item);

        public static void ClaimIsNot<T>(this object item) => IsNot<T>(item);

        public static void IsNot<T>(object c3)
        {
            Claim.False(c3 is T);
        }

        public static void ReferenceNotEqual(object o1, object o2, string message = null)
        {
            Claim.False(ReferenceEquals(o1, o2));
        }
    }
}