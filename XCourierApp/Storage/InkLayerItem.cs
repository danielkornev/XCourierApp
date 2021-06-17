using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Inking;

namespace XCourierApp.Storage
{
	public sealed class InkLayerItem
	{
		private readonly List<XInkStroke> _inkStrokes = new List<XInkStroke>();

        /// <summary>
        /// gets or sets the ink strokes
        /// </summary>
        public IEnumerable<XInkStroke> InkStrokes
        {
            get => _inkStrokes;

            set
            {
                lock (_inkStrokes)
                {
                    _inkStrokes.Clear();
                    _inkStrokes.AddRange(value);
                }
            }
        }

        public void Add(IEnumerable<XInkStroke> strokes)
        {
            lock (_inkStrokes)
            {
                _inkStrokes.AddRange(strokes);
            }
        }

        public void Remove(XInkStroke stroke)
        {
            lock (_inkStrokes)
            {
                _inkStrokes.Remove(stroke);
            }
        }
    } // class
} // namespace
