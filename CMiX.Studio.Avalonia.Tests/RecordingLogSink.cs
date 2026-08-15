using Avalonia.Logging;

namespace CMiX.Studio.Avalonia.Tests
{
    // Wraps whatever sink was installed so tests can capture log lines without silencing the
    // normal Avalonia trace output, and restore the previous sink afterward.
    public class RecordingLogSink : ILogSink
    {
        private readonly ILogSink? _inner;

        public RecordingLogSink(ILogSink? inner)
        {
            _inner = inner;
        }

        public List<LogEntry> Entries { get; } = new();

        public bool IsEnabled(LogEventLevel level, string area)
        {
            return true;
        }

        public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
        {
            Entries.Add(new LogEntry(level, area, messageTemplate, Array.Empty<object?>()));
            _inner?.Log(level, area, source, messageTemplate);
        }

        public void Log<T0>(LogEventLevel level, string area, object? source, string messageTemplate, T0 propertyValue0)
        {
            Entries.Add(new LogEntry(level, area, messageTemplate, new object?[] { propertyValue0 }));
            _inner?.Log(level, area, source, messageTemplate, propertyValue0);
        }

        public void Log<T0, T1>(LogEventLevel level, string area, object? source, string messageTemplate, T0 propertyValue0, T1 propertyValue1)
        {
            Entries.Add(new LogEntry(level, area, messageTemplate, new object?[] { propertyValue0, propertyValue1 }));
            _inner?.Log(level, area, source, messageTemplate, propertyValue0, propertyValue1);
        }

        public void Log<T0, T1, T2>(LogEventLevel level, string area, object? source, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2)
        {
            Entries.Add(new LogEntry(level, area, messageTemplate, new object?[] { propertyValue0, propertyValue1, propertyValue2 }));
            _inner?.Log(level, area, source, messageTemplate, propertyValue0, propertyValue1, propertyValue2);
        }

        public void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues)
        {
            propertyValues ??= Array.Empty<object?>();
            Entries.Add(new LogEntry(level, area, messageTemplate, propertyValues));
            _inner?.Log(level, area, source, messageTemplate, propertyValues);
        }

        public readonly struct LogEntry
        {
            public LogEntry(LogEventLevel level, string area, string messageTemplate, object?[] propertyValues)
            {
                Level = level;
                Area = area;
                MessageTemplate = messageTemplate;
                PropertyValues = propertyValues;
            }

            public LogEventLevel Level { get; }
            public string Area { get; }
            public string MessageTemplate { get; }
            public object?[] PropertyValues { get; }

            // Cheap stand in for structured template formatting: good enough to substring
            // search for a known fragment across both the template and its property values.
            public string FlattenedText =>
                MessageTemplate + " " + string.Join(" ", PropertyValues.Select(v => v?.ToString() ?? string.Empty));
        }
    }
}
