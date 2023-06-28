using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Project.StickFlickShapesRecognition.Scripts
{
    public class StickGestureMatcher
    {
        private readonly StickGesturesNode _root;
        private StickGesturesNode _currentNode;

        private readonly StringBuilder _matchingPattern;
        public string CurrentPattern => _matchingPattern.ToString();

        public StickGestureMatcher(List<StickGesture> gestures)
        {
            _root = new StickGesturesNode(' ');
            _currentNode = _root;
            
            foreach (var gesture in gestures)
            {
                AddGesture(gesture);
            }
            
            _matchingPattern = new StringBuilder();
        }
        
        
        public void AddGesture(StickGesture gesture)
        {
            foreach (var pattern in gesture.Patterns)
            {
                if (pattern.Length > 0)
                {
                    AddGesture(_root, pattern, gesture);
                }
            }
        }

        
        private void AddGesture(StickGesturesNode node, string pattern, StickGesture gesture)
        {
            var searchingNode = node.Children.FirstOrDefault(n => n.Value == pattern[0]);

            if (pattern.Length == 1)
            {
                var newNode = new StickGesturesNode(pattern[0], gesture);
                (searchingNode ?? node).Children.Add(newNode);
            }
            else
            {
                if (searchingNode == null)
                {
                    searchingNode = new StickGesturesNode(pattern[0]);
                    node.Children.Add(searchingNode);
                }

                AddGesture(searchingNode, pattern[1..], gesture);
            }
        }


        public void ResetPattern()
        {
            _matchingPattern.Clear();
            _currentNode = _root;
        }


        public void AddToken(char token)
        {
            _matchingPattern.Append(token);
            try
            {
                _currentNode = _currentNode.Children.First(n => n.Value == token);
            }
            catch (InvalidOperationException e)
            {
            }
        }


        public bool Match(out StickGesture gesture)
        {
            if (_currentNode.IsEndOfPattern)
            {
                gesture = _currentNode.Gesture;
                return true;
            }
            
            gesture = null;
            return false;
        }
        
        
        public bool CheckPartialMatching()
        {
            return _currentNode.Children.Count > 0;
        }
    }
}