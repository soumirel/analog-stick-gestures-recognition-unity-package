using UnityEngine;

namespace Project.StickFlickShapesRecognition.Scripts
{
    [CreateAssetMenu(fileName = "ShapeRecognitionSettings", menuName = "Settings/ShapeRecognition", order = 0)]
    public class StickGesturesRecognatorSettings : ScriptableObject
    {
        [Header("Zones scales")]
        [SerializeField] private float _deadZoneRadius;
        [SerializeField] private float _activationRadius;
        
        [Space] 
        
        [Header("Times scales")]
        
        [SerializeField] private float _maxShapeRecognitionSeconds;
        [SerializeField] private float _deadZoneCrossingSeconds;
        [SerializeField] private float _neutralZoneCrossingSeconds;
        [SerializeField] private float _secondsToContinueRecognizedGesture;



        #region Read-only properties

        public float DeadZoneRadius => _deadZoneRadius;
        public float ActivationRadius => _activationRadius;
        
        public float DeadZoneCrossingSeconds => _deadZoneCrossingSeconds;
        public float MaxShapeRecognitionSeconds => _maxShapeRecognitionSeconds;
        public float NeutralZoneCrossingSeconds => _neutralZoneCrossingSeconds;
        public float SecondsToContinueRecognizedGesture => _secondsToContinueRecognizedGesture;
        
        #endregion
    }
}
