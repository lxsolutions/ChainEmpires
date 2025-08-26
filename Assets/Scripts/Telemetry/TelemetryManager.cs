








using UnityEngine;
using System.Collections.Generic;

namespace ChainEmpires.Telemetry
{
    /// <summary>
    /// Telemetry manager that tracks game events and sends them to analytics.
    /// </summary>
    public class TelemetryManager : MonoBehaviour
    {
        // Singleton instance
        private static TelemetryManager _instance;
        public static TelemetryManager Instance => _instance;

        [Header("Telemetry Settings")]
        public string telemetryEndpoint = "http://localhost:8081/events";
        public bool enableTelemetry = true;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log("Telemetry Manager initialized");
        }

        /// <summary>
        /// Logs a session start event.
        /// </summary>
        public void LogSessionStart()
        {
            if (!enableTelemetry) return;

            var data = new Dictionary<string, object>
            {
                { "event_type", "session_start" },
                { "timestamp", System.DateTime.UtcNow.ToString("o") }
            };

            SendEvent(data);
        }

        /// <summary>
        /// Logs a session end event.
        /// </summary>
        public void LogSessionEnd()
        {
            if (!enableTelemetry) return;

            var data = new Dictionary<string, object>
            {
                { "event_type", "session_end" },
                { "timestamp", System.DateTime.UtcNow.ToString("o") }
            };

            SendEvent(data);
        }

        /// <summary>
        /// Logs a building event.
        /// </summary>
        public void LogBuilding(string buildingType, string action)
        {
            if (!enableTelemetry) return;

            var data = new Dictionary<string, object>
            {
                { "event_type", "building" },
                { "timestamp", System.DateTime.UtcNow.ToString("o") },
                { "building_type", buildingType },
                { "action", action }
            };

            SendEvent(data);
        }

        /// <summary>
        /// Logs a raid event.
        /// </summary>
        public void LogRaid(string target, bool success)
        {
            if (!enableTelemetry) return;

            var data = new Dictionary<string, object>
            {
                { "event_type", "raid" },
                { "timestamp", System.DateTime.UtcNow.ToString("o") },
                { "target", target },
                { "success", success }
            };

            SendEvent(data);
        }

        /// <summary>
        /// Logs a win/loss event.
        /// </summary>
        public void LogWinLoss(bool isVictory)
        {
            if (!enableTelemetry) return;

            var data = new Dictionary<string, object>
            {
                { "event_type", "victory" },
                { "timestamp", System.DateTime.UtcNow.ToString("o") }
            };

            SendEvent(data);
        }

        /// <summary>
        /// Sends an event to the telemetry endpoint.
        /// </summary>
        private void SendEvent(Dictionary<string, object> data)
        {
            if (string.IsNullOrEmpty(telemetryEndpoint)) return;

            // In a real implementation, this would send HTTP POST request
            Debug.Log($"Telemetry: {data["event_type"]} - {System.Text.Json.JsonSerializer.Serialize(data)}");

            // Example of how to send in production:
            /*
            StartCoroutine(SendEventToServer(data));
            */

            void SendEventToServer(Dictionary<string, object> eventData)
            {
                // Implementation would use UnityWebRequest or similar
            }
        }
    }
}








