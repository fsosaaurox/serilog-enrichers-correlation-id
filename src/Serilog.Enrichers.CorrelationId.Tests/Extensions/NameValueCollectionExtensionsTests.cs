using System.Collections.Specialized;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Serilog.Enrichers.CorrelationId.Extensions;

namespace Serilog.Tests.Extensions
{
    [TestFixture]
    [Parallelizable]
    public class NameValueCollectionExtensionsTests
    {
        [Test]
        public void TryGetValue_ReturnsTrue_WhenKeyIsFound()
        {
            var collection = new NameValueCollection {{"MyKey", "MyValue"}};

            ClassicAssert.IsTrue(collection.TryGetValue("MyKey", out _));
        }

        [Test]
        public void TryGetValue_ReturnsFalse_WhenKeyIsNotFound()
        {
            var collection = new NameValueCollection {{"MyKey", "MyValue"}};

            ClassicAssert.IsFalse(collection.TryGetValue("BadKey", out _));
        }

        private static readonly string[] expected = ["MyValue"];

        [Test]
        public void TryGetValue_SetsValues_WhenKeyIsFound()
        {
            var collection = new NameValueCollection {{"MyKey", "MyValue"}};

            collection.TryGetValue("MyKey", out var values);

            ClassicAssert.AreEqual(expected, values);
        }

        [Test]
        public void TryGetValue_SetsValuesToNull_WhenKeyIsNotFound()
        {
            var collection = new NameValueCollection {{"MyKey", "MyValue"}};

            collection.TryGetValue("BadKey", out var values);

            ClassicAssert.IsNull(values);
        }
    }
}
