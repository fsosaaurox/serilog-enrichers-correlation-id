﻿using FakeItEasy;
using NUnit.Framework;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Tests.Support;
using Microsoft.AspNetCore.Http;
using NUnit.Framework.Legacy;

namespace Serilog.Tests.Enrichers
{
    [TestFixture]
    [Parallelizable]
    public class CorrelationIdEnricherTests
    {
        [SetUp]
        public void SetUp()
        {
            _httpContextAccessor = A.Fake<IHttpContextAccessor>();
            _enricher = new CorrelationIdEnricher(_httpContextAccessor);
        }

        private IHttpContextAccessor _httpContextAccessor;
        private CorrelationIdEnricher _enricher;

        [Test]
        public void When_CurrentHttpContextIsNotNull_Should_CreateCorrelationIdProperty()
        {
            A.CallTo(() => _httpContextAccessor.HttpContext)
                .Returns(new DefaultHttpContext());

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(_enricher)
                .WriteTo.Sink(new DelegateSink.DelegatingSink(e => evt = e))
                .CreateLogger();

            log.Information(@"Has a CorrelationId property");

            ClassicAssert.NotNull(evt);
            ClassicAssert.IsTrue(evt.Properties.ContainsKey("CorrelationId"));
            ClassicAssert.NotNull(evt.Properties["CorrelationId"].LiteralValue());
        }

        [Test]
        public void When_CurrentHttpContextIsNotNull_ShouldNot_CreateCorrelationIdProperty()
        {
            A.CallTo(() => _httpContextAccessor.HttpContext)
                .Returns(null);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(_enricher)
                .WriteTo.Sink(new DelegateSink.DelegatingSink(e => evt = e))
                .CreateLogger();

            log.Information(@"Does not have a CorrelationId property");

            ClassicAssert.NotNull(evt);
            ClassicAssert.IsFalse(evt.Properties.ContainsKey("CorrelationId"));
        }

        [Test]
        public void When_MultipleLoggingCallsMade_Should_KeepUsingCreatedCorrelationIdProperty()
        {
            var httpContext = new DefaultHttpContext();

            A.CallTo(() => _httpContextAccessor.HttpContext)
                .Returns(httpContext);

            LogEvent evt = null;
            var log = new LoggerConfiguration()
                .Enrich.With(_enricher)
                .WriteTo.Sink(new DelegateSink.DelegatingSink(e => evt = e))
                .CreateLogger();

            log.Information(@"Has a CorrelationId property");

            var correlationId = evt.Properties["CorrelationId"].LiteralValue();

            log.Information(@"Here is another event");

            ClassicAssert.AreEqual(correlationId, evt.Properties["CorrelationId"].LiteralValue());
        }
    }
}
