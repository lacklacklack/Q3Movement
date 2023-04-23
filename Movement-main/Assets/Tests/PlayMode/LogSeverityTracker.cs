using UnityEngine;
using System.Collections.Generic;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Linq;

    /// <summary>
    ///   This tracker will assert for both warnings and errors. 
    ///   In [OneTimeSetUp], add some messages to ignore, and register the tracker:
    ///       m_logTracker.IgnoredMessages.AddRange(new[] { ... });
    ///       m_logTracker.Register()
    ///   Then, in a test, Reset(), do your work and AssertCleanLog():
    ///       m_logTracker.Reset();
    ///       ...
    ///       m_logTracker.AssertCleanLog("describe the context");
    /// </summary>
    public class LogSeverityTracker {
        public readonly List<string> IgnoredMessages = new List<string>();

        public void Register() {
            Application.logMessageReceived -= KeepSeverestMessage;
            Application.logMessageReceived += KeepSeverestMessage;
            // make sure the ignored messages can't kill us 
            foreach (var ignoredMsg in IgnoredMessages) {
                LogAssert.Expect(LogType.Error, ignoredMsg);
            }
        }

        public void Reset() {
            m_strongestLogSeverity = 0;
            m_strongestLogType = LogType.Log;
        }

        public void AssertCleanLog(string msg = null) {
            var prefix = string.IsNullOrEmpty(msg) ? "" : (msg + ": ");
            Assert.That(m_strongestLogType, Is.EqualTo(LogType.Log), prefix + $"found severe {m_strongestLogType}:\n{m_strongestLog}");
        }

        // -------------------------------------------------- private state

        private string m_strongestLog;
        private int m_strongestLogSeverity = 0;
        private LogType m_strongestLogType = LogType.Log;

        // -------------------------------------------------- private logic

        private void KeepSeverestMessage(string logString, string stackTrace, LogType type) {
            bool isIgnored = IgnoredMessages.Any(msg => logString.Contains(msg));
            if (isIgnored) {
                return;
            }

            int newSeverity = ScoreSeverityOf(type);
            if (newSeverity > m_strongestLogSeverity) {
                m_strongestLog = logString;
                m_strongestLogSeverity = newSeverity;
                m_strongestLogType = type;
            }
        }

        /// <summary>
        ///   annoyingly the LogType enum does not have semantic ordering! so let's work out our own.
        ///   higher numbers mean more severe.
        /// </summary>
        private int ScoreSeverityOf(LogType log) {
            switch (log) {
                case LogType.Log: return 0;
                case LogType.Warning: return 1;
                case LogType.Assert: return 2;
                case LogType.Error: return 3;
                case LogType.Exception: return 4;
                default:
                    Assert.Fail($"unknown log type {log}");
                    return int.MaxValue;
            }
        }
    }