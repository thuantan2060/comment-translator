//------------------------------------------------------------------------------
// <copyright file="TranslateAdornment.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace CommentTranslator.Presentation
{
    /// <summary>
    /// Adornment class that draws a square box in the top right hand corner of the viewport
    /// </summary>
    internal sealed class TranslateAdornment
    {
        /// <summary>
        /// Text view to add the adornment on.
        /// </summary>
        private readonly IWpfTextView _view;

        /// <summary>
        /// The layer for the adornment.
        /// </summary>
        private readonly IAdornmentLayer _layer;
        private List<TranslatePopup> _translatePopups = new List<TranslatePopup>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateAdornment"/> class.
        /// Creates a square image and attaches an event handler to the layout changed event that
        /// adds the the square in the upper right-hand corner of the TextView via the adornment layer
        /// </summary>
        /// <param name="view">The <see cref="IWpfTextView"/> upon which the adornment will be drawn</param>
        public TranslateAdornment(IWpfTextView view)
        {
            _view = view ?? throw new ArgumentNullException("view");
            _layer = view.GetAdornmentLayer("TranslateAdornment");

            //this.view.ViewportHeightChanged += this.OnSizeChanged;
            //this.view.ViewportWidthChanged += this.OnSizeChanged;
        }

        public void AddTranslate(SnapshotSpan span, string text)
        {
            var viewportSize = new Size(_view.ViewportWidth, _view.ViewportHeight);
            var popup = new TranslatePopup(span, text, viewportSize);

            _translatePopups.Add(popup);
            _layer.AddAdornment(span, null, popup);

            popup.Focus();
        }

        /// <summary>
        /// Event handler for viewport height or width changed events. Adds adornment at the top right corner of the viewport.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void OnSizeChanged(object sender, EventArgs e)
        {
            // Clear the adornment layer of previous adornments
            this._layer.RemoveAllAdornments();

            //// Place the image in the top right hand corner of the Viewport
            //Canvas.SetLeft(this.image, this.view.ViewportRight - RightMargin - AdornmentWidth);
            //Canvas.SetTop(this.image, this.view.ViewportTop + TopMargin);

            // Add the image to the adornment layer and make it relative to the viewport
            //this.adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, this.image, null);
        }
    }
}
