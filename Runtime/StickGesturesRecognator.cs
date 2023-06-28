using System;
using System.Collections.Generic;
using Project.StickFlickShapesRecognition.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.StickFlickShapes.Scripts
{
    public class StickGesturesRecognator : MonoBehaviour
    {
        const int ZONES_COUNT = 8;
        
        [SerializeField] private StickGesturesRecognatorSettings _settings;
        
        [SerializeField] private List<StickGesture> _shapes;
        private StickGestureMatcher _matcher;

        private Vector2 _stickPosition;

        private bool _onRecognition;
        private bool _onRecognized;
        private bool _readyToRecognition;

        private StickGesture _recognizedGesture;
        
        private float _secondsInDeadZone;
        private float _secondsInNeutralZone;
        private float _recognitionSeconds;
        private float _secondsAfterRecognition;

        private int _lastVisitedZone;


        private void Awake()
        {
            _matcher = new StickGestureMatcher(_shapes);
        }

        private void OnEnable()
        {
            _readyToRecognition = true;
        }


        public void OnStickFlick(InputAction.CallbackContext context)
        {
            SetStickPosition(context.ReadValue<Vector2>());
        }


        private void SetStickPosition(Vector2 stickPosition)
        {
            _stickPosition = stickPosition;
        }

        private void Update()
        {
            _analogHistory[Time.frameCount & 31] = _stickPosition;
            float stickMagnitude = _stickPosition.magnitude;
            
            if (_onRecognition)
            {
                _recognitionSeconds += Time.deltaTime;
                if (_recognitionSeconds > _settings.MaxShapeRecognitionSeconds)
                {
                    FinishRecognition();
                }
                if (_onRecognized)
                {
                    _secondsAfterRecognition += Time.deltaTime;
                    if (_secondsAfterRecognition > _settings.SecondsToContinueRecognizedGesture)
                    {
                        FinishRecognition();
                    }
                }
            }

            if (stickMagnitude < _settings.DeadZoneRadius)
            {
                OnDeadZone();
            }
            else if (stickMagnitude <= _settings.ActivationRadius)
            {
                OnNeutralZone();
            }
            else
            {
                OnTargetZone();
            }
        }


        private void OnDeadZone()
        {
            _secondsInNeutralZone = 0;
            
            if (_onRecognition)
            {
                _secondsInDeadZone += Time.deltaTime;
                if (_secondsInDeadZone > _settings.DeadZoneCrossingSeconds)
                {
                    FinishRecognition();
                }
            }
            else
            {
                _readyToRecognition = true;
            }
        }


        private void OnNeutralZone()
        {
            _secondsInDeadZone = 0;
            if (_onRecognition)
            {
                _secondsInNeutralZone += Time.deltaTime;
                if (_secondsInNeutralZone > _settings.NeutralZoneCrossingSeconds)
                {
                    FinishRecognition();
                }
            }
        }


        private void OnTargetZone()
        {
            _secondsInDeadZone = 0;
            _secondsInDeadZone = 0;

            if (_readyToRecognition)
            {
                StartRecognition();
            }

            if (_onRecognition)
            {
                if (ProgressFlick(_stickPosition))
                {
                    if (_matcher.Match(out var gesture))
                    {
                        _recognizedGesture = gesture;
                        _onRecognized = true;
                        _secondsAfterRecognition = 0;
                        
                        if (!_matcher.CheckPartialMatching())
                        {
                            FinishRecognition();
                        }
                    }
                }
            }
        }
        
        
        // Try appending a new entry to the combo.
        bool ProgressFlick(Vector2 stick) {        
            const float BUCKETS_PER_RADIAN = ZONES_COUNT / (2 * Mathf.PI);

            // Get the angle of the stick, and round it to one of a fixed number of buckets.
            float angle = Mathf.Atan2(-stick.x, -stick.y);
            int visitingZone = Mathf.RoundToInt(angle * BUCKETS_PER_RADIAN + ZONES_COUNT / 2.0f) % ZONES_COUNT;
            // If we've changed buckets, add a new letter to our combo.
            if (visitingZone != _lastVisitedZone) {
                _matcher.AddToken((char)('A' + visitingZone));
                _lastVisitedZone = visitingZone;
                Debug.Log(_lastVisitedZone);
                return true;
            }
            // Otherwise, nothing new to report.
            return false;
        }

        private void StartRecognition()
        {
            print("StartRecognition");
            _readyToRecognition = false;
            _onRecognition = true;
            
            _matcher.ResetPattern();
            _recognizedGesture = null;
            _recognitionSeconds = 0f;
            _lastVisitedZone = -1;
        }


        private void FinishRecognition()
        {
            if (_recognizedGesture is not null)
            {
                Debug.Log($"{_recognizedGesture.Name} with {_matcher.CurrentPattern} pattern processed");
            }
            else
            {
                Debug.Log($"Unknown pattern: {_matcher.CurrentPattern}");
            }
            
            _onRecognition = false;
            _onRecognized = false;
        }
        
#if UNITY_EDITOR
        Vector2[] _analogHistory = new Vector2[32];
        private void OnDrawGizmos() {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1, 1, 0));
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(Vector3.zero, 1.0f);
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(Vector3.zero, _settings.ActivationRadius);
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(Vector3.zero, _settings.DeadZoneRadius);

            Gizmos.color = Color.grey;
            for(int i = 0; i < ZONES_COUNT; i++) {
                Vector3 direction = Quaternion.Euler(0, 0, (360.0f / ZONES_COUNT) * (i + 0.5f)) * Vector3.up;
                Gizmos.DrawLine( 1f * direction,  _settings.ActivationRadius * direction);           
            }

            Gizmos.color = _recognitionSeconds == 0f ? Color.blue : Color.red;
            Vector3 lastPosition = _analogHistory[(Time.frameCount + 1) & 31];
            for (int x = 2; x <= 32; x++) {
                var next = _analogHistory[(Time.frameCount + x) & 31];
                Gizmos.DrawLine(lastPosition, next);
                lastPosition = next;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lastPosition, 0.1f);
        }
#endif
    }
}