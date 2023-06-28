using System.Collections.Generic;

namespace Project.StickFlickShapesRecognition.Scripts
{
    public class StickGesturesNode
    {
        public List<StickGesturesNode> Children { get; private set; }
        public char Value { get; private set; }
        public StickGesture Gesture { get; private set; }
        public bool IsEndOfPattern { get; private set; }

        public StickGesturesNode(char value, StickGesture gesture = null)
        {
            Value = value;
            Children = new List<StickGesturesNode>();
            Gesture = gesture;
            if (gesture != null)
            {
                IsEndOfPattern = true;
            }
        }
    }
}