﻿/* The MIT License (MIT)
* 
* Copyright (c) 2016 Marc Clifton
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using System.Collections.Generic;
using System.Drawing;

namespace FlowSharpLib
{
	/// <summary>
	/// Left-down dynamic connector. (horizontal line, vertical line at right.)
	/// Routing around shapes is ignored, which means that the best route may include going inside a connected shape.
	/// </summary>
	public class DynamicConnectorLD : DynamicConnector
	{
		public override Rectangle UpdateRectangle { get { return DisplayRectangle.Grow(anchorWidthHeight + 1 + BorderPen.Width); } }

		public DynamicConnectorLD(Canvas canvas) : base(canvas)
		{
			Initialize();
		}

		public DynamicConnectorLD(Canvas canvas, Point start, Point end) : base(canvas)
		{
			Initialize();
			startPoint = start;
			endPoint = end;
            DisplayRectangle = RecalcDisplayRectangle();
        }

        protected void Initialize()
		{
			lines.Add(new HorizontalLine(canvas));
			lines.Add(new VerticalLine(canvas));
		}

		public override List<ShapeAnchor> GetAnchors()
		{
			Size szAnchor = new Size(anchorWidthHeight, anchorWidthHeight);

			int startxOffset = startPoint.X < endPoint.X ? 0 : -anchorWidthHeight;
			int endyOffset = startPoint.Y < endPoint.Y ? -anchorWidthHeight : 0;

			return new List<ShapeAnchor>() {
				new ShapeAnchor(GripType.Start, new Rectangle(startPoint.Move(startxOffset, -anchorWidthHeight/2), szAnchor)),
				new ShapeAnchor(GripType.End, new Rectangle(endPoint.Move(-anchorWidthHeight/2, endyOffset), szAnchor)),
			};
		}

		public override GraphicElement CloneDefault(Canvas canvas)
		{
			DynamicConnectorLD line = (DynamicConnectorLD)base.CloneDefault(canvas);
			line.StartCap = StartCap;
			line.EndCap = EndCap;

			return line;
		}

		public override void UpdatePath()
		{
            UpdateCaps();

            if (startPoint.X < endPoint.X)
			{
				lines[0].DisplayRectangle = new Rectangle(startPoint.X, startPoint.Y - BaseController.MIN_HEIGHT / 2, endPoint.X - startPoint.X, BaseController.MIN_HEIGHT);
			}
			else
			{
				lines[0].DisplayRectangle = new Rectangle(endPoint.X, startPoint.Y - BaseController.MIN_HEIGHT / 2, startPoint.X - endPoint.X, BaseController.MIN_HEIGHT);
			}

			if (startPoint.Y < endPoint.Y)
			{
				lines[1].DisplayRectangle = new Rectangle(endPoint.X - BaseController.MIN_WIDTH / 2, startPoint.Y, BaseController.MIN_WIDTH, endPoint.Y - startPoint.Y);
			}
			else
			{
				lines[1].DisplayRectangle = new Rectangle(endPoint.X - BaseController.MIN_WIDTH / 2, endPoint.Y, BaseController.MIN_WIDTH, startPoint.Y - endPoint.Y);
			}

			lines.ForEach(l => l.UpdatePath());
		}

        protected void UpdateCaps()
        {
            if (startPoint.X < endPoint.X)
            {
                lines[0].StartCap = StartCap;
                lines[0].EndCap = AvailableLineCap.None;
            }
            else
            {
                lines[0].StartCap = AvailableLineCap.None;
                lines[0].EndCap = StartCap;
            }

            if (startPoint.Y < endPoint.Y)
            {
                lines[1].StartCap = AvailableLineCap.None;
                lines[1].EndCap = EndCap;
            }
            else
            {
                lines[1].StartCap = EndCap;
                lines[1].EndCap = AvailableLineCap.None;
            }

            lines.ForEach(l => l.UpdateProperties());
        }
    }
}
